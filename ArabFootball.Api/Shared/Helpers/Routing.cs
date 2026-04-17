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

        public static class Matches
        {
            public const string Prefix = Rule + "Matches";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
            public const string Add = Prefix;
            public const string Update = Prefix + "/{id:int}";
            public const string Delete = Prefix + "/{id:int}";

            public const string ChangeStatus = Prefix + "/{id:int}/status";
            public const string OpenPredictions = Prefix + "/{id:int}/open-predictions";
            public const string ClosePredictions = Prefix + "/{id:int}/close-predictions";
            public const string LinkChat = Prefix + "/{id:int}/link-chat";
            public const string UnlinkChat = Prefix + "/{id:int}/unlink-chat";

        }
        public static class Chats
        {
            public const string Prefix = Rule + "Chats";

            public const string GetAll = Prefix;
            public const string GetById = Prefix + "/{id:int}";
            public const string CreatePrivate = Prefix + "/{id:int}/create-private";
            public const string CreateGroup = Prefix + "/{id:int}/create-group";
            public const string CreateMatch = Prefix + "/{id:int}/create-match/{matchId}";
            public const string AddMember = Prefix + "/{id:int}/add-member";
            public const string RemoveMember = Prefix + "/{id:int}/remove-member";
        }

    }
}
