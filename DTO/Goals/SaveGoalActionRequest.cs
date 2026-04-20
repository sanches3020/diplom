namespace Sofia.Web.DTO.Goals
{
    public class SaveGoalActionRequest
    {
        /// <summary>
        /// ID цели
        /// </summary>
        public int GoalId { get; set; }

        /// <summary>
        /// Что сделано (текст действия)
        /// </summary>
        public string ActionText { get; set; } = string.Empty;

        /// <summary>
        /// Какой результат получен (текст результата)
        /// </summary>
        public string ResultText { get; set; } = string.Empty;
    }
}
