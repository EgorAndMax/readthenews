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
            ViewBag.RssChannels = db.RssChannels.ToList();
            ViewBag.CountsCategories = GetCountsCategories();

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

            return Redirect(@"Channel/" + processor.CurrentChannel.Id);
        }

        public ActionResult Channel(int? id)
        {
            if (id == null)
                return Redirect("Channels");

            int channelId = Int32.Parse(id.ToString());
            var channel = db.RssChannels.Find(channelId);
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
            ViewBag.CountsCategories = GetCountsCategories();
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
                                   join channel in db.RssChannels on item.RssChannelId equals channel.Id
                                   join sc in db.SubscribedChannels on channel.Id equals sc.RssChannelId
                                   where sc.UserId == UserId
                                   join d in db.DeletedNews on item.Id equals d.RssItemId
                                   where d.UserId == UserId
                                   select item).ToList();

            // TODO: реализовать фильтр по подписанным каналам
            ViewBag.CountsCategories = GetCountsCategories();

            return View(myNews);
        }

        public ActionResult MyChannels()
        {
            var myChannels = (from c in db.RssChannels
                              join sc in db.SubscribedChannels on c.Id equals sc.RssChannelId
                              where sc.UserId == UserId
                              select new ChannelNewsCount
                              {
                                  Id = c.Id,
                                  Title = c.Title,
                                  Count = (from n in db.RssItems
                                           where n.RssChannelId == c.Id
                                           select n.Id).Count()
                              }).ToList();

            // TODO: реализовать фильтр по подписанным каналам
            ViewBag.CountsCategories = GetCountsCategories();

            return View(myChannels);
        }

        public ActionResult GetNewsByCategory(string name)
        {
            if (String.IsNullOrEmpty(name))
                return View(new List<RssItem>());

            var category = (from rc in db.RssCategories.Include("RssItems")
                            where rc.Name == name
                            select rc).FirstOrDefault();

            if (category == null)
                return View(new List<RssItem>());

            ViewBag.CategoryName = name;
            var news = category.RssItems.Except(from d in db.DeletedNews
                                                where d.UserId == UserId
                                                join ri in db.RssItems on d.RssItemId equals ri.Id
                                                select ri).ToList();

            ViewBag.CountsCategories = GetCountsCategories();

            return View(news);
        }

        public ActionResult MyFavoriteNews()
        {
            var favoriteNews = (from fn in db.FavoriteNews
                                where fn.UserId == UserId
                                join ri in db.RssItems on fn.RssItemId equals ri.Id
                                select ri).Except(from d in db.DeletedNews
                                                  where d.UserId == UserId
                                                  join ri in db.RssItems on d.RssItemId equals ri.Id
                                                  select ri).ToList();

            ViewBag.CountsCategories = GetCountsCategories();

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

            ViewBag.CountsCategories = GetCountsCategories();

            return View(readingList);
        }

        private List<CountNewsOfCategory> GetCountsCategories() =>
            (from rc in db.RssCategories.Include("RssItems")
             let userId = UserId
             let deletedNews = (from d in db.DeletedNews
                                where d.UserId == userId
                                join ri in db.RssItems on d.RssItemId equals ri.Id
                                join fn in db.FavoriteNews on ri.Id equals fn.RssItemId
                                join defn in db.DefferedNews on ri.Id equals defn.RssItemId
                                select ri)
             select new CountNewsOfCategory
             {
                 Name = rc.Name,
                 Count = rc.RssItems.Except(deletedNews).Count()
             }).Distinct().OrderByDescending(cnc => cnc.Count).Take(20).ToList();
    }
}