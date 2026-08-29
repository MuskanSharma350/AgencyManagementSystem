namespace AgencyAppointmentSystem.Data.Repositories;

public interface IAgencySettingsRepository
{    Task<int> GetMaxAppointmentsPerDayAsync();

    Task SetMaxAppointmentsPerDayAsync(int maxAppointments);
}