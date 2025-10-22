using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Patients.Any()) return;

        var patient = new Patient
        {
            Name = "Alice Johnson",
            Age = 72,
            Location = new Location { Latitude = 47.6097, Longitude = -122.3331, Description = "Living Room" }
        };

        db.Patients.Add(patient);
        db.SaveChanges();
    }
}