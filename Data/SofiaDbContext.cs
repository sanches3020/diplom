using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Models;

namespace Sofia.Web.Data;

public class SofiaDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public SofiaDbContext(DbContextOptions<SofiaDbContext> options)
        : base(options)
    {
    }

    // -----------------------------
    // Таблицы приложения
    // -----------------------------
    public DbSet<Note> Notes { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Practice> Practices { get; set; }
    public DbSet<EmotionEntry> EmotionEntries { get; set; }

    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<TestInterpretation> TestInterpretations { get; set; }

    public DbSet<Psychologist> Psychologists { get; set; }
    public DbSet<PsychologistSchedule> PsychologistSchedules { get; set; }
    public DbSet<PsychologistTimeSlot> PsychologistTimeSlots { get; set; }
    public DbSet<PsychologistAppointment> PsychologistAppointments { get; set; }

    // -----------------------------
    // Конфигурация моделей
    // -----------------------------
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // -----------------------------
        // ApplicationUser → PsychologistProfile (1:1)
        // -----------------------------
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.PsychologistProfile)
            .WithOne(p => p.User)
            .HasForeignKey<ApplicationUser>(u => u.PsychologistProfileId)
            .OnDelete(DeleteBehavior.SetNull);

        // -----------------------------
        // Note → User (много к одному)
        // -----------------------------
        builder.Entity<Note>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // Goal → User
        // -----------------------------
        builder.Entity<Goal>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Goals)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // Appointment → User
        // -----------------------------
        builder.Entity<PsychologistAppointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // TestResult → User
        // -----------------------------
        builder.Entity<TestResult>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // UserAnswer → User
        // -----------------------------
        builder.Entity<UserAnswer>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------------------
        // PostgreSQL-specific optimizations
        // -----------------------------
        builder.HasPostgresExtension("uuid-ossp"); // если будем использовать UUID
    }
}
