namespace ParkinsonAlarm.Models;

public class PatientStatusDto
{
    public bool IsWalkingAlone { get; set; }
    public bool IsFalling { get; set; }
    public Location? Location { get; set; }
    public string? Notes { get; set; }
}