using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.DTOs;

public class CreatePatientDto
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Location? Location { get; set; }
}