using Sofia.Web.DTO.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IAppointmentsService
{
    Task<BookAppointmentResult> BookAppointmentAsync(string userId, BookAppointmentRequest request);
}
