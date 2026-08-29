using AgencyAppointmentSystem.Business.DTOs;

namespace AgencyAppointmentSystem.Business.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponse> BookAppointmentAsync(
        BookAppointmentRequest request);

    Task<List<AppointmentResponse>> GetAppointmentsByDateAsync(
        DateTime date);

    Task<bool> CompleteAppointmentAsync(int id);

    Task<bool> CancelAppointmentAsync(int id);
}