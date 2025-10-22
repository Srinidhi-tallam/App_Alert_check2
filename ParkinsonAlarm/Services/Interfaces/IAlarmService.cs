using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.Services.Interfaces;

public interface IAlarmService
{
    Task<Alarm> CreateAndNotifyAsync(Patient patient, string message, Location? location = null, string severity = "High", CancellationToken ct = default);
    Task<IEnumerable<Alarm>> ListAsync(CancellationToken ct = default);
    Task<bool> AcknowledgeAsync(Guid alarmId, CancellationToken ct = default);
}