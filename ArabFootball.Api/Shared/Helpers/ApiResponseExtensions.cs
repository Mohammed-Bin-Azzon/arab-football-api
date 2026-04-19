using System.Net;
using ArabFootball.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ArabFootball.Api.Shared.Helpers
{
    public static class ApiResponseExtensions
    {
        public static IActionResult ToActionResult<T>(this ControllerBase controller, ApiResponse<T> response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.OK => controller.Ok(response),
                HttpStatusCode.Created => controller.StatusCode(StatusCodes.Status201Created, response),
                HttpStatusCode.BadRequest => controller.BadRequest(response),
                HttpStatusCode.Unauthorized => controller.Unauthorized(response),
                HttpStatusCode.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, response),
                HttpStatusCode.NotFound => controller.NotFound(response),
                HttpStatusCode.Conflict => controller.Conflict(response),
                HttpStatusCode.UnprocessableEntity => controller.UnprocessableEntity(response),
                _ => controller.StatusCode((int)response.StatusCode, response)
            };
        }

        public static IActionResult ValidationProblemResponse(
            this ControllerBase controller,
            string message = "فشل التحقق من صحة البيانات.")
        {
            var errors = controller.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e =>
                    new ErrorDetails(x.Key, string.IsNullOrWhiteSpace(e.ErrorMessage) ? "قيمة غير صحيحة." : e.ErrorMessage)))
                .ToList();

            var response = ApiResponse<object>.Validation(message, errors);
            return controller.BadRequest(response);
        }
    }
}