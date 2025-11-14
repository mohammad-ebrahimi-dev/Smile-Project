using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;

namespace SmileProject.Databes.MainDbContext
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
{
}

        public DbSet<Sentences> Sentences { get; set; }
    }
}
