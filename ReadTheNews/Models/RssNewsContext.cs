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
        public DbSet<FavoriteNews> FavoriteNews { get; set; }
        public DbSet<DeletedRssItemsByUser> DeletedNews { get; set; }
        public DbSet<UserDefferedNews> DefferedNews { get; set; }
        public DbSet<UserSubscribedChannels> SubscribedChannels { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDefferedNews>().HasKey(d => new { d.RssItemId, d.UserId });
            modelBuilder.Entity<DeletedRssItemsByUser>().HasKey(d => new { d.RssItemId, d.UserId });
            modelBuilder.Entity<UserSubscribedChannels>().HasKey(s => new { s.RssChannelId, s.UserId });
            modelBuilder.Entity<FavoriteNews>().HasKey(t => new { t.RssItemId, t.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}