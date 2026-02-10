using ArabFootball.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace api_training.Controllers
{
  
    public class AppControllerBase : ControllerBase
    {
        public new ObjectResult Response<T>(ApiResponse<T> response)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(response),
                HttpStatusCode.Created => new CreatedResult(string.Empty, response),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                HttpStatusCode.Conflict => new ConflictObjectResult(response),
                HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                _ => new BadRequestObjectResult(response),
            };


        }
    }
}
