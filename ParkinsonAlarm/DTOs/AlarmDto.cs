using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.DTOs;

public class AlarmDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? Message { get; set; }
    public Location? Location { get; set; }
    public string? Severity { get; set; }
}