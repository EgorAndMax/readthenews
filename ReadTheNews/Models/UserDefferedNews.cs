using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class UserDefferedNews
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int RssItemId { get; set; }
    }
}