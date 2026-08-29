namespace AgencyAppointmentSystem.Data.Entities;

public class Appointment
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string TokenNumber { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }

    public TimeSpan AppointmentTime { get; set; }

    public string Status { get; set; } = "Waiting";

    public DateTime CreatedAt { get; set; }

    public Customer Customer { get; set; } = null!;
}