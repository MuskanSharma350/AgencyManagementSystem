using AgencyAppointmentSystem.Business.DTOs;
using AgencyAppointmentSystem.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgencyAppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(
        IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment(
        [FromBody] BookAppointmentRequest request)
    {
        try
        {
            var result =
                await _appointmentService
                    .BookAppointmentAsync(request);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] DateTime date)
    {
        var result =
            await _appointmentService
                .GetAppointmentsByDateAsync(date);

        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayAppointments()
    {
        var result =
            await _appointmentService
                .GetAppointmentsByDateAsync(
                    DateTime.Today);

        return Ok(result);
    }

    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var result =
                await _appointmentService
                    .CompleteAppointmentAsync(id);

            if (!result)
                return NotFound(new
                {
                    message = "Appointment not found."
                });

            return Ok(new
            {
                message =
                    "Appointment completed successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var result =
                await _appointmentService
                    .CancelAppointmentAsync(id);

            if (!result)
                return NotFound(new
                {
                    message = "Appointment not found."
                });

            return Ok(new
            {
                message =
                    "Appointment cancelled successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}