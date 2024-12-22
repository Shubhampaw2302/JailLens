using JailLensApi.Data;
using JailLensApi.Data.Models;
using Newtonsoft.Json;
using System.Text;

namespace ConsoleApp
{
    public class WebService
    {
        static List<ProgramEndResponse> programs = null;
        static List<int> EndedPrograms = new();

        private static Timer dayEndTimer;

        public static async Task GetPrograms()
        {
            string apiUrl = "https://localhost:7136/api/Alerts/GetAllPrograms";

            using HttpClient client = new();

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var programs = JsonConvert.DeserializeObject<List<ProgramEndResponse>>(responseData);

                WebService.programs = programs;
            }
        }

        public static async Task AddFacialEvents(FacialEvent facialEvent)
        {
            try
            {
                string apiUrl = "https://localhost:7136/api/Face/AddFacialEvents";

                using HttpClient client = new();
                var faceEvent = new FaceRecognitionEvents
                {
                    eventid = 0,
                    location = facialEvent.Location,
                    eventdatetime = facialEvent.EventDateTime,
                    personrecognized = facialEvent.IntakeId,
                    linktomatchreport = "",
                    additionalinfo = null,
                    isprocessed = 0
                };

                string stringFaceEvent = JsonConvert.SerializeObject(faceEvent);

                HttpContent content = new StringContent(stringFaceEvent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Facial Event Registration failure");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong while registering facial event of intake id : {facialEvent.IntakeId} ", ex);
            }
        }

        public static async Task ProcessFacialEvents()
        {
            try
            {
                string apiUrl = "https://localhost:7136/api/Face/ProcessFaceEvents";

                using HttpClient client = new();

                HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Facial Event Registration failure");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while processing facial events : ", ex);
            }
        }

        public static async Task ProcessEndedPrograms()
        {
            foreach(ProgramEndResponse program in programs)
            {
                string apiUrl = "https://localhost:7136/api/Alerts/AddAbsenteeismAlert?programId=" + program.programid;
                TimeOnly endTime = TimeOnly.Parse(program.defaultendtime);
                TimeOnly now = TimeOnly.FromDateTime(DateTime.Now);

                if (endTime <= now && !EndedPrograms.Contains(program.programid))
                {
                    // Call absenteeism API
                    using HttpClient client = new();

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("programId", program.programid.ToString())
                    });

                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error while processing ended programs");
                    }

                    // Test the initialize timer part to initialize the endprograms variable

                    EndedPrograms.Add(program.programid);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            var timerThread = new Thread(InitializeTimer);
            timerThread.Start();
            
            // Populate the programs data which will be used to process ended programs
            await GetPrograms();
            
            using (HttpClient client = new())
            {
                string apiUrl = "https://localhost:7136/api/Face/GetFacialEvents";

                try
                {
                    int eventNum = 0;

                    while (true)
                    {
                        Console.WriteLine("Press any key to generate a face event or press E to Exit : ");

                        string userInput = Console.ReadLine();
                        Console.WriteLine("User has entered : " + userInput);

                        if (userInput.ToLower() != "e")
                        {
                            HttpResponseMessage response = await client.GetAsync(apiUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                string responseData = await response.Content.ReadAsStringAsync();

                                var events = JsonConvert.DeserializeObject<List<FacialEvent>>(responseData);

                                Console.WriteLine($"Event : {eventNum}");

                                if (events.Count == 0)
                                {
                                    Console.WriteLine("No Events found");
                                }
                                else
                                {
                                    foreach (var evnt in events)
                                    {
                                        await AddFacialEvents(evnt);
                                        Console.WriteLine($"Intake ID = {evnt.IntakeId}");
                                        Console.WriteLine($"Location = {evnt.Location}");
                                        Console.WriteLine($"Time = {evnt.EventDateTime}");
                                        Console.WriteLine();
                                    }
                                    // Add to alerts table. UI should display the unprocessed events from the alerts table. User gives some inputs and comments and then the isprocess column is to be updated in the alerts table.
                                    await ProcessFacialEvents();
                                }
                            }
                            else
                            {
                                Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                            }

                            Console.WriteLine("\n\n");
                            eventNum += 1;

                            // Check if any program is ending at this time or has ended recently
                            await ProcessEndedPrograms();
                        }
                        else
                        {
                            Console.WriteLine("Exiting...");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private static void InitializeTimer()
        {
            // Calculate the time remaining until the end of the day (00:00:00)
            DateTime currentTime = DateTime.Now;
            DateTime dayEnd = currentTime.Date.AddDays(1); // Tomorrow at 00:00:00
            TimeSpan timeRemaining = dayEnd - currentTime;

            // Create the timer with the calculated time remaining
            dayEndTimer = new Timer(ExecuteMethodAtDayEnd, null, timeRemaining, Timeout.InfiniteTimeSpan);
        }

        private static void ExecuteMethodAtDayEnd(object state)
        {
            // The ended programs will be initialized once the day ends, i.e at 00:00:00

            EndedPrograms = new List<int>();

            // Reschedule the timer for the next day's end
            InitializeTimer();
        }
    }
}
