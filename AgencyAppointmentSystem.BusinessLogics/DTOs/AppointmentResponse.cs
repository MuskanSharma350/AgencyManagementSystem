namespace AgencyAppointmentSystem.Business.DTOs;

public class AppointmentResponse
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string TokenNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    public string? CustomerEmail { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeSpan AppointmentTime { get; set; }

    public string Status { get; set; } = string.Empty;
}