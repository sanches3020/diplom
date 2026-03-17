using System.ComponentModel.DataAnnotations;

namespace Sofia.Web.Models
{
    public class SaveEmotionRequest
    {
        // Дата, к которой относится эмоция
        [Required]
        public DateTime Date { get; set; }

        // Эмоция — строго enum
        [Required]
        public EmotionType Emotion { get; set; }

        // Дополнительная заметка
        [StringLength(1000)]
        public string? Note { get; set; }
    }
}
