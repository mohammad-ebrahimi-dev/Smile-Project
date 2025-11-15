using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Models;

namespace SmileProject.Databes.MainDbContext
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
{
}

        public DbSet<Sentences> Sentences { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
