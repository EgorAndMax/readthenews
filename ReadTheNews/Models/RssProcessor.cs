using ReadTheNews.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Parser = ReadTheNews.Helpers.RssParseHelper;

namespace ReadTheNews.Models
{
    public class RssProcessor
    {
        public bool IsChannelDownload { get; private set; }

        private SyndicationFeed _Channel;
        public RssChannel CurrentChannel { get; private set; }
        private string _RssChannelUrl;
        private RssNewsContext db;
        private bool _IsNewContent;
        private RssDataHelper _DataHelper;
        private RssItem _lastRssItemInDb;

        private RssProcessor()
        {
            IsChannelDownload = false;
            db = new RssNewsContext();
        }

        public RssProcessor(string url) : this()
        {
            try
            {
                var feedReader = XmlReader.Create(url);
                _Channel = SyndicationFeed.Load(feedReader);
                IsChannelDownload = true;
                _RssChannelUrl = url;
            }
            catch (Exception ex) // уточнить исключение
            {
                IsChannelDownload = false;
            }
        }

        public RssProcessor(RssChannel channel) : this(channel.Link)
        {
            CurrentChannel = channel;
        }


        public void GetLatestNews()
        {
            if (!this.IsChannelDownload)
                throw new ChannelNotDownloadException();

            if (CurrentChannel == null)
                GetRssChannel();

            var latestUpdate = new DateTime();
            if (CurrentChannel.PubDate != new DateTime())
                latestUpdate = CurrentChannel.PubDate;
            else
                latestUpdate = CurrentChannel.RssItems.First() != null ?
                    CurrentChannel.RssItems.First().Date : new DateTime();

            if (CurrentChannel.PubDate == latestUpdate && CurrentChannel.RssItems.Count > 0)
                return;

            // загрузка списка новостей
            GetRssItemList();
        }

        private void GetRssItemList()
        {
            if (_Channel == null || CurrentChannel == null)
                throw new ChannelNotDownloadException();
            if (!CheckNewContent(_Channel.Items.First()) && CurrentChannel.RssItems.Count > 0)
                return;

            DateTime yesterday = DateTime.Today.AddDays(-1).Date;
            foreach (SyndicationItem item in _Channel.Items)
            {
                if (_lastRssItemInDb != null && item.Title.Text == _lastRssItemInDb.Title)
                    break;

                if (item.PublishDate < yesterday)
                    break;

                GetRssItem(item);
            }

            CurrentChannel.PubDate = _Channel.LastUpdatedTime.DateTime != new DateTime() ?
                _Channel.LastUpdatedTime.DateTime :
                    _Channel.Items.FirstOrDefault() != null ?
                        _Channel.Items.FirstOrDefault().PublishDate.DateTime : DateTime.Now;

            _DataHelper.UpdateRssChannel(CurrentChannel);

            _DataHelper.SumbitChanges();
        }

        private void GetRssChannel()
        {
            if (!this.IsChannelDownload)
                throw new ChannelNotDownloadException();

            CurrentChannel = Parser.ParseChannel(_Channel, _RssChannelUrl);
            _DataHelper = new RssDataHelper(CurrentChannel);
        }

        private RssItem GetRssItem(SyndicationItem item)
        {
            RssItem newRssItem = Parser.ParseItem(item, CurrentChannel);

            _DataHelper.AddRssItem(newRssItem);

            return newRssItem;
        }

        private bool CheckNewContent(SyndicationItem item)
        {
            if (item == null)
            {
                _IsNewContent = false;
                return _IsNewContent;
            }

            _lastRssItemInDb = _DataHelper.GetLastRssItemFromDb();
            if (_lastRssItemInDb != null && _lastRssItemInDb.Title == item.Title.Text)
            {
                _IsNewContent = false;
                return _IsNewContent;
            }
            _IsNewContent = true;

            return _IsNewContent;
        }
    }

    public class ChannelNotDownloadException : Exception
    {
        public ChannelNotDownloadException() : base("Rss-канал не был загружен") { }
    }
}