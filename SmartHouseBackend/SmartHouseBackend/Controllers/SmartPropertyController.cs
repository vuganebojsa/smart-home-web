using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHouse.Core.Interfaces.Services;
using SmartHouse.Core.Model;
using SmartHouse.Infrastructure.DTOS;

namespace SmartHouse.Controllers
{
    [Route("api/v1/smartProperties")]
    [ApiController]
    public class SmartPropertyController : BaseController
    {
        private readonly ISmartPropertyService _smartPropertyService;
        public SmartPropertyController(ISmartPropertyService smartPropertyService)
        {
            _smartPropertyService = smartPropertyService;

        }

        [HttpPost("register")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(PropertyDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> RegisterProperty([FromBody] RegisterPropertyDTO propertyInfo)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var property = await _smartPropertyService.RegisterSmartProperty(propertyInfo, userId.Value);
            return GetResult(property);

        }

        [HttpGet("GetCities")]
        [ProducesResponseType(typeof(List<CityDTO>), 200)]
        public async Task<IActionResult> GetAllCities()
        {
            var properties = await _smartPropertyService.GetAllCities();
            return GetResult(properties);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SinglePropertyDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetProperty(Guid id)
        {
            var property = await _smartPropertyService.GetProperty(id);
            return GetResult(property);
        }

        [HttpGet("get-properties")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(PagedList<AdminProperties>), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetPropertiesForAdmin([FromQuery] Page page)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);

            var properties = await _smartPropertyService.GetPropertiesForAdmin(userId.Value, page);
            if (properties.IsSuccess)
            {
                SetMetadata(properties.Value);
            }
            return GetResult(properties);

        }
    }
}
