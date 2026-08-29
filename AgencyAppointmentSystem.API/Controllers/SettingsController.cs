using AgencyAppointmentSystem.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgencyAppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IAgencySettingsService _service;

    public SettingsController(
        IAgencySettingsService service)
    {
        _service = service;
    }

    [HttpGet("max-appointments")]
    public async Task<IActionResult> GetMaxAppointments()
    {
        var result =
            await _service
                .GetMaxAppointmentsPerDayAsync();

        return Ok(new
        {
            maxAppointmentsPerDay = result
        });
    }

    [HttpPut("max-appointments")]
    public async Task<IActionResult> SetMaxAppointments(
        [FromBody] int maxAppointments)
    {
        try
        {
            await _service
                .SetMaxAppointmentsPerDayAsync(
                    maxAppointments);

            return Ok(new
            {
                message =
                    "Maximum appointments updated successfully.",

                maxAppointmentsPerDay =
                    maxAppointments
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}