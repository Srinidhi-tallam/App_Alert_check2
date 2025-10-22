using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParkinsonAlarm.Data;
using ParkinsonAlarm.DTOs;
using ParkinsonAlarm.Hubs;
using ParkinsonAlarm.Models;
using ParkinsonAlarm.Services.Interfaces;

namespace ParkinsonAlarm.Services;

public class AlarmService : IAlarmService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<AlarmHub> _hub;
    private readonly IConfiguration _config;
    private readonly ILogger<AlarmService> _logger;

    public AlarmService(AppDbContext db, IHubContext<AlarmHub> hub, IConfiguration config, ILogger<AlarmService> logger)
    {
        _db = db;
        _hub = hub;
        _config = config;
        _logger = logger;
    }

    public async Task<Alarm> CreateAndNotifyAsync(Patient patient, string message, Location? location = null, string severity = "High", CancellationToken ct = default)
    {
        var alarm = new Alarm
        {
            PatientId = patient.Id,
            Message = message,
            Location = location ?? patient.Location,
            Severity = severity,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Alarms.Add(alarm);
        await _db.SaveChangesAsync(ct);

        var dto = new AlarmDto
        {
            Id = alarm.Id,
            PatientId = alarm.PatientId,
            PatientName = patient.Name,
            CreatedAt = alarm.CreatedAt,
            Message = alarm.Message,
            Location = alarm.Location,
            Severity = alarm.Severity
        };

        await _hub.Clients.All.SendAsync("ReceiveAlarm", dto, cancellationToken: ct);

        var smsEnabled = _config.GetValue<bool>("Notification:SmsEnabled");
        var emailEnabled = _config.GetValue<bool>("Notification:EmailEnabled");

        if (smsEnabled)
        {
            Console.WriteLine($"[SMS] Would send SMS: {dto.Message}");
        }

        if (emailEnabled)
        {
            Console.WriteLine($"[Email] Would send email: {dto.Message}");
        }

        return alarm;
    }

    public async Task<IEnumerable<Alarm>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Alarms.Include(a => a.Patient).OrderByDescending(a => a.CreatedAt).ToListAsync(ct);
    }

    public async Task<bool> AcknowledgeAsync(Guid alarmId, CancellationToken ct = default)
    {
        _logger.LogInformation("AcknowledgeAsync: searching alarm {AlarmId}", alarmId);
        var a = await _db.Alarms.FirstOrDefaultAsync(x => x.Id == alarmId, ct);
        if (a == null)
        {
            _logger.LogInformation("AcknowledgeAsync: alarm {AlarmId} not found", alarmId);
            return false;
        }

        a.IsAcknowledged = true;
        a.AcknowledgedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);

        await _hub.Clients.All.SendAsync("AlarmAcknowledged", new { alarmId = a.Id, acknowledgedAt = a.AcknowledgedAt }, cancellationToken: ct);
        _logger.LogInformation("AcknowledgeAsync: alarm {AlarmId} acknowledged", alarmId);
        return true;
    }
}