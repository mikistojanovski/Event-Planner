using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class EnrollGuests
    {
        public Event Event { get; set; }
        public IEnumerable<string>? selectedStudents { get; set; }
        public IEnumerable<SelectListItem>? EnrollGuest { get; set; }
        public int? Interest { get; set; }
        public int? Favorite { get; set; }

    }
}
