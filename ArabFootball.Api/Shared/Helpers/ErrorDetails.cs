namespace ArabFootball.Shared.Helpers
{
    public class ErrorDetails
    {
        public ErrorDetails(string field, string errorMessage)
        {
            Field = field;
            Message = errorMessage;
        }

        public ErrorDetails()
        {
        }

        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}