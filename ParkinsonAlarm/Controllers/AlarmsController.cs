using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkinsonAlarm.DTOs;
using ParkinsonAlarm.Services.Interfaces;

namespace ParkinsonAlarm.Controllers;

[ApiController]
[Route("api/alarms")]
public class AlarmsController : ControllerBase
{
    private readonly IAlarmService _alarms;
    private readonly ILogger<AlarmsController> _logger;

    public AlarmsController(IAlarmService alarms, ILogger<AlarmsController> logger)
    {
        _alarms = alarms;
        _logger = logger;
    }

    // GET /api/alarms
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlarmDto>), 200)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var list = await _alarms.ListAsync(ct);

        // Map entity -> DTO to avoid EF navigation serialization issues
        var dtoList = list.Select(a => new AlarmDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient?.Name,
            CreatedAt = a.CreatedAt,
            Message = a.Message,
            Location = a.Location,
            Severity = a.Severity
        });

        return Ok(dtoList);
    }

    // POST /api/alarms/{id}/ack
    // Acknowledge an existing alarm by id (path)
    [HttpPost("{id:guid}/ack")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Ack([FromRoute] Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Ack called for alarm {AlarmId}", id);

        var ok = await _alarms.AcknowledgeAsync(id, ct);
        if (!ok)
        {
            _logger.LogWarning("Alarm {AlarmId} not found", id);
            return NotFound(new { message = "Alarm not found", id });
        }

        var acknowledgedAt = DateTimeOffset.UtcNow;
        _logger.LogInformation("Alarm {AlarmId} acknowledged at {AckAt}", id, acknowledgedAt);
        return Ok(new { message = "Acknowledged", id, acknowledgedAt });
    }

    // POST /api/alarms/ack
    // Convenience: accept JSON body { "id": "..." } for Swagger users
    [HttpPost("ack")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AckFromBody([FromBody] GuidIdDto dto, CancellationToken ct)
    {
        if (dto is null || dto.Id == Guid.Empty) return BadRequest(new { message = "id is required" });
        return await Ack(dto.Id, ct);
    }
}

public record GuidIdDto(Guid Id);