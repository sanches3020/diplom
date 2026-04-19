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
    // Основные сущности
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

    // -----------------------------
    // Психологи
    // -----------------------------
    public DbSet<Psychologist> Psychologists { get; set; }
    public DbSet<PsychologistSchedule> PsychologistSchedules { get; set; }
    public DbSet<PsychologistTimeSlot> PsychologistTimeSlots { get; set; }
    public DbSet<PsychologistAppointment> PsychologistAppointments { get; set; }

    // -----------------------------
    // Отзывы
    // -----------------------------
    public DbSet<PsychologistReview> PsychologistReviews { get; set; }

    // -----------------------------
    // Уведомления
    // -----------------------------
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }

    // -----------------------------
    // Логи
    // -----------------------------
    public DbSet<AdminLog> AdminLogs { get; set; }

    // -----------------------------
    // Форум
    // -----------------------------
    public DbSet<ForumCategory> ForumCategories { get; set; }
    public DbSet<ForumThread> ForumThreads { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumPostLike> ForumPostLikes { get; set; }

    // -----------------------------
    // Медиа
    // -----------------------------
    public DbSet<MediaFile> MediaFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // -----------------------------
<<<<<<< HEAD
        // ВАЖНО: УБРАНО HasConversion (pgcrypto нельзя так использовать)
        // Шифрование выполняется на уровне сервиса через DbFunction
        // -----------------------------
=======
        // ApplicationUser → Psychologist (1:1)
        // -----------------------------
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Psychologist)
            .WithOne(p => p.User)
            .HasForeignKey<ApplicationUser>(u => u.PsychologistId)
            .OnDelete(DeleteBehavior.SetNull);
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab

        // -----------------------------
        // Note → User
        // -----------------------------
        builder.Entity<Note>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Goal → User
        builder.Entity<Goal>()
            .HasOne<ApplicationUser>()
            .WithMany(u => u.Goals)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Appointment → User
        builder.Entity<PsychologistAppointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // TestResult → User
        builder.Entity<TestResult>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserAnswer → User
        builder.Entity<UserAnswer>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // AdminLog → Admin
        builder.Entity<AdminLog>()
            .HasOne(l => l.Admin)
            .WithMany()
            .HasForeignKey(l => l.AdminId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicationUser: IsBlocked
        builder.Entity<ApplicationUser>()
            .Property(u => u.IsBlocked)
            .HasDefaultValue(false);

        // -----------------------------
        // Форум (schema: forum)
        // -----------------------------

        builder.Entity<ForumCategory>(entity =>
        {
            entity.ToTable("Categories", schema: "forum");

            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Description)
                .HasMaxLength(500);
        });

        builder.Entity<ForumThread>(entity =>
        {
            entity.ToTable("Threads", schema: "forum");

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Threads)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ForumPost>(entity =>
        {
            entity.ToTable("Posts", schema: "forum");

            entity.Property(p => p.Content)
                .IsRequired();

            entity.HasOne(p => p.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.MediaFile)
                .WithMany()
                .HasForeignKey(p => p.MediaFileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ForumPostLike>(entity =>
        {
            entity.ToTable("PostLikes", schema: "forum");

            entity.HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();
        });

        // -----------------------------
        // MediaFile
        // -----------------------------
        builder.Entity<MediaFile>(entity =>
        {
            entity.ToTable("MediaFiles");

            entity.Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(m => m.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(m => m.ContentType)
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}