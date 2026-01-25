namespace api_training.Shared.Helpers
{
    public class ErrorDetails
    {

        public ErrorDetails(string field,string ErrorMessage)
        {
            Field=field;
            Message=ErrorMessage;


        }
        public ErrorDetails()
        {
            
        }
        public string Field { get; set; }
        public string Message { get; set; }

    }
}
