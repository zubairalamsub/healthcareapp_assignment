using HealthcareApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Office> Offices { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Caregiver> Caregivers { get; set; }
        public DbSet<PatientCaregiver> PatientCaregivers { get; set; }
        public DbSet<SearchAudit> SearchAudits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientCaregiver>()
                .HasKey(pc => new { pc.PatientId, pc.CaregiverId });

            modelBuilder.Entity<PatientCaregiver>()
                .HasOne(pc => pc.Patient)
                .WithMany(p => p.PatientCaregivers)
                .HasForeignKey(pc => pc.PatientId);

            modelBuilder.Entity<PatientCaregiver>()
                .HasOne(pc => pc.Caregiver)
                .WithMany(c => c.PatientCaregivers)
                .HasForeignKey(pc => pc.CaregiverId);

            modelBuilder.Entity<Caregiver>()
                .HasIndex(c => new { c.FirstName, c.LastName, c.Phone });

            modelBuilder.Entity<Patient>()
                .HasIndex(p => new { p.FirstName, p.LastName, p.Phone });

            modelBuilder.Entity<Office>()
                .HasIndex(o => o.Name);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
