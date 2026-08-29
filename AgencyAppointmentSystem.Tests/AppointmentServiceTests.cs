using AgencyAppointmentSystem.Business.DTOs;
using AgencyAppointmentSystem.Business.Services;
using AgencyAppointmentSystem.Data.Entities;
using AgencyAppointmentSystem.Data.Repositories;
using Moq;
using Xunit;

namespace AgencyAppointmentSystem.Tests.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepo;
    private readonly Mock<IHolidayRepository> _holidayRepo;
    private readonly Mock<IAgencySettingsRepository> _settingsRepo;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _appointmentRepo = new Mock<IAppointmentRepository>();
        _holidayRepo = new Mock<IHolidayRepository>();
        _settingsRepo = new Mock<IAgencySettingsRepository>();

        _service = new AppointmentService(
            _appointmentRepo.Object,
            _holidayRepo.Object,
            _settingsRepo.Object);
    }

    private static Customer Customer(string name, string phone) =>
        new() { Name = name, Phone = phone };

    [Fact]
    public async Task BookAppointment_ShouldCreateAppointment_WhenDateIsAvailable()
    {
        var date = DateTime.Today.AddDays(1);
        var request = new BookAppointmentRequest
        {
            CustomerName = "Rahul Sharma",
            CustomerPhone = "9876543210",
            PreferredDate = date
        };

        _holidayRepo.Setup(x => x.IsHolidayAsync(date)).ReturnsAsync(false);
        _appointmentRepo.Setup(x => x.GetAppointmentCountAsync(date)).ReturnsAsync(5);
        _settingsRepo.Setup(x => x.GetMaxAppointmentsPerDayAsync()).ReturnsAsync(50);
        _appointmentRepo.Setup(x => x.GetLastAppointmentAsync(date))
            .ReturnsAsync(new Appointment
            {
                Id = 5,
                TokenNumber = "T005",
                AppointmentDate = date
            });

        var result = await _service.BookAppointmentAsync(request);

        Assert.NotNull(result);
        Assert.Equal("T006", result.TokenNumber);
        Assert.Equal("Rahul Sharma", result.CustomerName);
        Assert.Equal("9876543210", result.CustomerPhone);
        Assert.Equal(date.Date, result.AppointmentDate.Date);
        Assert.Equal("Waiting", result.Status);

        _appointmentRepo.Verify(
            x => x.AddCustomerAsync(It.Is<Customer>(c =>
                c.Name == "Rahul Sharma" && c.Phone == "9876543210")),
            Times.Once);

        _appointmentRepo.Verify(
            x => x.AddAsync(It.Is<Appointment>(a =>
                a.TokenNumber == "T006" &&
                a.AppointmentDate == date.Date &&
                a.AppointmentTime == new TimeSpan(11, 15, 0) &&
                a.Status == "Waiting")),
            Times.Once);

        _appointmentRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BookAppointment_ShouldIssueFirstToken_WhenNoAppointmentsExist()
    {
        var date = DateTime.Today.AddDays(1);
        var request = new BookAppointmentRequest
        {
            CustomerName = "Sara",
            CustomerPhone = "9000000000",
            PreferredDate = date
        };

        _holidayRepo.Setup(x => x.IsHolidayAsync(date)).ReturnsAsync(false);
        _appointmentRepo.Setup(x => x.GetAppointmentCountAsync(date)).ReturnsAsync(0);
        _settingsRepo.Setup(x => x.GetMaxAppointmentsPerDayAsync()).ReturnsAsync(50);
        _appointmentRepo.Setup(x => x.GetLastAppointmentAsync(date))
            .ReturnsAsync((Appointment?)null);

        var result = await _service.BookAppointmentAsync(request);

        Assert.Equal("T001", result.TokenNumber);
        Assert.Equal(new TimeSpan(10, 0, 0), result.AppointmentTime);
    }

    [Fact]
    public async Task BookAppointment_ShouldMoveToNextWorkingDay_WhenPreferredDateIsHoliday()
    {
        var preferred = DateTime.Today.AddDays(1);
        var next = preferred.AddDays(1);
        var request = new BookAppointmentRequest
        {
            CustomerName = "Priya",
            CustomerPhone = "9876543211",
            PreferredDate = preferred
        };

        _holidayRepo.Setup(x => x.IsHolidayAsync(preferred)).ReturnsAsync(true);
        _holidayRepo.Setup(x => x.IsHolidayAsync(next)).ReturnsAsync(false);
        _appointmentRepo.Setup(x => x.GetAppointmentCountAsync(next)).ReturnsAsync(2);
        _settingsRepo.Setup(x => x.GetMaxAppointmentsPerDayAsync()).ReturnsAsync(50);
        _appointmentRepo.Setup(x => x.GetLastAppointmentAsync(next))
            .ReturnsAsync(new Appointment
            {
                Id = 2,
                TokenNumber = "T002",
                AppointmentDate = next
            });

        var result = await _service.BookAppointmentAsync(request);

        Assert.Equal(next.Date, result.AppointmentDate.Date);
        Assert.Equal("T003", result.TokenNumber);
    }

    [Fact]
    public async Task BookAppointment_ShouldMoveToNextDay_WhenDailyLimitReached()
    {
        var preferred = DateTime.Today.AddDays(1);
        var next = preferred.AddDays(1);
        var request = new BookAppointmentRequest
        {
            CustomerName = "Amit",
            CustomerPhone = "9876543212",
            PreferredDate = preferred
        };

        _holidayRepo.Setup(x => x.IsHolidayAsync(preferred)).ReturnsAsync(false);
        _holidayRepo.Setup(x => x.IsHolidayAsync(next)).ReturnsAsync(false);
        _appointmentRepo.Setup(x => x.GetAppointmentCountAsync(preferred)).ReturnsAsync(50);
        _appointmentRepo.Setup(x => x.GetAppointmentCountAsync(next)).ReturnsAsync(10);
        _settingsRepo.Setup(x => x.GetMaxAppointmentsPerDayAsync()).ReturnsAsync(50);
        _appointmentRepo.Setup(x => x.GetLastAppointmentAsync(next))
            .ReturnsAsync(new Appointment
            {
                Id = 10,
                TokenNumber = "T010",
                AppointmentDate = next
            });

        var result = await _service.BookAppointmentAsync(request);

        Assert.Equal(next.Date, result.AppointmentDate.Date);
        Assert.Equal("T011", result.TokenNumber);
    }

    [Fact]
    public async Task BookAppointment_ShouldRejectPastDate()
    {
        var request = new BookAppointmentRequest
        {
            CustomerName = "Rahul",
            CustomerPhone = "9876543210",
            PreferredDate = DateTime.Today.AddDays(-1)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.BookAppointmentAsync(request));
    }

    [Fact]
    public async Task BookAppointment_ShouldRejectMissingCustomerName()
    {
        var request = new BookAppointmentRequest
        {
            CustomerName = "  ",
            CustomerPhone = "9876543210",
            PreferredDate = DateTime.Today.AddDays(1)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.BookAppointmentAsync(request));
    }

    [Fact]
    public async Task GetAppointmentsByDate_ShouldReturnOrderedQueue()
    {
        var date = DateTime.Today;
        var appointments = new List<Appointment>
        {
            new()
            {
                Id = 1,
                TokenNumber = "T001",
                AppointmentDate = date,
                AppointmentTime = new TimeSpan(10, 0, 0),
                Status = "Waiting",
                Customer = Customer("Rahul", "9876543210")
            },
            new()
            {
                Id = 2,
                TokenNumber = "T002",
                AppointmentDate = date,
                AppointmentTime = new TimeSpan(10, 15, 0),
                Status = "Completed",
                Customer = Customer("Priya", "9876543211")
            }
        };

        _appointmentRepo.Setup(x => x.GetAppointmentsByDateAsync(date))
            .ReturnsAsync(appointments);

        var result = await _service.GetAppointmentsByDateAsync(date);

        Assert.Equal(2, result.Count);
        Assert.Equal("T001", result[0].TokenNumber);
        Assert.Equal("Rahul", result[0].CustomerName);
        Assert.Equal("Completed", result[1].Status);
    }

    [Fact]
    public async Task CompleteAppointment_ShouldSetStatusToCompleted()
    {
        var appointment = new Appointment
        {
            Id = 1,
            TokenNumber = "T001",
            AppointmentDate = DateTime.Today,
            Status = "Waiting",
            Customer = Customer("Rahul", "9876543210")
        };

        _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(appointment);

        var result = await _service.CompleteAppointmentAsync(1);

        Assert.True(result);
        Assert.Equal("Completed", appointment.Status);
        _appointmentRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelAppointment_ShouldSetStatusToCancelled()
    {
        var appointment = new Appointment
        {
            Id = 1,
            TokenNumber = "T001",
            AppointmentDate = DateTime.Today,
            Status = "Waiting",
            Customer = Customer("Rahul", "9876543210")
        };

        _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(appointment);

        var result = await _service.CancelAppointmentAsync(1);

        Assert.True(result);
        Assert.Equal("Cancelled", appointment.Status);
        _appointmentRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CompleteAppointment_ShouldReturnFalse_WhenNotFound()
    {
        _appointmentRepo.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Appointment?)null);

        var result = await _service.CompleteAppointmentAsync(999);

        Assert.False(result);
        _appointmentRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CancelAppointment_ShouldReturnFalse_WhenNotFound()
    {
        _appointmentRepo.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Appointment?)null);

        var result = await _service.CancelAppointmentAsync(999);

        Assert.False(result);
        _appointmentRepo.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CompleteAppointment_ShouldThrow_WhenAppointmentCancelled()
    {
        var appointment = new Appointment
        {
            Id = 1,
            TokenNumber = "T001",
            AppointmentDate = DateTime.Today,
            Status = "Cancelled",
            Customer = Customer("Rahul", "9876543210")
        };

        _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(appointment);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CompleteAppointmentAsync(1));
    }
}
