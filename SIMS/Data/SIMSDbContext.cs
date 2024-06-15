﻿using Microsoft.EntityFrameworkCore;
using SIMS.Data.Entities;

namespace SIMS.Data
{
    public class SIMSDbContext : DbContext
    {
        public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<UserInfos> UserInfos { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Majors> Majors { get; set; }
        public DbSet<Subjects> Subjects { get; set; }
        public DbSet<Semesters> Semesters { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Enrollments> Enrollments { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }

    }
}
