using Microsoft.AspNet.Identity;
using ReadTheNews.Models;
using System;
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
            try
            {
                ViewBag.RssChannels = db.RssChannels.ToList();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToRoute(new { controller = "RssNews", action = "Error" });
            }
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
            ViewBag.CountsCategories = (from rc in db.RssCategories.Include("RssItems")
                                        orderby rc.RssItems.Count() descending
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = rc.RssItems.Count()
                                        }).Take(20).ToList();

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
                          ).ToList();

            // TODO: реализовать фильтр по подписанным каналам
            ViewBag.CountsCategories = (from rc in db.RssCategories.Include("RssItems")
                                        orderby rc.RssItems.Count() descending
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = rc.RssItems.Count()
                                        }).Take(20).ToList();

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
            ViewBag.CountsCategories = (from rc in db.RssCategories.Include("RssItems")
                                        orderby rc.RssItems.Count() descending
                                        select new CountNewsOfCategory
                                        {
                                            Name = rc.Name,
                                            Count = rc.RssItems.Count()
                                        }).Take(20).ToList();

            return View(myChannels);
        }
    }
}