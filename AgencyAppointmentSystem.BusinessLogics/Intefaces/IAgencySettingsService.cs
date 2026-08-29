namespace AgencyAppointmentSystem.Business.Interfaces;

public interface IAgencySettingsService
{
    Task<int> GetMaxAppointmentsPerDayAsync();

    Task SetMaxAppointmentsPerDayAsync(int maxAppointments);
}