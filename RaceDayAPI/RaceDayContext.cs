
using Microsoft.EntityFrameworkCore;
using RaceDayManagementSystem.Models;

namespace RaceDayManagementSystem.Data;

public class RaceDayContext : DbContext
{
    public RaceDayContext(
        DbContextOptions<RaceDayContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organiser> Organisers => Set<Organiser>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(x => x.DateCreated)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Users_Role",
                "[Role] IN ('Organiser', 'Participant')"));

        modelBuilder.Entity<Organiser>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Organiser>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Participant>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Participant>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
            .HasOne(x => x.Organiser)
            .WithMany()
            .HasForeignKey(x => x.OrganiserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Events_Distance", "[DistanceKm] > 0"));

        modelBuilder.Entity<Category>()
            .HasOne(x => x.Event)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Categories_Age",
                    "[MinAge] >= 0 AND [MaxAge] >= [MinAge]");

                t.HasCheckConstraint(
                    "CK_Categories_Distance",
                    "[DistanceKm] > 0");
            });

        modelBuilder.Entity<Enrolment>()
            .Property(x => x.EnrolmentDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Enrolment>()
            .Property(x => x.Status)
            .HasDefaultValue("Active");

        modelBuilder.Entity<Enrolment>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_Enrolments_Status",
                "[Status] IN ('Active', 'Cancelled', 'Completed')"));

        modelBuilder.Entity<Enrolment>()
            .HasIndex(x => new { x.ParticipantId, x.EventId })
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .HasOne(x => x.Participant)
            .WithMany()
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(x => x.Event)
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Result>()
            .Property(x => x.CapturedDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Result>()
            .HasIndex(x => x.EnrolmentId)
            .IsUnique();

        modelBuilder.Entity<Result>()
            .ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Results_Position",
                    "[FinishPosition] IS NULL OR [FinishPosition] > 0");

                t.HasCheckConstraint(
                    "CK_Results_TotalFinishers",
                    "[TotalFinishers] IS NULL OR [TotalFinishers] > 0");
            });

        modelBuilder.Entity<Result>()
            .HasOne(x => x.Enrolment)
            .WithOne(x => x.Result)
            .HasForeignKey<Result>(x => x.EnrolmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
