using System.ComponentModel.DataAnnotations;

namespace ParkinsonAlarm.Models;

public class Patient
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public Location? Location { get; set; }

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;

    public List<Alarm> Alarms { get; set; } = new();
}