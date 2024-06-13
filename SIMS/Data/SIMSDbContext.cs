using Microsoft.EntityFrameworkCore;
using SIMS.Data.Entities;

namespace SIMS.Data
{
    public class SIMSDbContext : DbContext
    {
        public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<StudentDetail> StudentDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
