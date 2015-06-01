using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReadTheNews.Models;
using System.Reflection;
using Microsoft.AspNet.Identity;

namespace ReadTheNews.Controllers
{
    public class ReportsController : Controller
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
            return View();
        }

        public RedirectToRouteResult CreateDocument()
        { 
            string type = Request.Params["type_doc"];
            string name = @"E:\Projects\read_the_news\ReadTheNews\Content\Reports\Report." + type;

            Report report = new Report();
            try
            {
                switch (type)
                {
                    case "xls":
                        report.CreateExcel(name, db, UserId);
                        break;
                    case "ods":
                        report.CreateCalc(name, db, UserId);
                        break;
                    case "pdf":
                        DateTime start = DateTime.Parse(Request.Params["start_date"]);
                        DateTime finish = DateTime.Parse(Request.Params["finish_date"]);
                        report.CreatePdf(name, start, finish, db, UserId);
                        break;
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }

            return RedirectToAction("Download", new { path = name , type = type});

        }

        public FileStreamResult Download(string path, string type)
        {
            FileInfo info = new FileInfo(path);
            return File(info.OpenRead(), "text/plain", "Report." + type);
        }

        public ContentResult Error(string message)
        {
            return Content("<h1>Ошибка скачивания</h1><h2>" + message + "</h2>");
        }

    }
}