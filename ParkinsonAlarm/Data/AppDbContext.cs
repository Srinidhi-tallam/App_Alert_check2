using Microsoft.EntityFrameworkCore;
using ParkinsonAlarm.Models;

namespace ParkinsonAlarm.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Alarm> Alarms => Set<Alarm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure owned type for Location so EF maps it into parent table columns
        modelBuilder.Entity<Patient>().OwnsOne(p => p.Location);
        modelBuilder.Entity<Alarm>().OwnsOne(a => a.Location);
    }
}