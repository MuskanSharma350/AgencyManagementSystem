using AgencyAppointmentSystem.Business.DTOs;
using AgencyAppointmentSystem.Business.Interfaces;
using AgencyAppointmentSystem.Data.Entities;
using AgencyAppointmentSystem.Data.Repositories;

namespace AgencyAppointmentSystem.Business.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IHolidayRepository _holidayRepository;
    private readonly IAgencySettingsRepository _settingsRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IHolidayRepository holidayRepository,
        IAgencySettingsRepository settingsRepository)
    {
        _appointmentRepository = appointmentRepository;
        _holidayRepository = holidayRepository;
        _settingsRepository = settingsRepository;
    }

    public async Task<AppointmentResponse> BookAppointmentAsync(
        BookAppointmentRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Customer name is required.");

        if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            throw new ArgumentException("Customer phone is required.");

        var appointmentDate = request.PreferredDate.Date;

        if (appointmentDate < DateTime.Today)
            throw new ArgumentException(
                "Appointment date cannot be in the past.");

        var maxAppointments =
            await _settingsRepository.GetMaxAppointmentsPerDayAsync();

        if (maxAppointments <= 0)
            throw new InvalidOperationException(
                "Maximum appointments per day is not configured.");

        // Find next available working day
        while (true)
        {
            var isHoliday =
                await _holidayRepository.IsHolidayAsync(appointmentDate);

            var appointmentCount =
                await _appointmentRepository
                    .GetAppointmentCountAsync(appointmentDate);

            if (!isHoliday && appointmentCount < maxAppointments)
                break;

            appointmentDate = appointmentDate.AddDays(1);
        }

        // Get last token for selected date
        var lastAppointment =
            await _appointmentRepository
                .GetLastAppointmentAsync(appointmentDate);

        var nextTokenNumber = 1;

        if (lastAppointment != null)
        {
            var token = lastAppointment.TokenNumber;

            if (token.StartsWith("T") &&
                int.TryParse(token[1..], out var lastNumber))
            {
                nextTokenNumber = lastNumber + 1;
            }
        }

        var tokenNumber = $"T{nextTokenNumber:D3}";

        // Create customer
        var customer = new Customer
        {
            Name = request.CustomerName.Trim(),
            Phone = request.CustomerPhone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.CustomerEmail)
                ? null
                : request.CustomerEmail.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _appointmentRepository.AddCustomerAsync(customer);

        // Appointment time:
        // First token = 10:00
        // Each customer gets a 15 minute slot
        var appointmentTime =
            new TimeSpan(10, 0, 0)
                .Add(TimeSpan.FromMinutes(
                    (nextTokenNumber - 1) * 15));

        var appointment = new Appointment
        {
            Customer = customer,
            CustomerId = customer.Id,
            TokenNumber = tokenNumber,
            AppointmentDate = appointmentDate,
            AppointmentTime = appointmentTime,
            Status = "Waiting",
            CreatedAt = DateTime.UtcNow
        };

        await _appointmentRepository.AddAsync(appointment);

        await _appointmentRepository.SaveChangesAsync();

        return MapToResponse(appointment);
    }

    public async Task<List<AppointmentResponse>>
        GetAppointmentsByDateAsync(DateTime date)
    {
        var appointments =
            await _appointmentRepository
                .GetAppointmentsByDateAsync(date.Date);

        return appointments
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<bool> CompleteAppointmentAsync(int id)
    {
        var appointment =
            await _appointmentRepository.GetByIdAsync(id);

        if (appointment == null)
            return false;

        if (appointment.Status == "Cancelled")
            throw new InvalidOperationException(
                "Cancelled appointment cannot be completed.");

        appointment.Status = "Completed";

        await _appointmentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelAppointmentAsync(int id)
    {
        var appointment =
            await _appointmentRepository.GetByIdAsync(id);

        if (appointment == null)
            return false;

        if (appointment.Status == "Completed")
            throw new InvalidOperationException(
                "Completed appointment cannot be cancelled.");

        appointment.Status = "Cancelled";

        await _appointmentRepository.SaveChangesAsync();

        return true;
    }

    private static AppointmentResponse MapToResponse(
        Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            CustomerId = appointment.CustomerId,
            TokenNumber = appointment.TokenNumber,

            CustomerName = appointment.Customer?.Name ?? string.Empty,
            CustomerPhone = appointment.Customer?.Phone ?? string.Empty,
            CustomerEmail = appointment.Customer?.Email,

            AppointmentDate = appointment.AppointmentDate,
            AppointmentTime = appointment.AppointmentTime,
            Status = appointment.Status
        };
    }
}