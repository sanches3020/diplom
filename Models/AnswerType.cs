namespace Sofia.Web.Models
{
    public enum AnswerType
    {
        SingleChoice = 0,   // один правильный ответ
        MultipleChoice = 1, // несколько правильных ответов
        Text = 2,           // открытый текстовый ответ
        Scale = 3           // шкала (например, 1–10)
    }
}
