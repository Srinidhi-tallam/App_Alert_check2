using Microsoft.EntityFrameworkCore;
using ParkinsonAlarm.Data;
using ParkinsonAlarm.DTOs;
using ParkinsonAlarm.Models;
using ParkinsonAlarm.Services.Interfaces;

namespace ParkinsonAlarm.Services;

public class PatientService : IPatientService
{
    private readonly AppDbContext _db;
    private readonly IAlarmService _alarmService;

    public PatientService(AppDbContext db, IAlarmService alarmService)
    {
        _db = db;
        _alarmService = alarmService;
    }

    public async Task<Patient> CreateAsync(CreatePatientDto dto, CancellationToken ct = default)
    {
        var p = new Patient
        {
            Name = dto.Name,
            Age = dto.Age,
            Location = dto.Location
        };
        _db.Patients.Add(p);
        await _db.SaveChangesAsync(ct);
        return p;
    }

    public async Task<Patient?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Patients.Include(p => p.Alarms).FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<Alarm>> GetAlarmsAsync(Guid patientId, CancellationToken ct = default)
    {
        return await _db.Alarms.Where(a => a.PatientId == patientId).OrderByDescending(a => a.CreatedAt).ToListAsync(ct);
    }

    public async Task<Alarm> UpdateStatusAsync(Guid patientId, PatientStatusDto status, CancellationToken ct = default)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == patientId, ct)
                      ?? throw new KeyNotFoundException("Patient not found");

        patient.LastUpdated = DateTimeOffset.UtcNow;
        if (status.Location != null) patient.Location = status.Location;
        await _db.SaveChangesAsync(ct);

        bool dangerous = status.IsWalkingAlone || status.IsFalling;

        if (dangerous)
        {
            var msgParts = new List<string>();
            if (status.IsFalling) msgParts.Add("fall detected");
            if (status.IsWalkingAlone) msgParts.Add("walking alone detected");
            if (!string.IsNullOrWhiteSpace(status.Notes)) msgParts.Add(status.Notes);

            string message = $"Patient {patient.Name}: {string.Join(", ", msgParts)}";
            var alarm = await _alarmService.CreateAndNotifyAsync(patient, message, patient.Location, severity: status.IsFalling ? "Critical" : "High", ct);
            return alarm;
        }

        var nonAlarm = new Alarm
        {
            PatientId = patient.Id,
            Message = "Status updated (no alarm)",
            Location = patient.Location,
            Severity = "Info",
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Alarms.Add(nonAlarm);
        await _db.SaveChangesAsync(ct);
        return nonAlarm;
    }

    public async Task<IEnumerable<Patient>> ListAsync(CancellationToken ct)
    {
        return await _db.Patients.ToListAsync(ct);
    }

}