using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces
{
    public interface IClientResultsService
    {
        /// <summary>
        /// Получение результатов тестов клиента для просмотра психологом.
        /// </summary>
        /// <param name="psychologistUserId">ID психолога (Identity UserId)</param>
        /// <param name="clientUserId">ID клиента (string)</param>
        /// <returns>ViewModel с результатами или null, если доступа нет</returns>
        Task<ClientResultsViewModel?> GetClientResultsAsync(string psychologistUserId, string clientUserId);

        /// <summary>
        /// Генерация CSV-файла с результатами клиента.
        /// </summary>
        /// <param name="psychologistUserId">ID психолога (Identity UserId)</param>
        /// <param name="clientUserId">ID клиента (string)</param>
        /// <returns>DTO с байтами файла, именем и статусом</returns>
        Task<ClientResultsCsvResponse> GetClientResultsCsvAsync(string psychologistUserId, string clientUserId);
    }
}
