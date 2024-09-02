using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Diplomska.Models
{
    public class Event
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Display(Name = "Date")]
        public String? Date { get; set; }

        [Display(Name = "Time")]
        public String? Time { get; set; }
       
        public string? Genre { get; set; }

        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Display(Name = "Host")]
        public string? HostName { get; set; }
        public Guest? Hoster { get; set; }
        public string? Location { get; set; }

        public string? Poster { get; set; }
        public ICollection<EventGuest>? Guests { get; set; }
    }
}
