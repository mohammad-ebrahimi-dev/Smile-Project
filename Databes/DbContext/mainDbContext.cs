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
        public DbSet<User> Users { get; set; }
        public DbSet<Sentences> Sentences { get; set; }
        public DbSet<UserSentence> UserSentences { get; set; }

    }
}
