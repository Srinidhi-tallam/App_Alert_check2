using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkinsonAlarm.DTOs;
using ParkinsonAlarm.Models;
using ParkinsonAlarm.Services.Interfaces;

namespace ParkinsonAlarm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patients;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patients, ILogger<PatientsController> logger)
    {
        _patients = patients;
        _logger = logger;
    }

    // POST /api/patients
    [HttpPost]
    [ProducesResponseType(typeof(Patient), 201)]
    public async Task<ActionResult<Patient>> Create([FromBody] CreatePatientDto dto, CancellationToken ct)
    {
        var p = await _patients.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }

    // GET /api/patients/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Patient>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var p = await _patients.GetAsync(id, ct);
        if (p == null) return NotFound();
        return Ok(p);
    }

    // POST /api/patients/{id}/status
    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(Alarm), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateStatusDto dto, CancellationToken ct)
    {
        if (dto is null) return BadRequest();

        try
        {
            var alarm = await _patients.UpdateStatusAsync(id, new PatientStatusDto
            {
                IsWalkingAlone = dto.IsWalkingAlone,
                IsFalling = dto.IsFalling,
                Location = dto.Location,
                Notes = dto.Notes
            }, ct);

            return Ok(alarm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStatus failed for patient {PatientId}", id);
            // In development this will return details; in production you can return a generic Problem()
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    // GET /api/patients/{id}/alarms
    [HttpGet("{id:guid}/alarms")]
    [ProducesResponseType(typeof(IEnumerable<Alarm>), 200)]
    public async Task<ActionResult<IEnumerable<Alarm>>> GetAlarms([FromRoute] Guid id, CancellationToken ct)
    {
        var alarms = await _patients.GetAlarmsAsync(id, ct);
        return Ok(alarms);
    }

    // GET /api/patients
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
    public async Task<ActionResult<IEnumerable<Patient>>> List(CancellationToken ct)
    {
        var list = await _patients.ListAsync(ct);
        return Ok(list);
    }

}