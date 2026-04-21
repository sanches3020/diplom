using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
    public DbSet<GoalAction> GoalActions { get; set; }
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
    public DbSet<PsychologistReview> PsychologistReviews { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSettings> NotificationSettings { get; set; }
    public DbSet<AdminLog> AdminLogs { get; set; }
    public DbSet<ForumCategory> ForumCategories { get; set; }
    public DbSet<ForumThread> ForumThreads { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumPostLike> ForumPostLikes { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Complaint> Complaints { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Глобальный конвертер для всех DateTime полей → UTC
        var converter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue 
                ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime())
                : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(converter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableConverter);
                }
            }
        }

        base.OnModelCreating(builder);

        // -----------------------------
        // ВАЖНО: УБРАНО HasConversion (pgcrypto нельзя так использовать)
        // Шифрование выполняется на уровне сервиса через DbFunction
        // -----------------------------
        // ApplicationUser → Psychologist (1:1)
        // -----------------------------
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Psychologist)
            .WithOne(p => p.User)
            .HasForeignKey<ApplicationUser>(u => u.PsychologistId)
            .OnDelete(DeleteBehavior.SetNull);

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

        // GoalAction → Goal
        builder.Entity<GoalAction>()
            .HasOne(ga => ga.Goal)
            .WithMany()
            .HasForeignKey(ga => ga.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        // GoalAction → User
        builder.Entity<GoalAction>()
            .HasOne(ga => ga.User)
            .WithMany(u => u.GoalActions)
            .HasForeignKey(ga => ga.UserId)
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

        // -----------------------------
        // Жалобы и модерация
        // -----------------------------
        builder.Entity<Complaint>(entity =>
        {
            entity.ToTable("Complaints");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            // Foreign key: SenderUser
            entity.HasOne(c => c.SenderUser)
                .WithMany()
                .HasForeignKey(c => c.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key: TargetUser
            entity.HasOne(c => c.TargetUser)
                .WithMany()
                .HasForeignKey(c => c.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Foreign key: Message (optional)
            entity.HasOne(c => c.Message)
                .WithMany()
                .HasForeignKey(c => c.MessageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Foreign key: Post (optional)
            entity.HasOne(c => c.Post)
                .WithMany()
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.SetNull);

            // Foreign key: ReviewedByAdmin (optional)
            entity.HasOne(c => c.ReviewedByAdmin)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes для быстрого поиска
            entity.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Complaints_Status");

            entity.HasIndex(c => c.TargetUserId)
                .HasDatabaseName("IX_Complaints_TargetUserId");

            entity.HasIndex(c => c.CreatedAt)
                .HasDatabaseName("IX_Complaints_CreatedAt");

            entity.HasIndex(c => new { c.TargetUserId, c.Status })
                .HasDatabaseName("IX_Complaints_TargetUserId_Status");
        });
    }
}