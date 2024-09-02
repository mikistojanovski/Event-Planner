using Diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Diplomska.ViewModels
{
    public class HostSearch
    {
        public IList<Guest> Hosters { get; set; }
        public string? SearchString { get; set; }
    }
}
