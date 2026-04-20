namespace Sofia.Web.DTO.Companion
{
    public class CompanionLevelInfoResponse
    {
        /// <summary>
        /// Текущий уровень компаньена (1-5)
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Количество дней в системе
        /// </summary>
        public int DaysInSystem { get; set; }

        /// <summary>
        /// Количество дней до следующего уровня
        /// </summary>
        public int DaysToNextLevel { get; set; }

        /// <summary>
        /// Процент прогресса к следующему уровню (0-100)
        /// </summary>
        public int ProgressPercent { get; set; }

        /// <summary>
        /// Является ли это максимальный уровень
        /// </summary>
        public bool MaxLevel { get; set; }
    }
}
