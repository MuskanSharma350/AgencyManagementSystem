using AgencyAppointmentSystem.Business.Interfaces;
using AgencyAppointmentSystem.Data.Entities;
using AgencyAppointmentSystem.Data.Repositories;

namespace AgencyAppointmentSystem.Business.Services;

public class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _repository;

    public HolidayService(IHolidayRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Holiday>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Holiday> AddAsync(Holiday holiday)
    {
        if (holiday == null)
            throw new ArgumentNullException(nameof(holiday));

        var holidayDate = holiday.HolidayDate.Date;

        if (holidayDate < DateTime.Today)
            throw new ArgumentException(
                "Holiday date cannot be in the past.");

        var exists =
            await _repository.IsHolidayAsync(holidayDate);

        if (exists)
            throw new ArgumentException(
                "Holiday already exists.");

        holiday.HolidayDate = holidayDate;

        if (string.IsNullOrWhiteSpace(holiday.Description))
            holiday.Description = "Public Holiday";

        var result =
            await _repository.AddAsync(holiday);

        await _repository.SaveChangesAsync();

        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result =
            await _repository.DeleteAsync(id);

        if (result)
            await _repository.SaveChangesAsync();

        return result;
    }
}