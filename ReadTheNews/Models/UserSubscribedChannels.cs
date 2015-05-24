using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class UserSubscribedChannels
    {
        [Required]
        public int RssChannelId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}