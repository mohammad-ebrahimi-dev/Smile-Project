using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;

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
        public DbSet<Log> Logs { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<CategorySentence> CategorySentences { get; set; }
        public DbSet<Otp> Otps { get; set; }
    }
}
