using Diplomska.Models;

namespace Diplomska.ViewModels
{
    public class ListEvents
    {
        public IList<EventGuest> Connections { get; set; }
        public Guest Guest { get; set; }

    }
}
