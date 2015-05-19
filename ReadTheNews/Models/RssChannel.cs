using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadTheNews.Models
{
    public class RssChannel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Заголовок")]
        [MaxLength(120)]
        public string Title { get; set; }

        [Display(Name = "Язык канала")]
        [MaxLength(10)]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Ссылка")]
        [MaxLength(150)]
        public string Link { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Адрес изображения")]
        [MaxLength(150)]
        public string ImageSrc { get; set; }

        [Required]
        [Display(Name = "Дата публикации")]
        [DataType(DataType.DateTime)]
        public DateTime PubDate { get; set; }

        public virtual ICollection<RssItem> RssItems { get; set; }

        public RssChannel()
        {
            RssItems = new List<RssItem>();
        }
    }

    [NotMapped]
    public class ChannelNewsCount
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
    }
}
