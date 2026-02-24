using BackendApiExam.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendApiExam.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketStatusLog> TicketStatusLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(e =>
            {
                e.HasIndex(r => r.Name).IsUnique();
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.HasOne(u => u.Role)
                 .WithMany(r => r.Users)
                 .HasForeignKey(u => u.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ticket>(e =>
            {
                e.Property(t => t.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20);

                e.Property(t => t.Priority)
                 .HasConversion<string>()
                 .HasMaxLength(10);

                e.HasOne(t => t.CreatedByUser)
                 .WithMany(u => u.CreatedTickets)
                 .HasForeignKey(t => t.CreatedBy)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.AssignedToUser)
                 .WithMany(u => u.AssignedTickets)
                 .HasForeignKey(t => t.AssignedTo)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TicketComment>(e =>
            {
                e.HasOne(c => c.Ticket)
                 .WithMany(t => t.Comments)
                 .HasForeignKey(c => c.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(c => c.User)
                 .WithMany(u => u.Comments)
                 .HasForeignKey(c => c.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TicketStatusLog>(e =>
            {
                e.Property(l => l.OldStatus)
                 .HasConversion<string>()
                 .HasMaxLength(20);

                e.Property(l => l.NewStatus)
                 .HasConversion<string>()
                 .HasMaxLength(20);

                e.HasOne(l => l.Ticket)
                 .WithMany(t => t.StatusLogs)
                 .HasForeignKey(l => l.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(l => l.ChangedByUser)
                 .WithMany()
                 .HasForeignKey(l => l.ChangedBy)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "MANAGER" },
                new Role { Id = 2, Name = "SUPPORT" },
                new Role { Id = 3, Name = "USER" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "System Manager",
                    Email = "jeet@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("jeet@123"),
                    RoleId = 1,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}