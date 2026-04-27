using System.Net;
using System.Text.Json.Serialization;

namespace ArabFootball.Shared.Helpers
{
    public class ApiResponse<T>
    {

        public bool IsSuccess { get; set; }
        [JsonConverter(typeof(JsonNumberEnumConverter<HttpStatusCode>))]
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public List<ErrorDetails> Errors { get; set; }


        public static ApiResponse<T> Success(T data = default, string message = "")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                StatusCode = HttpStatusCode.OK,
            };
        }

        public static ApiResponse<T> Error(HttpStatusCode httpStatus, string message = "", List<ErrorDetails> errors = null)
        {
            return new ApiResponse<T>
            {
                StatusCode = httpStatus,
                Message = message,
                Errors = errors,
            };
        }
    }