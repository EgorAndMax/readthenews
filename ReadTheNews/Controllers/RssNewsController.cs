using ReadTheNews.Helpers;
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
                return Redirect("RssNews/Channels");

            int channelId = Int32.Parse(id.ToString());
            RssChannel channel = db.RssChannels.Find(channelId);
            RssProcessor processor;
            try
            {
                processor = new RssProcessor(channel);
                processor.GetLatestNews();
                /*
                using (var dataHelper = new RssDataHelper())
                {
                    var counts = dataHelper.GetCountsCategoriesOfRssChannel(channelId, _userId);
                    if (counts == null)
                        throw new Exception("Категории новостей канала не были загружены");
                    ViewBag.CountsCategoriesOfRssChannel = counts;
                    ViewBag.IsSubscribe = dataHelper.IsUserSubcribeOnRssChannel(channelId);
                }
                */
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Error");
            }
            return View(channel);
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}