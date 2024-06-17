using Microsoft.EntityFrameworkCore;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Admin;

namespace SIMS.Data
{
    public class SIMSDbContext : DbContext
    {
        // string connectionString = "Data Source=TAIKUN\\SQLEXPRESS;Initial Catalog=SIMS;Persist Security Info=True;User ID=lythaicuong;Password=***********;Trust Server Certificate=True"; 
        public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
        }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<UserInfos> UserInfos { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Lecturers> Lecturers { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Majors> Majors { get; set; }
        public DbSet<Subjects> Subjects { get; set; }
        public DbSet<Semesters> Semesters { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Enrollments> Enrollments { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }

    }
}
