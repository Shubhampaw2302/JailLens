using JailLensApi.Data;
using JailLensApi.Data.Models;
using JailLensApi.Infrastructure.IService;
using Microsoft.EntityFrameworkCore;

namespace JailLensApi.Infrastructure.Service
{
    public class FaceService : IFaceService
    {
        private readonly JailLensDbContext _dbContext;

        public FaceService(JailLensDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FacialEvent>> GetFacialEvents()
        {
            try
            {
                var faceEvents = new List<FacialEvent>();

                var inmates = _dbContext.inmate.Select(inmate => inmate.intakeid).ToList();
                var locations = _dbContext.locations.Select(location => location.locationname).ToList();

                int inmateCount = inmates.Count;
                int locationCount = locations.Count;

                inmates = Shuffle(inmates, inmateCount);
                locations = Shuffle(locations, locationCount);

                Random random = new();
                int range = random.Next(Math.Min(inmateCount, locationCount));

                for (var i = 0; i < range; i++)
                {
                    Random r = new();
                    int locationIndex = r.Next(locationCount);

                    var faceEvent = new FacialEvent
                    {
                        IntakeId = inmates[i],
                        Location = locations[locationIndex],
                        EventDateTime = DateTime.Now,
                    };
                    faceEvents.Add(faceEvent);
                }

                return faceEvents;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting facial events : ", ex);
            }
        }

        public static List<T> Shuffle<T>(List<T> list, int count)
        {
            Random random = new Random();

            int n = count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public async Task<string> AddFacialEvents(FaceRecognitionEvents faceEvent)
        {
            try
            {
                _dbContext.facerecognitionevents.Add(faceEvent);
                await _dbContext.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding facial events : ", ex);
            }
        }

        public async Task ProcessFacialEvents()
        {
            try
            {
                var unprocessedEvents = _dbContext.facerecognitionevents.Where(evnt => evnt.isprocessed == 0).ToList();
                var actualProgram = "";

                if (unprocessedEvents.Count == 0)
                {
                    Console.WriteLine("No unprocessed face events found");
                }
                else
                {
                    foreach (var evnt in unprocessedEvents)
                    {
                        var faceEvent = new FacialEvent
                        {
                            IntakeId = evnt.personrecognized,
                            Location = evnt.location,
                            EventDateTime = evnt.eventdatetime
                        };

                        var isLocAppropriate = await VerifySchedule(faceEvent);
                        var programName = _dbContext.programs.Where(program => program.programdefaultlocation == evnt.location)
                                                             .Select(program => program.programname).FirstOrDefault();
                        var attendance = "A";
                        int isProcessed = 2;

                        Console.WriteLine("\n\n");
                        if (isLocAppropriate)
                        {
                            // Mark the attendance as present
                            Console.WriteLine($"Inmate {evnt.personrecognized} is at the correct location");
                            attendance = "P";
                            isProcessed = 1;
                        }
                        else
                        {
                            // Make an entry to the alerts table
                            Console.WriteLine($"Inmate {evnt.personrecognized} is at the wrong location");
                            var inmateName = _dbContext.inmate.Where(inmate => inmate.intakeid == evnt.personrecognized)
                                                              .Select(inmate => inmate.firstname).FirstOrDefault();


                            var actualPrograms = _dbContext.Set<ProgramResponse>().FromSqlRaw($"SELECT getactualprogram('{evnt.personrecognized}', '{evnt.eventdatetime}') AS actualprogram").ToList();

                            // Add the actual programname to the alerts and attendance tables

                            if (actualPrograms.Count == 0)
                            {
                                var idleProgram = new ProgramResponse
                                {
                                    actualprogram = "Sit Idle"
                                };

                                actualPrograms.Add(idleProgram);
                            }

                            actualProgram = actualPrograms[0].actualprogram;

                            var alert = new JailLensAlert
                            {
                                jaillensalertid = 0,
                                intakeid = evnt.personrecognized,
                                programname = programName,
                                inmatename = inmateName,
                                alertdescription = $"{inmateName} was supposed to undertake {actualProgram} at {evnt.eventdatetime}, but was found engaging in {programName} in {evnt.location}",
                                alertcategory = "Security Breach",
                                comments = null,
                                createddate = DateTime.Now,
                                isprocessed = 0,
                                actualprogramname = actualProgram
                            };
                            _dbContext.jaillensalert.Add(alert);
                        }
                        
                        Console.WriteLine("\n\n");

                        // Mark Attendance
                        var attendanceData = new Attendance
                        {
                            id = 0,
                            intakeid = evnt.personrecognized,
                            programname = programName,
                            timeslot = evnt.eventdatetime,
                            attendance = attendance,
                            actualprogramname = attendance == "P" ? null : actualProgram
                        };

                        _dbContext.attendance.Add(attendanceData);

                        // Update Isprocessed column of face rec table
                        var eventToUpdate = await _dbContext.facerecognitionevents.FindAsync(evnt.eventid);
                        eventToUpdate.isprocessed = isProcessed;
                    }
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while processing face events : ", ex);
            }
        }

        public async Task<bool> VerifySchedule(FacialEvent evnt)
        {
            try
            {
                string timeNoted = evnt.EventDateTime.ToLongTimeString();
                var res = _dbContext.Set<Response>().FromSqlRaw($"SELECT VerifySchedule('{evnt.IntakeId}', '{evnt.Location}', '{timeNoted}') AS response").ToList();

                return res.FirstOrDefault().response;
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong ", ex);
            }
        }
    }
}
