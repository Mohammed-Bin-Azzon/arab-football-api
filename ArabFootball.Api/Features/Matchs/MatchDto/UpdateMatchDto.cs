using System.ComponentModel.DataAnnotations;

namespace ArabFootball.Api.Features.Matchs.MatchDto
{
    public class UpdateMatchDto
    {
        [Required(ErrorMessage = "اسم الفريق المضيف مطلوب.")]
        [MaxLength(100)]
        public string HomeTeam { get; set; } = null!;

        [Required(ErrorMessage = "اسم الفريق الضيف مطلوب.")]
        [MaxLength(100)]
        public string AwayTeam { get; set; } = null!;

        [Required(ErrorMessage = "اسم الدوري مطلوب.")]
        [MaxLength(100)]
        public string League { get; set; } = null!;

        [Required(ErrorMessage = "تاريخ المباراة مطلوب.")]
        public string MatchDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "الساعة مطلوبة.")]
        [Range(1, 12, ErrorMessage = "الساعة يجب أن تكون من 1 إلى 12 فقط.")]
        public int Hour { get; set; }

        [Required(ErrorMessage = "الدقيقة مطلوبة.")]
        [Range(0, 59, ErrorMessage = "الدقيقة يجب أن تكون من 0 إلى 59 فقط.")]
        public int Minute { get; set; }

        [Required(ErrorMessage = "الفترة مطلوبة: صباح أو مساء.")]
        public string Period { get; set; } = string.Empty;
    }
}