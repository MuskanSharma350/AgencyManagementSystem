using AgencyAppointmentSystem.Data.Context;
using AgencyAppointmentSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencyAppointmentSystem.Data.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly AppDbContext _context;

    public HolidayRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsHolidayAsync(DateTime date)
    {
        return await _context.Holidays
            .AnyAsync(x => x.HolidayDate.Date == date.Date);
    }

    public async Task<List<Holiday>> GetAllAsync()
    {
        return await _context.Holidays
            .OrderBy(x => x.HolidayDate)
            .ToListAsync();
    }

    public async Task<Holiday> AddAsync(Holiday holiday)
    {
        await _context.Holidays.AddAsync(holiday);

        return holiday;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var holiday = await _context.Holidays
            .FirstOrDefaultAsync(x => x.Id == id);

        if (holiday == null)
            return false;

        _context.Holidays.Remove(holiday);

        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}