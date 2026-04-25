using System.Net;
using System.Text.Json.Serialization;

namespace ArabFootball.Shared.Helpers
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        [JsonConverter(typeof(JsonNumberEnumConverter<HttpStatusCode>))]
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ErrorDetails> Errors { get; set; } = new();

        public static ApiResponse<T> Success(T? data = default, string message = "")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Data = data,
                Errors = new()
            };
        }

        public static ApiResponse<T> Created(T? data = default, string message = "")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Message = message,
                Data = data,
                Errors = new()
            };
        }

        public static ApiResponse<T> Fail(
            HttpStatusCode statusCode,
            string message = "",
            List<ErrorDetails>? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors ?? new()
            };
        }

        public static ApiResponse<T> Validation(string message, List<ErrorDetails> errors)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = message,
                Data = default,
                Errors = errors ?? new()
            };
        }

        public static ApiResponse<T> NotFound(string message = "العنصر غير موجود.")
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = message,
                Data = default,
                Errors = new()
            };
        }

        public static ApiResponse<T> Unauthorized(string message = "غير مصرح لك.")
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.Unauthorized,
                Message = message,
                Data = default,
                Errors = new()
            };
        }

        public static ApiResponse<T> Forbidden(string message = "ليس لديك صلاحية لتنفيذ هذا الإجراء.")
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.Forbidden,
                Message = message,
                Data = default,
                Errors = new()
            };
        }
    }
}