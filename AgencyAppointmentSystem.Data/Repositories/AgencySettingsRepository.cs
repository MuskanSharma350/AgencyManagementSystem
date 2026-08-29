using AgencyAppointmentSystem.Data.Context;
using AgencyAppointmentSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencyAppointmentSystem.Data.Repositories;

public class AgencySettingsRepository
    : IAgencySettingsRepository
{
    private readonly AppDbContext _context;

    public AgencySettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetMaxAppointmentsPerDayAsync()
    {
        var settings =
            await _context.AgencySettings
                .FirstOrDefaultAsync();

        if (settings == null)
        {
            return 50;
        }

        return settings.MaxAppointmentsPerDay;
    }

    public async Task SetMaxAppointmentsPerDayAsync(
        int maxAppointments)
    {
        if (maxAppointments <= 0)
            throw new ArgumentException(
                "Maximum appointments must be greater than zero.");

        var settings =
            await _context.AgencySettings
                .FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new AgencySettings
            {
                MaxAppointmentsPerDay = maxAppointments,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.AgencySettings
                .AddAsync(settings);
        }
        else
        {
            settings.MaxAppointmentsPerDay =
                maxAppointments;

            settings.UpdatedAt =
                DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}