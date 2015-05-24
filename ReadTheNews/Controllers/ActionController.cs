using Microsoft.AspNet.Identity;
using ReadTheNews.Models;
using System;
using System.Web.Mvc;

namespace ReadTheNews.Controllers
{
    public class ActionController : Controller, IDisposable
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

        public EmptyResult SubscribeOnChannel(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int channelId = Int32.Parse(id.ToString());
            db.SubscribedChannels.Add(new UserSubscribedChannels { RssChannelId = channelId, UserId = UserId });
            db.SaveChanges();

            return new EmptyResult();
        }
    }
}
