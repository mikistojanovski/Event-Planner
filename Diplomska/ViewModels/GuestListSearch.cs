using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class GuestListSearch
    {
        public Event Event { get; set; }

        public IEnumerable<EventGuest>? enrolledGuests { get; set; }
        public IEnumerable<SelectListItem>? EnrolledStudents { get; set; }
        public string? SearchString { get; set; }
    }
}
