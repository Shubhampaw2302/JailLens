using JailLensApi.Data;
using JailLensApi.Data.Models;
using JailLensApi.Infrastructure.IService;
using Microsoft.Extensions.Logging;

namespace JailLensApi.Infrastructure.Service
{
    public class AlertsService : IAlertsService
    {
        private readonly JailLensDbContext _dbContext;
        private readonly ILogger _logger;

        public AlertsService(JailLensDbContext dbContext, ILogger<AlertsService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<List<JailLensAlert>> GetAlerts()
        {
            try
            {
                return _dbContext.jaillensalert.Where(alert => alert.isprocessed == 0).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting jail alerts : ", ex);
            }
        }

        public async Task ProcessAlert(JailLensAlert alert)
        {
            try
            {
                var alertToProcess = await _dbContext.jaillensalert.FindAsync(alert.jaillensalertid);

                alertToProcess.alertdescription = alert.alertdescription;
                alertToProcess.alertcategory = alert.alertcategory;
                alertToProcess.comments = alert.comments;
                alertToProcess.isprocessed = 1;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while processing jail alerts : ", ex);
            }
        }

        public async Task<int> GetAlertsCount()
        {
            try
            {
                return _dbContext.jaillensalert.Where(alert => alert.isprocessed == 0).Count();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in controller : ", ex.Message);
                throw new Exception("Error while getting alert count : ", ex);
            }
        }

        public async Task AddAbsenteeismAlert(int programId)
        {
            try
            {
                string programName = _dbContext.programs.Where(program => program.programid == programId)
                                                   .Select(program => program.programname).First();

                var inmatesRegisteredInProgram = _dbContext.inmateschedule.Where(schedule => schedule.programid == programId)
                                                                          .Select(schedule => schedule.intakeid)
                                                                          .ToList();

                foreach(var inmate in inmatesRegisteredInProgram)
                {
                    var absent = await IsAbsent(inmate);

                    if (absent)
                    {
                        var inmateName = _dbContext.inmate.Where(mate => mate.intakeid == inmate)
                                                          .Select(mate => mate.firstname)
                                                          .First();

                        var alert = new JailLensAlert
                        {
                            jaillensalertid = 0,
                            intakeid = inmate,
                            programname = programName,
                            inmatename = inmateName,
                            alertdescription = $"{inmateName} was absent for the program {programName} on {DateTime.Today}",
                            alertcategory = "Program Absence",
                            comments = null,
                            createddate = DateTime.Now,
                            isprocessed = 0,
                            actualprogramname = programName
                        };

                        _dbContext.jaillensalert.Add(alert);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while posting absenteeism alert : ", ex);
            }
        }

        public async Task<bool> IsAbsent(string intakeid)
        {
            try
            {
                var currentDate = DateTime.Today;

                var result = _dbContext.attendance.Where(att => att.timeslot.Date == currentDate && att.intakeid == intakeid && att.attendance == "P").ToList();

                return result.Count == 0;             // True if absent, false if present
            }
            catch (Exception ex)
            {
                throw new Exception("Error while verifying absence : ", ex);
            }
        }

        public async Task<List<ProgramEndResponse>> GetPrograms()
        {
            try
            {
                return _dbContext.programs.Select(prog => new ProgramEndResponse
                {
                    programid = prog.programid,
                    defaultendtime = prog.defaultendtime.ToLongTimeString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting alert count : ", ex);
            }
        }
    }
}
