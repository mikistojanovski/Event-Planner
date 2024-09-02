using Diplomska.Models;
using System.ComponentModel.DataAnnotations;

namespace Diplomska.ViewModels
{
    public class EditAsGuest
    {
        public EventGuest guestList { get; set; }//tvoi eventi
        public int? interest { get; set; }
        public bool? favorite { get; set; }
    }
}
