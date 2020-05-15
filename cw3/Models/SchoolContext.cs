using cw3.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Models
{
    public class SchoolContext : DbContext
    {
        public SchoolContext()
        {

        }
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {

        }

        public DbSet<Student> students { get; set; }
        public DbSet<Enrollment> enrollments { get; set; }
        public DbSet<Studies> studies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //"Data Source=localhost,1433;Initial Catalog=SampleDb;User ID=sa;Password=Passw0rd"
                optionsBuilder.UseSqlServer("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True")
                    .UseLazyLoadingProxies();
            }
            //base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Student>().ToTable("StudentN");

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent).HasName("IdStudent");
                entity.Property(e => e.FirstName);
                entity.Property(e => e.LastName);
                entity.Property(e => e.IndexNumber);
                entity.Property(e => e.BirthDate);
                entity.Property(e => e.IdEnrollment);
            });

            modelBuilder.Entity<Enrollment>(e =>
            {
                e.HasKey(e => e.IdEnrollment).HasName("IdEnrollment");
                e.Property(e => e.Semester);
                e.Property(e => e.IdStudy);
                e.Property(e => e.StartDate);

            });

            modelBuilder.Entity<Studies>(e => 
            { 
                e.HasKey(e => e.IdStudy).HasName("studies");
                e.Property(e => e.Name);
            });
            //base.OnModelCreating(modelBuilder);
        }

    }
}
