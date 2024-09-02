using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class AddInterest
    {
        public string Id { get; set; }
        public Guest Guest { get; set; }
        public Event Event { get; set; }

        public string GuestId { get; set; }
        public string EventId { get; set; }
        public int? Interest { get; set; }
    }
}
