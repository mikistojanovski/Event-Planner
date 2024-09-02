using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class GuestSearch
    {
        public Event? Event { get; set; }
        public IList<Guest> Guests { get; set; }
        public string? SearchString { get; set; }
    }
}
