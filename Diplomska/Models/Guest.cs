using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Diplomska.Models
{
    public class Guest
    {
        public string Id { get; set; }

        [Remote(action: "IsNameInUse", controller: "Home")]
        [Display(Name = "Username")]
        public string? Username { get; set; }
        public string? Password { get; set; }

        [Remote(action: "IsEmailInUse", controller: "Home")]
        public string? Email { get; set; }
        public ICollection<EventGuest>? Events { get; set; }
        public ICollection<Event>? Hosting { get; set; }

        public string? user { get; set; }

    }
}
