using Diplomska.Models;
using System.ComponentModel.DataAnnotations;

namespace Diplomska.ViewModels
{
    public class Edit_EventPicture
    {
        public Event? Event { get; set; }
        public string? Title { get; set; }

        [Display(Name = "Date")]
        public String? Date { get; set; }

        [Display(Name = "Time")]
        public String? Time { get; set; }

        public string? Genre { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        public string? Location { get; set; }

        [Display(Name = "Host")]
        public string? HostName { get; set; }
        public Guest? Hoster { get; set; }

        [Display(Name = "Poster")]
        public IFormFile? ProfileImage { get; set; }

        [Display(Name = "Title")]
        public string? Desc { get; set; }
    }
}
