using System.ComponentModel.DataAnnotations;

namespace AgencyAppointmentSystem.Business.DTOs;

public class BookAppointmentRequest
{
    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string CustomerPhone { get; set; } = string.Empty;

    [EmailAddress]
    public string? CustomerEmail { get; set; }

    [Required]
    public DateTime PreferredDate { get; set; }
}