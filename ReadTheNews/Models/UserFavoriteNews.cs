using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class FavoriteNews
    {
        [Required]
        public int RssItemId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}