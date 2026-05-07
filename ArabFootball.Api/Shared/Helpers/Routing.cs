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
       
        public static class ChatMembers
        {
            public const string Prefix = Rule + "ChatMembers";

            public const string GetAll = Prefix;
            public const string MuteMember = Prefix + "/{id:int}/mute-member";
            public const string UnmuteMember = Prefix + "/{id:int}/unmute-member";
            public const string MakeModerator = Prefix + "/{id:int}/make-moderator";
            public const string RevokeModerator = Prefix + "/{id:int}/revoke-moderator";
            public const string JoinChat = Prefix + "/{id:int}/join-chat";
            public const string LeaveChat = Prefix + "/{id:int}/leave-chat";
        }

        
    }
}
