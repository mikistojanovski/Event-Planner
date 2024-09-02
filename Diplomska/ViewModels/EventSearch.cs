using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class EventSearch
    {
        public IList<Event> Events { get; set; }
        public SelectList Genres { get; set; }
        public string? EventGenre { get; set; }
        public string? SearchString { get; set; }
    }
}
