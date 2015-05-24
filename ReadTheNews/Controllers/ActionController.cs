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

        public EmptyResult AddNewFavoriteRssNews(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            db.FavoriteNews.Add(
                new UserFavoriteNews
                {
                    RssItemId = rssItemId,
                    UserId = UserId
                });
            db.SaveChanges();

            return new EmptyResult();
        }

        public EmptyResult DeleteNewsFromUserNewsList(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            db.DeletedNews.Add(
                new DeletedRssItemsByUser
                {
                    RssItemId = rssItemId,
                    UserId = _userId
                });
            db.SaveChanges();

            return new EmptyResult();
        }

        public EmptyResult ReadLaterThisNews(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            db.DefferedNews.Add(
                new UserDefferedNews
                {
                    RssItemId = rssItemId,
                    UserId = UserId
                });
            db.SaveChanges();

            return new EmptyResult();
        }
    }
}
