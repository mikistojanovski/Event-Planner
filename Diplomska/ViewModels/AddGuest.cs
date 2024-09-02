using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class AddGuest
    {
        public Guest Hoster { get; set; }
        public IEnumerable<int>? selectedEvents { get; set; }
        public IEnumerable<SelectListItem>? EnrolledEvents { get; set; }
        public int?  interest{ get; set; }

    }
}
