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
        public DbSet<UserFavoriteNews> FavoriteNews { get; set; }
        public DbSet<DeletedRssItemsByUser> DeletedNews { get; set; }
        public DbSet<UserDefferedNews> DefferedNews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFavoriteNews>().HasKey(t => new { t.RssItemId, t.UserId });
            modelBuilder.Entity<UserDefferedNews>().HasKey(d => new { d.RssItemId, d.UserId });
            modelBuilder.Entity<DeletedRssItemsByUser>().HasKey(d => new { d.RssItemId, d.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}