using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.DTOs;

public class UpdateStatusDto
{
    public bool IsWalkingAlone { get; set; }
    public bool IsFalling { get; set; }
    public Location? Location { get; set; }
    public string? Notes { get; set; }
}