using System.Data.Entity;

namespace ReadTheNews.Models
{
    public class RssNewsContext : DbContext
    {
        public RssNewsContext()
            : base("DefaultConnection")
        { }

        public DbSet<RssCategory> RssCategories { get; set; }
        public DbSet<RssChannel> RssChannels { get; set; }
        public DbSet<RssItem> RssItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}