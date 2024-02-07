using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.Interfaces.Services;

namespace SmartHouse.Controllers
{
    [Route("api/v1/smartDevices")]
    [ApiController]
    public class SmartDeviceController : BaseController
    {
        private readonly ISmartDeviceService _smartDeviceService;

        public SmartDeviceController(ISmartDeviceService smartDeviceService)
        {
            _smartDeviceService = smartDeviceService;
        }

        [HttpPost("register-lamp")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterLamp([FromBody] RegisterLampDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterLamp(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-ambient-sensor")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterAmbientSensor([FromBody] RegisterDeviceDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterAmbientSensor(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-sprinkler-system")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterSprinklerSystem([FromBody] RegisterDeviceDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterSprinklerSystem(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-air-conditioner")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterAirConditioner([FromBody] RegisterAirConditionerDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterAirConditioner(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-washing-machine")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterWashingMachine([FromBody] RegisterWashingMachineDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterWashingMachine(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-vehicle-gate")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterVehicleGate([FromBody] RegisterDeviceDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterVehicleGate(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-house-battery")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterHouseBattery([FromBody] RegisterHouseBatteryDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterHouseBattery(device, userId.Value);
            return GetResult(property);

        }
        [HttpPost("register-electric-vehicle-charger")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterElectricVehicleCharger([FromBody] RegisterElectricChargerDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterElectricVehicleCharger(device, userId.Value);
            return GetResult(property);

        }

        [HttpPost("register-solar-panel-system")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SmartDeviceDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterSolarPanelSystem([FromBody] RegisterSolarPanelSystemDTO device)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartDeviceService.RegisterSolarPanelSystem(device, userId.Value);
            return GetResult(property);

        }
        [HttpGet]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<SmartDeviceDTO>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetDevices()
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            List<SmartDeviceDTO> devices = await _smartDeviceService.GetDevices(userId.Value);
            return GetResult<List<SmartDeviceDTO>>(devices);

        }
        [HttpGet("{propertyId}/devices")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(PagedList<SmartDeviceDTO>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetDevicesByProperty(Guid propertyId, [FromQuery] Page page)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var devices = await _smartDeviceService.GetDevicesByPropertyId(propertyId, userId.Value, page);
            if (devices.IsSuccess)
            {
                SetMetadata(devices.Value);
            }
            return GetResult(devices);

        }

        [HttpGet("washing-machine-cycles")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<Cycle>), 200)]
        public async Task<IActionResult> GetWashingMachineCycles()
        {

            var cycles = await _smartDeviceService.GetWashingMachineCycles();
            return GetResult(cycles);

        }

        [HttpPut("turn-on-off/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> TurnOnOffDevice(Guid id, [FromBody] TurnOnOffDTO turnOnOff)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var status = await _smartDeviceService.TurnOnOff(id, turnOnOff.IsOn, userId.Value);
            return GetResult(status);

        }

        [HttpPut("turn-gate-public-private/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> TurnGatePublicPrivate(Guid id, [FromBody] GatePublicPrivateDTO turnPublicPrivate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var status = await _smartDeviceService.TurnGatePublicPrivate(id, turnPublicPrivate.IsPublic, userId.Value);
            return GetResult(status);

        }

        [HttpGet("n-devices")]
        [ProducesResponseType(typeof(List<SmartDeviceForPingDTO>), 200)]
        public async Task<IActionResult> GetTotalDevices([FromQuery] int totalDevices)
        {

            var devices = await _smartDeviceService.GetNDevices(totalDevices);
            return GetResult(devices);

        }
        [HttpGet("n-devices-type")]
        [ProducesResponseType(typeof(List<SmartDeviceForPingDTO>), 200)]
        public async Task<IActionResult> GetTotalDevicesByType([FromQuery] int totalDevices, [FromQuery] SmartDeviceType type)
        {

            Result<List<SmartDeviceForPingDTO>> devices = await _smartDeviceService.GetNDevicesWithType(totalDevices, type);
            return GetResult(devices);

        }
        [HttpGet("battery/{batteryId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(BatteryDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetBatteryById(Guid batteryId)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var status = await _smartDeviceService.GetBattery(batteryId, userId.Value);
            return GetResult(status);

        }


        [HttpGet("gate-info/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(GateInfoDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetGateInfo(Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var gateInfo = await _smartDeviceService.GetGateInfo(id, userId.Value);
            return GetResult(gateInfo);
        }

        [HttpGet("sprinkler-info/{id}")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> GetSprinklerInfo(Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var sprinklerInfo = await _smartDeviceService.GetSprinklerInfo(id, userId.Value);
            return GetResult(sprinklerInfo);
        }

        [HttpGet("licence-plates/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetLicencePlates(Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var licencePlates = await _smartDeviceService.GetLicencePlates(id, userId.Value);
            return GetResult(licencePlates);
        }

        [HttpPut("register-licence-plate/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(LicencePlateDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> RegisterLicencePlate(Guid id, [FromBody] LicencePlateDTO licencePlate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var licencePlateReturned = await _smartDeviceService.RegisterLicencePlate(id, licencePlate, userId.Value);
            return GetResult(licencePlateReturned);

        }

        [HttpPut("remove-licence-plate/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> RemoveLicencePlate(Guid id, [FromBody] LicencePlateDTO licencePlate)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var licencePlateRemoved = await _smartDeviceService.RemoveLicencePlate(id, licencePlate, userId.Value);
            return GetResult(licencePlateRemoved);

        }
        [HttpGet("solar-panel-system/{spsId}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(SpSDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetSpsById(Guid spsId)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var status = await _smartDeviceService.GetSps(spsId, userId.Value);
            return GetResult(status);
        }

        [HttpPut("vehicle-charge/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SetPercentageOfCharge(Guid id, [FromBody] PercentageOfChargeDTO percentageOfCharge)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var newCharge = await _smartDeviceService.SetPercentageOfCharge(id, percentageOfCharge.PercentageOfCharge, userId.Value);
            return GetResult(newCharge);

        }

        [HttpGet("vehicle-charger/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(VehicleChargerDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetVehicleCharger(Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var status = await _smartDeviceService.GetVehicleCharger(id, userId.Value);
            return GetResult(status);

        }
        [HttpGet("active-days/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetActiveDays(Guid id)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var activeDays = await _smartDeviceService.GetActiveDays(id, userId.Value);
            return GetResult(activeDays);
        }

        [HttpPut("add-sprinkler-day/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> AddSprinklerDay(Guid id, [FromBody] DayDTO day)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var dayReturned = await _smartDeviceService.AddSprinklerDay(id, day, userId.Value);
            return GetResult(dayReturned);

        }

        [HttpPut("remove-sprinkler-day/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> RemoveSprinklerDay(Guid id, [FromBody] DayDTO day)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var dayRemoved = await _smartDeviceService.RemoveSprinklerDay(id, day, userId.Value);
            return GetResult(dayRemoved);

        }

        [HttpPut("change-sprinkle-special-state/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ChangeSprinkleSpecialState(Guid id, [FromBody] SprinklerSpecialStateDTO changeSpecialState)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var state = await _smartDeviceService.ChangeSprinklerSpecialState(id, changeSpecialState.IsActive, userId.Value);
            return GetResult(state);

        }

        [HttpPut("change-sprinkler-start-time/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ChangeSprinklerStartTime(Guid id, [FromBody] SprinklerTimeDTO startTime)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var state = await _smartDeviceService.ChangeSprinklerStartTime(id, startTime, userId.Value);
            return GetResult(state);

        }

        [HttpPut("change-sprinkler-end-time/{id}")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ChangeSprinklerEndTime(Guid id, [FromBody] SprinklerTimeDTO endTime)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var state = await _smartDeviceService.ChangeSprinklerEndTime(id, endTime, userId.Value);
            return GetResult(state);

        }

    }
}
