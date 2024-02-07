using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartHouse.Filters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var firstError = context.ModelState.Values
                .SelectMany(entry => entry.Errors)
                .FirstOrDefault();

                if (firstError != null)
                {
                    string errorMessage = firstError.ErrorMessage;
                    context.Result = new BadRequestObjectResult(errorMessage);
                }
                else
                {
                    context.Result = new BadRequestObjectResult("Invalid request");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
