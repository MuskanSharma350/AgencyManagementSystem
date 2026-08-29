namespace AgencyAppointmentSystem.Data.Entities;

public class Holiday
{
    public int Id { get; set; }

    public DateTime HolidayDate { get; set; }

    public string Description { get; set; } = string.Empty;
}