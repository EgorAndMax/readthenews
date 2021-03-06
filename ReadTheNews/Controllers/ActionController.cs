﻿using Microsoft.AspNet.Identity;
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
            try
            {
                db.SubscribedChannels.Add(new UserSubscribedChannels { RssChannelId = channelId, UserId = UserId });
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            return new EmptyResult();
        }

        public EmptyResult AddNewFavoriteRssNews(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            try
            {
                db.FavoriteNews.Add(
                new FavoriteNews
                {
                    RssItemId = rssItemId,
                    UserId = UserId
                });
           
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
             return new EmptyResult();
        }

        public EmptyResult DeleteNewsFromUserNewsList(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            try
            {
                db.DeletedNews.Add(
                new DeletedRssItemsByUser
                {
                    RssItemId = rssItemId,
                    UserId = UserId
                });
            
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            return new EmptyResult();
        }

        public EmptyResult ReadLaterThisNews(int? id)
        {
            if (id == null)
                return new EmptyResult();

            int rssItemId = Int32.Parse(id.ToString());
            try
            {
                db.DefferedNews.Add(
                new UserDefferedNews
                {
                    RssItemId = rssItemId,
                    UserId = UserId
                });
            
                db.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            return new EmptyResult();
        }
    }
}
