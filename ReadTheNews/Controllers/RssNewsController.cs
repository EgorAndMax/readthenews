using Microsoft.AspNet.Identity;
using ReadTheNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ReadTheNews.Controllers
{
    [Authorize]
    public class RssNewsController : Controller
    {
        private RssNewsContext db = new RssNewsContext();

        private string _userId;
        private string UserId
        {
            get
            {
                if (String.IsNullOrEmpty(_userId))
                    _userId = User.Identity.GetUserId();
                return _userId;
            }
        }

        public ActionResult Index()
        {
            return Redirect("RssNews/Channels");
        }

        [AllowAnonymous]
        public ActionResult Channels()
        {
            var fn = db.FavoriteNews.ToList();
            var channels = db.RssChannels.ToList();
            ViewBag.RssChannels = channels;

            var countsCategories = (from d in db.DeletedNews
                                    let userId = UserId
                                    where d.UserId == userId
                                    from ri in db.RssItems
                                    where d.RssItemId == ri.Id
                                    select new CountNewsOfCategory
                                    {
                                        Count = 9,
                                        Name = "88"
                                    }
                                    ).Take(1).ToList();




            //Except(from d in db.DeletedNews
            //                              where d.UserId == UserId
            //                              join ri2 in db.RssItems on d.RssItemId equals ri2.Id
            //                              select d)
            //select ri

            //);

            ViewBag.CountsCategories = countsCategories;

            return View();
        }

        [HttpPost]
        public ActionResult GetNews(string url)
        {
            RssProcessor processor;
            try
            {
                processor = new RssProcessor(url);
                if (!processor.IsChannelDownload)
                    return RedirectToAction("Error");

                processor.GetLatestNews();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return Redirect("Error");
            }

            var channel = processor.CurrentChannel;
            channel.RssItems.ToArray();

            return View(@"RssNews\Channel", channel);
        }

        public ActionResult Channel(int? id)
        {
            if (id == null)
                return Redirect("Channels");

            int channelId = Int32.Parse(id.ToString());
            RssChannel channel = db.RssChannels.Find(channelId);
            RssProcessor processor;
            try
            {
                processor = new RssProcessor(channel);
                processor.GetLatestNews();

                ViewBag.IsSubscribe = db.SubscribedChannels.Any(sc => sc.RssChannelId == channelId && sc.UserId == UserId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error");
            }

            // TODO: реализовать фильтр по подписанным каналам
            ViewBag.CountsCategories = (from rc in db.RssCategories
                                        let userId = UserId
                                        let deletedNews = (from d in db.DeletedNews
                                                           where d.UserId == userId
                                                           join ri in db.RssItems on d.RssItemId equals ri.Id
                                                           select ri)
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = (from subs_news in rc.RssItems
                                                     join sn in db.SubscribedChannels on subs_news.RssChannelId equals sn.RssChannelId
                                                     where sn.UserId == userId
                                                     select subs_news).Except(deletedNews).Count()
                                        }).OrderByDescending(cnc => cnc.Count).Take(20).ToList();

            channel.RssItems = channel.RssItems.Except(from d in db.DeletedNews
                                                       where d.UserId == UserId
                                                       join ri in db.RssItems on d.RssItemId equals ri.Id
                                                       select ri).ToList();

            return View(channel);
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        public ActionResult MyNews()
        {
            var myNews = (from item in db.RssItems
                          join channel in db.RssChannels on item.RssChannelId equals channel.Id
                          join sc in db.SubscribedChannels on channel.Id equals sc.RssChannelId
                          where sc.UserId == UserId
                          select item
                          ).Except(from item in db.RssItems
                                   join d in db.DeletedNews on item.Id equals d.RssItemId
                                   where d.UserId == UserId
                                   select item).ToList();

            // TODO: реализовать фильтр по подписанным каналам
            ViewBag.CountsCategories = (from rc in db.RssCategories.Include("RssItems")
                                        let deletedNews = (from d in db.DeletedNews
                                                           where d.UserId == UserId
                                                           join ri in db.RssItems on d.RssItemId equals ri.Id
                                                           select ri)
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = rc.RssItems.Except(deletedNews).Count()
                                        }).OrderByDescending(cnc => cnc.Count).Take(20).ToList();

            return View(myNews);
        }

        public ActionResult MyChannels()
        {
            var myChannels = (from c in db.RssChannels
                              let userId = UserId
                              let deletedNews = (from ri in db.RssItems
                                                 join del in db.DeletedNews on ri.Id equals del.RssItemId
                                                 where del.UserId == userId
                                                 select ri)
                              join sc in db.SubscribedChannels on c.Id equals sc.RssChannelId
                              where sc.UserId == userId
                              select new ChannelNewsCount
                              {
                                  Id = c.Id,
                                  Title = c.Title,
                                  Count = (from n in c.RssItems.Except(deletedNews)
                                           select n.Id).Count()
                              }).ToList();

            // TODO: реализовать фильтр по подписанным каналам
            // ViewBag.CountsCategories =
            var r = (from rc in db.RssCategories.Include("RssItems")
                     let deletedNews = (from ri in db.RssItems
                                        join del in db.DeletedNews on ri.Id equals del.RssItemId
                                        where del.UserId == UserId
                                        select ri)
                     select new CountNewsOfCategory
                     {
                         Name = rc.Name,
                         Count = rc.RssItems.Except(deletedNews).Count() // вопрос! что здесь IQueryable или IEnumerable??
                     }).OrderByDescending(cnc => cnc.Count).Take(20).ToList();

            return View(myChannels);
        }

        public ActionResult GetNewsByCategory(string name)
        {
            if (String.IsNullOrEmpty(name))
                return View(new List<RssCategory>());

            var category = (from rc in db.RssCategories.Include("RssItems")
                            where rc.Name == name
                            select rc).FirstOrDefault();

            if (category == null)
                return View(new List<RssCategory>());

            ViewBag.CategoryName = name;
            var news = category.RssItems.Except(from d in db.DeletedNews
                                                where d.UserId == UserId
                                                join ri in db.RssItems on d.RssItemId equals ri.Id
                                                select ri).ToList();

            return View(news);
        }

        public ActionResult MyFavoriteNews()
        {
            var favoriteNews = (from fn in db.FavoriteNews
                                where fn.UserId == UserId
                                join ri in db.RssItems on fn.RssItemId equals ri.Id
                                select ri).ToList();
            if (favoriteNews == null)
                favoriteNews = new List<RssItem>();

            return View(favoriteNews);
        }

        public ActionResult ReadItLater()
        {
            var readingList = (from dn in db.DefferedNews
                               where dn.UserId == UserId
                               join ri in db.RssItems on dn.RssItemId equals ri.Id
                               select ri).Except(from d in db.DeletedNews
                                                 where d.UserId == UserId
                                                 join ri in db.RssItems on d.RssItemId equals ri.Id
                                                 select ri).ToList();

            return View(readingList);
        }

    }
}