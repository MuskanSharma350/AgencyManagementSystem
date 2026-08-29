namespace AgencyAppointmentSystem.Data.Entities;

public class AgencySettings
{
    public int Id { get; set; }

    public int MaxAppointmentsPerDay { get; set; }

    public DateTime UpdatedAt { get; set; }
}