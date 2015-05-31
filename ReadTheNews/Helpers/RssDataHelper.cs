using ReadTheNews.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ReadTheNews.Helpers
{
    public class RssDataHelper : IDisposable
    {
        private RssChannel _Channel;
        private List<RssItem> _Items;
        private List<RssCategory> _Categories;
        private RssNewsContext db;

        public RssDataHelper(RssChannel channel)
        {
            db = new RssNewsContext();

            _Channel = channel;
            if (!db.RssChannels.Any(rc => rc.Title == _Channel.Title))
            {
                db.RssChannels.Add(channel);
                db.SaveChanges();
            }

            _Items = new List<RssItem>();
            _Categories = new List<RssCategory>();
        }

        public void AddRssItem(RssItem item)
        {
            var item_categories = item.RssCategories.ToList();

            for(int i = 0; i < item_categories.Count; i++)
            {
                var category = _Categories.Find(ri => item_categories[i].Name == ri.Name);
                if (category != null)
                    item_categories[i] = category;
                else
                    _Categories.Add(item_categories[i]);
            }

            item.RssCategories = item_categories;
            _Items.Add(item);
        }

        public RssItem GetLastRssItemFromDb()
        {
            return db.RssItems.FirstOrDefault(i => i.IsLastNews && i.RssChannelId == _Channel.Id);
        }

        public void UpdateRssChannel(RssChannel channel) // test, problem with Id?
        {
            db.Entry(channel).State = EntityState.Modified;
        }

        public void SumbitChanges()
        {
            db.SaveChanges(); // update channel

            db.RssCategories.AddRange(_Categories);
            db.SaveChanges();

            db.RssItems.AddRange(_Items);
            db.SaveChanges();
        }

        private void DeleteOldRssItems(int channelId) // вставить в код
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;

            var deletedNews = from n in db.RssItems
                              where n.Date < yesterday &&
                                    n.RssChannelId == channelId
                              select n;

            db.RssItems.RemoveRange(deletedNews);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}