namespace ArabFootball.Shared.Helpers
{
    public class Routing
    {
        public const string Root = "api";
        public const string Rule = Root + "/";
        
        

        public static class UserAuth
        {
            public const string Prefix = Rule + "auth";

            public const string Login = Prefix + "/login";                       
            public const string Register = Prefix + "/register";
        }
    }
}
