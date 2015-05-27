using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class RssItemRssCategories
    {
        [Required]
        public int RssItemId { get; set; }
        public virtual RssItem RssItem { get; set; }

        [Required]
        public int RssCategoryId { get; set; }
        public virtual RssCategory RssCategory { get; set; }
    }
}