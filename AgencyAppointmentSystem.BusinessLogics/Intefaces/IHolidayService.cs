using AgencyAppointmentSystem.Data.Entities;

namespace AgencyAppointmentSystem.Business.Interfaces;

public interface IHolidayService
{
    Task<List<Holiday>> GetAllAsync();

    Task<Holiday> AddAsync(Holiday holiday);

    Task<bool> DeleteAsync(int id);
}