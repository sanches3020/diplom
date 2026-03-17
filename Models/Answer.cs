using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models;

public class Answer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public virtual Question Question { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Numeric value used in scoring
    /// </summary>
    [Range(-100, 100)]
    public int Value { get; set; } = 0;

    /// <summary>
    /// Order of answer in UI
    /// </summary>
    [Range(0, 100)]
    public int Order { get; set; } = 0;
}
