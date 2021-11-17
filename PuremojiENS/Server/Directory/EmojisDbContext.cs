using Microsoft.EntityFrameworkCore;
using PuremojiENS.Shared;

namespace PuremojiENS.Server.Directory
{
    public class EmojisDbContext : DbContext
    {
        public DbSet<Emoji> Emojis { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Emojis.db");
        }
    }
}