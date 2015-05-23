namespace ReadTheNews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RssCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RssItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        Link = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false, maxLength: 1000),
                        Date = c.DateTime(nullable: false),
                        ImageSrc = c.String(maxLength: 150),
                        IsLastNews = c.Boolean(nullable: false),
                        RssChannelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RssChannels", t => t.RssChannelId, cascadeDelete: true)
                .Index(t => t.RssChannelId);
            
            CreateTable(
                "dbo.RssChannels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 120),
                        Language = c.String(maxLength: 10),
                        Link = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false, maxLength: 500),
                        ImageSrc = c.String(maxLength: 150),
                        PubDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RssItemRssCategories",
                c => new
                    {
                        RssItem_Id = c.Int(nullable: false),
                        RssCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RssItem_Id, t.RssCategory_Id })
                .ForeignKey("dbo.RssItems", t => t.RssItem_Id, cascadeDelete: true)
                .ForeignKey("dbo.RssCategories", t => t.RssCategory_Id, cascadeDelete: true)
                .Index(t => t.RssItem_Id)
                .Index(t => t.RssCategory_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RssItems", "RssChannelId", "dbo.RssChannels");
            DropForeignKey("dbo.RssItemRssCategories", "RssCategory_Id", "dbo.RssCategories");
            DropForeignKey("dbo.RssItemRssCategories", "RssItem_Id", "dbo.RssItems");
            DropIndex("dbo.RssItemRssCategories", new[] { "RssCategory_Id" });
            DropIndex("dbo.RssItemRssCategories", new[] { "RssItem_Id" });
            DropIndex("dbo.RssItems", new[] { "RssChannelId" });
            DropTable("dbo.RssItemRssCategories");
            DropTable("dbo.RssChannels");
            DropTable("dbo.RssItems");
            DropTable("dbo.RssCategories");
        }
    }
}
