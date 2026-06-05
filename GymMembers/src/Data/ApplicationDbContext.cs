using GymMembers.Models;
using Microsoft.EntityFrameworkCore;

namespace GymMembers.Data;

/* The Gym Membership example includes an many‑to‑many relationship between GymMember and ClassSession.
   This will be a new table in the DB that has two FKs to the other tables, but no CLR class in our code.
   Using Entity.UsingEntity will create the table purely from the model.
   We also seed test data so the DB is populated on first migration/update. */

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<GymMember> GymMembers => Set<GymMember>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // GymMember config
        modelBuilder.Entity<GymMember>(entity =>
        {
            entity.Property(m => m.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(m => m.LastName).HasMaxLength(100).IsRequired();
        });

        // ClassSession config
        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
        });

        // Many-to-many with join table that is NOT a class
        modelBuilder.Entity<GymMember>()
            .HasMany(m => m.ClassSessions)
            .WithMany(c => c.GymMembers)
            .UsingEntity<Dictionary<string, object>>(
                "GymMemberClassSession", // join table name
                j => j
                    .HasOne<ClassSession>()
                    .WithMany()
                    .HasForeignKey("ClassSessionId")
                    .HasConstraintName("FK_GymMemberClassSession_ClassSession")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<GymMember>()
                    .WithMany()
                    .HasForeignKey("GymMemberId")
                    .HasConstraintName("FK_GymMemberClassSession_GymMember")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("GymMemberId", "ClassSessionId");
                    j.ToTable("GymMemberClassSession");

                    // Seed join data here
                    j.HasData(
                        new { GymMemberId = 1, ClassSessionId = 1 },
                        new { GymMemberId = 1, ClassSessionId = 2 },
                        new { GymMemberId = 2, ClassSessionId = 2 },
                        new { GymMemberId = 3, ClassSessionId = 3 }
                    );
                });

        // Seed GymMembers
        modelBuilder.Entity<GymMember>().HasData(
            new GymMember
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                JoinDate = new DateTime(2024, 1, 10)
            },
            new GymMember
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Smith",
                JoinDate = new DateTime(2024, 2, 5)
            },
            new GymMember
            {
                Id = 3,
                FirstName = "Charlie",
                LastName = "Brown",
                JoinDate = new DateTime(2024, 3, 1)
            }
        );

        // Seed ClassSessions
        modelBuilder.Entity<ClassSession>().HasData(
            new ClassSession
            {
                Id = 1,
                Name = "Morning Yoga",
                StartTime = new DateTime(2024, 6, 1, 8, 0, 0),
                Capacity = 20
            },
            new ClassSession
            {
                Id = 2,
                Name = "HIIT Blast",
                StartTime = new DateTime(2024, 6, 1, 18, 0, 0),
                Capacity = 15
            },
            new ClassSession
            {
                Id = 3,
                Name = "Spin Class",
                StartTime = new DateTime(2024, 6, 2, 7, 0, 0),
                Capacity = 25
            }
        );
    }
}
