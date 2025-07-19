using ElevatorSystem.API.Models.Entities;
using ElevatorSystem.API.Models.Enums; 
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ElevatorSystem.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Building> Buildings { get; set; } = null!;
        public DbSet<Elevator> Elevators { get; set; } = null!;
        public DbSet<ElevatorCall> ElevatorCalls { get; set; } = null!;
        public DbSet<ElevatorCallAssignment> ElevatorCallAssignments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.UpdatedAt)
                    .IsRequired(false);
            });

            // Building entity configuration
            modelBuilder.Entity<Building>(entity =>
            {
                entity.ToTable("Buildings");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(b => b.NumberOfFloors)
                    .IsRequired();

                entity.Property(b => b.UserId)
                    .IsRequired();

                entity.Property(b => b.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(b => b.UpdatedAt)
                    .IsRequired(false);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Elevator entity configuration
            modelBuilder.Entity<Elevator>(entity =>
            {
                entity.ToTable("Elevators");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BuildingId)
                    .IsRequired();

                entity.Property(e => e.CurrentFloor)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.Property(e => e.Direction)
                    .IsRequired();

                entity.Property(e => e.DoorStatus)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);

                entity.HasOne<Building>()
                      .WithMany()
                      .HasForeignKey(e => e.BuildingId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ElevatorCall entity
            modelBuilder.Entity<ElevatorCall>(entity =>
            {
                entity.ToTable("ElevatorCalls");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BuildingId).IsRequired();
                entity.Property(e => e.ElevatorId).IsRequired(false);
                entity.Property(e => e.RequestedFloor).IsRequired();
                entity.Property(e => e.CallTime).IsRequired();
                entity.Property(e => e.IsHandled).IsRequired();

                
                entity.Property(e => e.Direction)
                      .HasConversion<int>() 
                      .IsRequired();

                entity.HasOne(e => e.Building)
                    .WithMany()
                    .HasForeignKey(e => e.BuildingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Elevator)
                    .WithMany()
                    .HasForeignKey(e => e.ElevatorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ElevatorCallAssignment entity
            modelBuilder.Entity<ElevatorCallAssignment>(entity =>
            {
                entity.ToTable("ElevatorCallAssignments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ElevatorId).IsRequired();
                entity.Property(e => e.ElevatorCallId).IsRequired();
                entity.Property(e => e.AssignmentTime).IsRequired();

                entity.HasOne(e => e.ElevatorCall)
                    .WithMany()
                    .HasForeignKey(e => e.ElevatorCallId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Elevator)
                    .WithMany()
                    .HasForeignKey(e => e.ElevatorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = System.DateTime.UtcNow;
                    entity.UpdatedAt = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = System.DateTime.UtcNow;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
