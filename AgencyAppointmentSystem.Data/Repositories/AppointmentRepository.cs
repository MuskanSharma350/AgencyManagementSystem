using AgencyAppointmentSystem.Data.Context;
using AgencyAppointmentSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencyAppointmentSystem.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetAppointmentCountAsync(DateTime date)
    {
        return await _context.Appointments
            .Where(x =>
                x.AppointmentDate.Date == date.Date &&
                x.Status != "Cancelled")
            .CountAsync();
    }

    public async Task<Appointment?> GetLastAppointmentAsync(
        DateTime date)
    {
        return await _context.Appointments
            .Where(x =>
                x.AppointmentDate.Date == date.Date)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Appointment>>
        GetAppointmentsByDateAsync(DateTime date)
    {
        return await _context.Appointments
            .Include(x => x.Customer)
            .Where(x =>
                x.AppointmentDate.Date == date.Date)
            .OrderBy(x => x.AppointmentTime)
            .ThenBy(x => x.TokenNumber)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
    }

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);

        return customer;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}