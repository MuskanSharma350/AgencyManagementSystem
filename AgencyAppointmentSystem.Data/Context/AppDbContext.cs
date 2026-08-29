using AgencyAppointmentSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgencyAppointmentSystem.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<Holiday> Holidays => Set<Holiday>();

    public DbSet<AgencySettings> AgencySettings => Set<AgencySettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Phone)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(150);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(x => x.TokenNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => new
            {
                x.AppointmentDate,
                x.TokenNumber
            }).IsUnique();

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasIndex(x => x.HolidayDate)
                .IsUnique();

            entity.Property(x => x.Description)
                .HasMaxLength(200);
        });
    }
}