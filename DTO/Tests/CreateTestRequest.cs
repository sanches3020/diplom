namespace Sofia.Web.DTO.Tests;

public class CreateTestRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();

    public class QuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new();
    }

    public class AnswerDto
    {
        public string Text { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
