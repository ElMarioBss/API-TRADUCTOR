using API_TRADUCTOR.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace API_TRADUCTOR
{
    public class ApiTraductorDB : DbContext
    {

        public ApiTraductorDB(DbContextOptions<ApiTraductorDB> options) 
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Mario A.", Email = "marioal.antonio@gmail.com", Password = "123" },
            new User { Id = 2, Name = "Lizandro", Email = "daniel@gmail.com", Password = "123" }
            );

            base.OnModelCreating(modelBuilder);

        }
    }
}