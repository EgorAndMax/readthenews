namespace ReadTheNews.Migrations
{
    using System.Data.Entity.Migrations;
    using ReadTheNews.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<RssNewsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ReadTheNews.Models.RssNewsContext";
        }

        protected override void Seed(RssNewsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
