using AgencyAppointmentSystem.Data.Entities;

namespace AgencyAppointmentSystem.Data.Repositories;

public interface IAppointmentRepository
{
    Task<int> GetAppointmentCountAsync(DateTime date);

    Task<Appointment?> GetLastAppointmentAsync(DateTime date);

    Task<List<Appointment>> GetAppointmentsByDateAsync(
        DateTime date);

    Task<Appointment?> GetByIdAsync(int id);

    Task AddAsync(Appointment appointment);

    Task<Customer> AddCustomerAsync(Customer customer);

    Task SaveChangesAsync();
}