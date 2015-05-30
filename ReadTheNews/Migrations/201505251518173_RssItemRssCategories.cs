namespace ReadTheNews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RssItemRssCategories : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RssItemRssCategories");
            CreateTable(
                "dbo.RssItemRssCategories",
                c => new
                    {
                        RssCategoryId = c.Int(nullable: false),
                        RssItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RssCategoryId, t.RssItemId })
                .ForeignKey("dbo.RssItems", t => t.RssItemId, cascadeDelete: true)
                .ForeignKey("dbo.RssCategories", t => t.RssCategoryId, cascadeDelete: true)
                .Index(t => t.RssItemId)
                .Index(t => t.RssCategoryId);
            
        }
        
        public override void Down()
        {
        }
    }
}
