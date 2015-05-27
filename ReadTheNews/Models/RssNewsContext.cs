﻿using System.Data.Entity;

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
        public DbSet<UserSubscribedChannels> SubscribedChannels { get; set; }
        public DbSet<RssItemRssCategories> RssItemRssCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFavoriteNews>().HasKey(t => new { t.RssItemId, t.UserId });
            modelBuilder.Entity<UserDefferedNews>().HasKey(d => new { d.RssItemId, d.UserId });
            modelBuilder.Entity<UserSubscribedChannels>().HasKey(s => new { s.RssChannelId, s.UserId });

            modelBuilder.Entity<DeletedRssItemsByUser>().HasKey(d => new { d.RssItemId, d.UserId });
            modelBuilder.Entity<DeletedRssItemsByUser>().HasRequired(d => d.RssItem)
                .WithMany(ri => ri.DeletedNews)
                .HasForeignKey(d => d.RssItemId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<RssItemRssCategories>().HasKey(rirc => new { rirc.RssItemId, rirc.RssCategoryId });
            modelBuilder.Entity<RssItemRssCategories>().HasRequired(rirc => rirc.RssItem)
                .WithMany(ri => ri.RssItemRssCategories)
                .HasForeignKey(rirc => rirc.RssItemId)
                .WillCascadeOnDelete();
            modelBuilder.Entity<RssItemRssCategories>().HasRequired(rirc => rirc.RssCategory)
                .WithMany(rc => rc.RssItemRssCategories)
                .HasForeignKey(rirc => rirc.RssCategoryId)
                .WillCascadeOnDelete();

            base.OnModelCreating(modelBuilder);
        }
    }
}