using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkinsonAlarm.Models;

public class Alarm
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid PatientId { get; set; }

    [ForeignKey(nameof(PatientId))]
    public Patient? Patient { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string Message { get; set; } = string.Empty;

    public Location? Location { get; set; }

    public bool IsAcknowledged { get; set; } = false;

    public DateTimeOffset? AcknowledgedAt { get; set; }

    public string Severity { get; set; } = "High";
}