using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class UserFavoriteNews
    {
        [Required]
        public int RssItemId { get; set; }
        public virtual RssItem RssItem { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}