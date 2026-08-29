using AgencyAppointmentSystem.Data.Entities;

namespace AgencyAppointmentSystem.Data.Repositories;

public interface IHolidayRepository
{
    Task<bool> IsHolidayAsync(DateTime date);

    Task<List<Holiday>> GetAllAsync();

    Task<Holiday> AddAsync(Holiday holiday);

    Task<bool> DeleteAsync(int id);

    Task SaveChangesAsync();
}