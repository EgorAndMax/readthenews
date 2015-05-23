using System.ComponentModel.DataAnnotations;

namespace ReadTheNews.Models
{
    public class DeletedRssItemsByUser
    {
        [Required]
        public int RssItemId { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}