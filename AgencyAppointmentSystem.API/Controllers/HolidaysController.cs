using AgencyAppointmentSystem.Business.Interfaces;
using AgencyAppointmentSystem.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AgencyAppointmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService _holidayService;

    public HolidaysController(
        IHolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var holidays =
            await _holidayService.GetAllAsync();

        return Ok(holidays);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] Holiday holiday)
    {
        try
        {
            var result =
                await _holidayService.AddAsync(holiday);

            return CreatedAtAction(
                nameof(GetAll),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result =
            await _holidayService.DeleteAsync(id);

        if (!result)
            return NotFound(new
            {
                message = "Holiday not found."
            });

        return NoContent();
    }
}