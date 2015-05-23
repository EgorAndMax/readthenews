using ReadTheNews.Helpers;
using ReadTheNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReadTheNews.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var dataHelper = new RssDataHelper(
                new RssChannel
                {
                    Title = "new",
                    Description = "qw",
                    ImageSrc = ".jpg",
                    Link = "url",
                    PubDate = DateTime.Now
                });

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}