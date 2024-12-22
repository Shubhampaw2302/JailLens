using JailLensApi.Data;
using JailLensApi.Data.Models;
using JailLensApi.Infrastructure.IService;
using Microsoft.AspNetCore.Mvc;

namespace JailLensApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAlertsService _alertsService;

        public AlertsController(IAlertsService alertsService, ILogger<AlertsController> logger)
        {
            _logger = logger;
            _alertsService = alertsService;
        }

        [HttpGet]
        [Route("GetUnprocessedAlerts")]
        public async Task<List<JailLensAlert>> GetAlerts()
        {
            try
            {
                return await _alertsService.GetAlerts();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting the alerts : ", ex.Message);
                throw new Exception("Error while getting the alerts : ", ex);
            }
        }

        [HttpPost]
        [Route("ProcessJailAlerts")]
        public async Task<int> ProcessJailAlerts(JailLensAlert alert)
        {
            try
            {
                await _alertsService.ProcessAlert(alert);
                return StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while processing the alert : ", ex.Message);
                throw new Exception("Error while processing the alert : ", ex);
            }
        }

        [HttpGet]
        [Route("GetAlertCount")]
        public async Task<int> ProcessJailAlerts()
        {
            try
            {
                return await _alertsService.GetAlertsCount();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting alert count : ", ex.Message);
                throw new Exception("Error while getting alert count : ", ex);
            }
        }

        [HttpPost]
        [Route("AddAbsenteeismAlert")]
        public async Task<int> AddAbsenteeismAlert(int programId)
        {
            try
            {
                await _alertsService.AddAbsenteeismAlert(programId);
                return StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while adding absenteeism alert : ", ex.Message);
                throw new Exception("Error while adding absenteeism alert : ", ex);
            }
        }

        [HttpGet]
        [Route("GetAllPrograms")]
        public async Task<List<ProgramEndResponse>> GetPrograms()
        {
            try
            {
                return await _alertsService.GetPrograms();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting programs : ", ex.Message);
                throw new Exception("Error while getting programs : ", ex);
            }
        }
    }
}
