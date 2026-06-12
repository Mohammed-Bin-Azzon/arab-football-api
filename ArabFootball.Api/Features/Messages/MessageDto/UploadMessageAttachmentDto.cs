using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Messages.MessageDto
{
    public class UploadMessageAttachmentDto
    {
        [Required(ErrorMessage = "يجب إرفاق صورة أو فيديو")]
        public IFormFile File { get; set; } = null!;
    }
}
