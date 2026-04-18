using Sofia.Web.ViewModels.Psychologists;

namespace Sofia.Web.Services.Interfaces
{
    public interface IClientResultsService
    {
        /// <summary>
        /// Получение результатов тестов клиента для просмотра психологом.
        /// </summary>
        /// <param name="psychologistUserId">ID психолога (Identity UserId)</param>
        /// <param name="clientId">ID клиента (int)</param>
        /// <returns>ViewModel с результатами или null, если доступа нет</returns>
        Task<ClientResultsViewModel?> GetClientResultsAsync(string psychologistUserId, int clientId);

        /// <summary>
        /// Генерация CSV-файла с результатами клиента.
        /// </summary>
        /// <param name="psychologistUserId">ID психолога (Identity UserId)</param>
        /// <param name="clientId">ID клиента (int)</param>
        /// <returns>DTO с байтами файла, именем и статусом</returns>
        Task<ClientResultsCsvResponse> GetClientResultsCsvAsync(string psychologistUserId, int clientId);
    }
}
