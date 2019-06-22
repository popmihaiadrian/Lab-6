using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_2_webapi.Models
{

    //DbContext = Unit of work
    public class TasksDbContext :DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
                entity.HasIndex(u => u.Username).IsUnique();
            });

            //builder.Entity<UserUserRole>(entity => {
            //    entity.HasKey(u => new { u.UserId, u.UserRoleId }).IsUnique();
            //});


            builder.Entity<Task>()
                .HasOne(u => u.Owner)
                .WithMany(f => f.Tasks)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public TasksDbContext(DbContextOptions<TasksDbContext> optons) : base(optons)
        {
        }

        //DbSet = Repository
        //DbSet = O tabela din baza de date
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserUserRole> UserUserRoles { get; set; }
    }
}
