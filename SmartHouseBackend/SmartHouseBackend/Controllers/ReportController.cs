using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.InfluxDTOs;
using SmartHouse.Infrastructure.Interfaces.Services;
using SmartHouse.Infrastructure.MqttDTOs;

namespace SmartHouse.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("solar-panel-history/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistory(
            Guid id,
            [FromQuery] string username,
            [FromQuery] string from,
            [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, username, from, to, userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-from-to/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryFromTo(
            Guid id,
            [FromQuery] string from,
            [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, "", from, to, userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-from/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryFrom(
           Guid id,
           [FromQuery] string from)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, "", from, "", userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-to/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryTo(
           Guid id,
           [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, "", "", to, userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-from-username/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryFromUsername(
          Guid id,
           [FromQuery] string username,
          [FromQuery] string from
          )
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, username, from, "", userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-to-username/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryToUsername(
           Guid id,
           [FromQuery] string username,
           [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, username, "", to, userId.Value);
            return GetResult(data);
        }
        [HttpGet("solar-panel-history-username/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SPSActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSolarPanelHistoryUsername(
           Guid id,
           [FromQuery] string username)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetSolarPanelSystemHistory(id, username, "", "", userId.Value);
            return GetResult(data);
        }
        [HttpGet("getLampLuminosityHistory/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(List<LuminosityDataDTO>), 200)]

        public async Task<IActionResult> GetLampLuminosityHistory(Guid id, string startDate, string endDate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var lampHistory = await _reportService.GetLampLuminosityHistory(id, startDate, endDate, userId.Value);
            return GetResult(lampHistory);
        }
        [HttpGet("property-energy-consumption-time-period/{smartPropertyId}/{totalTimePeriod}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<PropertyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEnergyConsumption(Guid smartPropertyId, TotalTimePeriod totalTimePeriod)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetEnergyConsumption(smartPropertyId, totalTimePeriod, userId.Value);
            return GetResult(result);
        }

        [HttpGet("property-energy-consumption/{smartPropertyId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<PropertyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetPropertyEnergyConsumption(Guid smartPropertyId)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetPropertyEnergyConsumption(smartPropertyId, userId.Value);
            return GetResult(result);
        }

        [HttpGet("property-energy-consumption-from-to/{smartPropertyId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<PropertyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetEnergyConsumptionFromTo(Guid smartPropertyId,
            [FromQuery] string from,
            [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetEnergyConsumptionFromTo(smartPropertyId, from, to, userId.Value);
            return GetResult(result);
        }

        [HttpGet("getGateEventHistory/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(List<GateEventInfoDTO>), 200)]
        public async Task<IActionResult> GetGateEventHistory(Guid id, string startDate, string endDate, string? licencePlate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var gateHistory = await _reportService.GetGateEventHistory(id, startDate, endDate, licencePlate, userId.Value);
            return GetResult(gateHistory);
        }

        [HttpGet("getSprinklerEventHistory/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(List<SprinklerEventInfoDTO>), 200)]

        public async Task<IActionResult> GetSprinklerEventHistory(Guid id, string startDate, string endDate, string? username)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var sprinklerHistory = await _reportService.GetSprinklerEventHistory(id, startDate, endDate, username, userId.Value);
            return GetResult(sprinklerHistory);
        }

        [HttpGet("get-device-on-off-history/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(List<DeviceOnOffInfoDTO>), 200)]

        public async Task<IActionResult> GetDeviceOnOffHistory(Guid id, string startDate, string endDate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var deviceOnOff = await _reportService.GetDeviceOnOffHistory(id, startDate, endDate, userId.Value);
            return GetResult(deviceOnOff);
        }

        [HttpGet("get-device-online-offline-history/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(OnlineOfflineHistoryDTO), 200)]

        public async Task<IActionResult> GetDeviceOnlineOfflineHistory(Guid id, string startDate, string endDate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var deviceOnlineOffline = await _reportService.GetDeviceOnlineOfflineHistory(id, startDate, endDate, userId.Value);
            return GetResult(deviceOnlineOffline);
        }

        [HttpGet("get-gate-public-private-history/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(List<GatePublicPrivateInfoDTO>), 200)]
        public async Task<IActionResult> GetGatePublicPrivateHistory(Guid id, string startDate, string endDate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var gatePublicPrivate = await _reportService.GetGatePublicPrivateHistory(id, startDate, endDate, userId.Value);
            return GetResult(gatePublicPrivate);
        }

        [HttpGet("temperature-data/{smartDeviceId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<TemperatureDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetTemperature(Guid smartDeviceId)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetTemperatureData(smartDeviceId, userId.Value);
            return GetResult(result);
        }

        [HttpGet("temperature-data-time-period/{smartDeviceId}/{totalTimePeriod}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<TemperatureDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetTemperatureByTimePeriod(Guid smartDeviceId, TotalTimePeriod totalTimePeriod)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetTemperatureTimePeriod(smartDeviceId, totalTimePeriod, userId.Value);
            return GetResult(result);
        }

        [HttpGet("temperature-data-from-to/{smartDeviceId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<TemperatureDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetTemperatureByFromTo(Guid smartDeviceId, [FromQuery] string from, [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetTemperatureFromTo(smartDeviceId, from, to, userId.Value);
            return GetResult(result);
        }



        [HttpGet("humidity-data/{smartDeviceId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<HumidityDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetHumidity(Guid smartDeviceId)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetHumidityData(smartDeviceId, userId.Value);
            return GetResult(result);
        }

        [HttpGet("humidity-data-time-period/{smartDeviceId}/{totalTimePeriod}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<HumidityDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetHumidityByTimePeriod(Guid smartDeviceId, TotalTimePeriod totalTimePeriod)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetHumidityTimePeriod(smartDeviceId, totalTimePeriod, userId.Value);
            return GetResult(result);
        }

        [HttpGet("humidity-data-from-to/{smartDeviceId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<HumidityDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetHumidityByFromTo(Guid smartDeviceId, [FromQuery] string from, [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var result = await _reportService.GetHumidityFromTo(smartDeviceId, from, to, userId.Value);
            return GetResult(result);
        }


        [HttpGet("vehicle-charger-history/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<VehicleChargerActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetVehicleChargerHistory(
            Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetVehicleChargerHistory(id, userId.Value);
            return GetResult(data);
        }
        [HttpGet("vehicle-charger-history-in-range/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<VehicleChargerActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetAllActionsByVehicleChargerInRange(
           Guid id, [FromQuery] string from, [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetAllActionsByVehicleChargerInRange(id, from, to, userId.Value, "");
            return GetResult(data);
        }
        [HttpGet("vehicle-charger-history-in-range-with-executer/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<VehicleChargerActionsDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetAllActionsByVehicleChargerInRangeWithExecuter(
           Guid id, [FromQuery] string from, [FromQuery] string to, [FromQuery] string executer)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetAllActionsByVehicleChargerInRange(id, from, to, userId.Value, executer);
            return GetResult(data);
        }

        [HttpGet("energy-consumed-city/{cityName}/{isConsumed}/{totalTimePeriod}")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(List<EnergyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEnergyConsumedByCityInTimePeriod(
           string cityName, bool isConsumed, TotalTimePeriod totalTimePeriod)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetEnergyConsumedProducedByCity(cityName, totalTimePeriod, userId.Value, isConsumed);
            return GetResult(data);
        }
        [HttpGet("energy-consumed-city-in-range/{cityName}/{isConsumed}")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(List<EnergyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEnergyConsumedByCityInRange(
          string cityName, bool isConsumed, [FromQuery] string from, [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetEnergyConsumedProducedByCityInRange(cityName, from, to, userId.Value, isConsumed);
            return GetResult(data);
        }

        [HttpGet("energy-consumed-property/{propertyId}/{isConsumed}/{totalTimePeriod}")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(List<EnergyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEnergyConsumedByPropertyInTimePeriod(
          Guid propertyId, bool isConsumed, TotalTimePeriod totalTimePeriod)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetEnergyConsumedProducedByProperty(propertyId, totalTimePeriod, userId.Value, isConsumed);
            return GetResult(data);
        }
        [HttpGet("energy-consumed-property-in-range/{propertyId}/{isConsumed}")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(List<EnergyConsumptionDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetEnergyConsumedByPropertyInRange(
          Guid propertyId, bool isConsumed, [FromQuery] string from, [FromQuery] string to)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var data = await _reportService.GetEnergyConsumedProducedByPropertyInRange(propertyId, from, to, userId.Value, isConsumed);
            return GetResult(data);
        }

    }
}
