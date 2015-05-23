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
        private RssChannel _CurrentChannel;
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
            _CurrentChannel = channel;
        }


        public void GetLatestNews()
        {
            if (!this.IsChannelDownload)
                throw new ChannelNotDownloadException();

            if (_CurrentChannel == null)
                GetRssChannel();

            var latestUpdate = new DateTime();
            if (_CurrentChannel.PubDate != new DateTime())
                latestUpdate = _CurrentChannel.PubDate;
            else
                latestUpdate = _CurrentChannel.RssItems.First() != null ?
                    _CurrentChannel.RssItems.First().Date : new DateTime();

            if (_CurrentChannel.PubDate == latestUpdate && _CurrentChannel.RssItems.Count > 0)
                return;

            // загрузка списка новостей
            GetRssItemList();
        }

        private void GetRssItemList()
        {
            if (_Channel == null || _CurrentChannel == null)
                throw new ChannelNotDownloadException();
            if (!CheckNewContent(_Channel.Items.First()) && _CurrentChannel.RssItems.Count > 0)
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

            _CurrentChannel.PubDate = _Channel.LastUpdatedTime.DateTime != new DateTime() ?
                _Channel.LastUpdatedTime.DateTime :
                    _Channel.Items.FirstOrDefault() != null ?
                        _Channel.Items.FirstOrDefault().PublishDate.DateTime : DateTime.Now;

            _DataHelper.UpdateRssChannel(_CurrentChannel);

            _DataHelper.SumbitChanges();
        }

        private void GetRssChannel()
        {
            if (!this.IsChannelDownload)
                throw new ChannelNotDownloadException();

            _CurrentChannel = Parser.ParseChannel(_Channel, _RssChannelUrl);
            _DataHelper = new RssDataHelper(_CurrentChannel);
        }

        private RssItem GetRssItem(SyndicationItem item)
        {
            RssItem newRssItem = Parser.ParseItem(item, _CurrentChannel);

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