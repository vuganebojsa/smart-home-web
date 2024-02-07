using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHouse.Core.Interfaces.Services;
using SmartHouse.Core.Model;
using SmartHouse.DTOS;
using SmartHouse.Infrastructure.DTOS;
using System.Security.Claims;

namespace SmartHouse.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TokenDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginInfo)
        {
            var token = await _userService.Login(loginInfo.Email, loginInfo.Password);
            return GetResult(token);
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO userInfo)
        {
            var user = await _userService.Register(userInfo);
            return GetResult(user);
        }
        [HttpPost("register-admin")]
        [Authorize(Roles = "SUPERADMIN")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDTO userInfo)
        {
            var user = await _userService.RegisterAdmin(userInfo);
            return GetResult(user);

        }

        [HttpGet("activate")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> ActivateUser([FromQuery] string userEmail)
        {
            var confirmation = await _userService.ActivateUser(userEmail);
            return GetResult(confirmation);
        }
        [HttpGet("GetUserProperties")]
        [Authorize(Roles = "USER")]
        [ProducesResponseType(typeof(PagedList<UserPropertyDTO>), 200)]
        public async Task<IActionResult> ShowAllUserProperties([FromQuery] Page page)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                var properties = await _userService.GetUserProperties(userId, page);
                if (properties.IsSuccess)
                {
                    SetMetadata(properties.Value);
                }
                return GetResult(properties);

            }
            return BadRequest();

        }

        [HttpGet("GetPendingProperties")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]

        public async Task<IActionResult> ShowPendingProperties([FromQuery] Page page)
        {
            var properties = await _userService.GetPendingProperties(page);
            if (properties.IsSuccess)
            {
                SetMetadata(properties.Value);
            }
            return GetResult(properties);
        }

        [HttpPost("{id}/ProcessRequest")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ProccesPendingPropertyRequest(Guid id, [FromBody] ProcessPropertyRequestDTO request)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid adminId))
            {
                var result = await _userService.ProccesPendingPropertyRequest(id, request);
                return GetResult(result);
            }
            return BadRequest();
        }

        [HttpGet("profile")]
        [Authorize(Roles = "ADMIN, SUPERADMIN, USER")]
        [ProducesResponseType(typeof(UserProfileDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var profile = await _userService.GetUserProfile(userId.Value);
            return GetResult(profile);
        }

        [HttpPut("edit-profile")]
        [Authorize(Roles = "ADMIN, SUPERADMIN, USER")]
        [ProducesResponseType(typeof(EditUserDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> EditProfile([FromBody] EditUserDTO user)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var profile = await _userService.EditProfile(user, userId.Value);
            return GetResult(profile);
        }

        [HttpPut("edit-password")]
        [Authorize(Roles = "ADMIN, SUPERADMIN, USER")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> EditPassword([FromBody] EditPasswordDTO passwordInfo)
        {
            var userId = GetUserId();
            if (userId.IsFailed) return BadRequest(userId.Errors.ElementAt(0).Message);
            var profile = await _userService.EditPassword(passwordInfo, userId.Value);
            return GetResult(profile);
        }

        [HttpPut("edit-first-password")]
        [ProducesResponseType(typeof(TokenDTO), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 400)]

        public async Task<IActionResult> EditFirstPassword([FromQuery] string email, [FromBody] EditPasswordDTO passwordInfo)
        {
            var profile = await _userService.EditFirstPassword(passwordInfo, email);
            return GetResult(profile);
        }

    }

}


