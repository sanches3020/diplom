using Sofia.Web.DTO.Psychologist;
using Sofia.Web.ViewModels.Psychologists;

namespace Sofia.Web.Services.Interfaces;

public interface IAppointmentsService
{
    Task<BookAppointmentResult> BookAppointmentAsync(string userId, BookAppointmentRequest request);
    Task<UserAppointmentsViewModel> GetUserAppointmentsAsync(string userId);
}
