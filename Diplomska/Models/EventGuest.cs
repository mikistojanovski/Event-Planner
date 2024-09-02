using System.ComponentModel.DataAnnotations;

namespace Diplomska.Models
{
    public class EventGuest
    {
        public string Id { get; set; }

        [Required]
        public string GuestId { get; set; }
        public Guest? Guest { get; set; }

        [Required]
        public string EventId { get; set; }
        public Event? Event { get; set; }
        
        public int? Interest { get; set; }

    }
}
