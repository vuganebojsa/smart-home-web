using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartHouse.Core.Model;
using System.Security.Claims;

namespace SmartHouse.Controllers
{

    public class BaseController : ControllerBase
    {
        public ObjectResult GetResult<T>(Result<T> result)
        {
            return result.IsSuccess ? Ok(result.ValueOrDefault) : GetErrorReturn(result);
        }

        private ObjectResult GetErrorReturn<T>(Result<T> result)
        {
            int statusCode = 500;
            var error = result.Errors[0];
            if (error.HasMetadataKey("StatusCode"))
                statusCode = (int)error.Metadata["StatusCode"];

            return statusCode switch
            {
                400 => BadRequest(result.Errors.ElementAt(0).Message),
                404 => NotFound(result.Errors.ElementAt(0).Message),
                _ => StatusCode(statusCode, result.Errors.ElementAt(0).Message),
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public Result<Guid> GetUserId()
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Result.Ok(userId);

            }
            return Result.Fail("Invalid JWT token.");
        }
        protected void SetMetadata<T>(PagedList<T> response)
        {
            var metadata = new
            {
                response.TotalCount,
                response.PageSize,
                response.CurrentPage,
                response.TotalPages,
                response.HasNext,
                response.HasPrevious
            };
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        }
    }
}
