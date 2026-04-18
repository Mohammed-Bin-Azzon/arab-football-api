namespace ArabFootball.Api.Features.Chats.ChatDto
{
    public class CreateGroupChatDto
    {
        public string Title { get; set; }
        public List<int> MemberIds { get; set; }
    }
}
