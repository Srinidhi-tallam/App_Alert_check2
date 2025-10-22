using ParkinsonAlarm.DTOs;
using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreateAsync(CreatePatientDto dto, CancellationToken ct = default);
    Task<Patient?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Alarm>> GetAlarmsAsync(Guid patientId, CancellationToken ct = default);
    Task<Alarm> UpdateStatusAsync(Guid patientId, PatientStatusDto status, CancellationToken ct = default);

    Task<IEnumerable<Patient>> ListAsync(CancellationToken ct = default);

}