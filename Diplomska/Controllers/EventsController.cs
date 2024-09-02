using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplomska.Models;
using Diplomska.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Diplomska.ViewModels;
using System.Security.Claims;
using Diplomska.Data;

namespace Diplomska.Controllers
{
    public class EventsController : Controller
    {
        private readonly DiplomskaContext _context;
        private readonly UserManager<DiplomskaUser> _userManager;
        private readonly SignInManager<DiplomskaUser> _signInManager;


        public EventsController(DiplomskaContext context, UserManager<DiplomskaUser> UserManager, SignInManager<DiplomskaUser> SignInManager)
        {
            _userManager = UserManager;
            _signInManager = SignInManager;
            _context = context;
        }


        static string authSecret = "Vf2xi4idcUd9yD7YfnrCiAokIY3oe2pUdjYRpVN3";
        static string basePath = "https://diplomska-11f45-default-rtdb.europe-west1.firebasedatabase.app/";

        IFirebaseClient client;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = authSecret,
            BasePath = basePath
        };

        // GET: Events
        public async Task<IActionResult> Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Event>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Event>(((JProperty)item).Value.ToString()));
            }
            IQueryable<Event> movies = list.AsQueryable();
            DateTime currentDateTime = DateTime.Now;
            var enum2 = from user in movies
                        orderby user.Date
                        select user;
            var e = new List<Event>();
            var movieGenreVM = new EventSearch
            {
                Events = enum2.ToList(),
            };
            return View(movieGenreVM);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(string? id, Edit_EventPicture @event)
        {
            if (id == null)
            {
                return NotFound();
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);
            @event.Event = data;
            @event.Desc = data.Poster;
            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Event>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Event>(((JProperty)item).Value.ToString()));
            }
            ViewData["HostName"] = new SelectList(list, "HostName", "Username");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Edit_EventPicture @event)
        {
            string uniqueFileName = null;
            var hoster = User.FindFirstValue(ClaimTypes.Name);
            if (@event.ProfileImage != null)
            {
                uniqueFileName = UploadedFile(@event);
            }
            else
            {
                @event.Desc = uniqueFileName;
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response1 = client.Get("guests/");
            dynamic data1 = JsonConvert.DeserializeObject<dynamic>(response1.Body);
            var list1 = new List<Guest>();
            foreach (var item in data1)
            {
                list1.Add(JsonConvert.DeserializeObject<Guest>(((JProperty)item).Value.ToString()));
            }
            Guest k = list1.Where(m => m.Username == hoster).First();
            Event newEmployee = new Event
            {
                Title = @event.Title,
                Date = @event.Date,
                Time = @event.Time,
                Genre = @event.Genre,
                Price = @event.Price,
                Location = @event.Location,
                Hoster = k,
                HostName = hoster,
                Poster = uniqueFileName
            };
            try
            {
                AddToFirebase(newEmployee);
                AddInterestToFirebase(newEmployee);
                ModelState.AddModelError(string.Empty, "ADded");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }


            return RedirectToAction("Details", new { id = newEmployee.Id });
            return View(@event);
        }

        private void AddInterestToFirebase(Event newEmployee)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = new EventGuest
            {
                Event = newEmployee,
                EventId = newEmployee.Id,
                Guest = newEmployee.Hoster,
                GuestId = newEmployee.Hoster.Username,
            Interest=1
        };
            PushResponse response = client.Push("eventguests/", data);
        }

        private void AddToFirebase(Event @event)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = @event;
            PushResponse response = client.Push("events/", data);
            data.Id = response.Result.name;
            SetResponse setResponse = client.Set("events/" + data.Id, data);

        }
        private string UploadedFile(Edit_EventPicture viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfileImage.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfileImage.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);
            Edit_EventPicture viewmodel = new Edit_EventPicture
            {
                Event = data,
                Desc = data.Poster
            };
            return View(viewmodel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Edit_EventPicture viewmodel)
        {
            string uniqueFileName = null;
            if (id != viewmodel.Event.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfileImage != null)
                    {
                        uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Event.Poster = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Event.Poster = viewmodel.Desc;
                    }
                    client = new FireSharp.FirebaseClient(config);
                    SetResponse response = client.Set("events/" + viewmodel.Event.Id, viewmodel.Event);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(viewmodel.Event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = viewmodel.Event.Id });

            }
            return View(viewmodel);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);

            if (data == null)
            {
                return NotFound();
            }
            return View(data);
            return RedirectToAction("Index");
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("events/" + id);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);
            return (data.Id == id);
        }

        //GET Events/EventsHosting
        public async Task<IActionResult> EventsHosting()
        {
            var id = User.FindFirstValue(ClaimTypes.Name);
            if (id == null)
            {
                return NotFound();
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);

            var list = new List<Event>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Event>(((JProperty)item).Value.ToString()));
            }
            List<Event> coursesQuery = list.FindAll(M => M.HostName == id);
            ViewBag.Message = id;
            await _context.SaveChangesAsync();
            if (id == null)
            {
                return NotFound();
            }
            var CourseTitleVM = new EventSearch
            {
                Events = coursesQuery.ToList(),
            };
            return View(CourseTitleVM);
        }

        //GET Events/Guest_Joining   GUESTLIST
        public async Task<IActionResult> Guest_Joining(string? id, string? searchString)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response1 = client.Get("eventguests");
            dynamic data1 = JsonConvert.DeserializeObject<dynamic>(response1.Body);

            var list1 = new List<EventGuest>();
            if (data1 != null)
            {
                foreach (var item in data1)
                {
                    list1.Add(JsonConvert.DeserializeObject<EventGuest>(((JProperty)item).Value.ToString()));
                }
            }
            IQueryable<EventGuest> guestlist = list1.AsQueryable();
            guestlist = guestlist.Include(c => c.Guest).Include(c => c.Event);
            guestlist = guestlist.Where(s => s.EventId == id);

            var guestscoming = new List<Guest>();
            foreach (var item in guestlist)
            {
                guestscoming.Add(item.Guest);
            }
            IQueryable<Guest> guests = guestscoming.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                guests=guests.Where(m=>m.Username ==searchString);
            }

            FirebaseResponse response2 = client.Get("events");
            dynamic data2 = JsonConvert.DeserializeObject<dynamic>(response2.Body);
            var list2 = new List<Event>();
            if (data2 != null)
            {
                foreach (var item in data2)
                {
                    list2.Add(JsonConvert.DeserializeObject<Event>(((JProperty)item).Value.ToString()));
                }
            }
            ViewData["Event"] = list2.Where(s => s.Id == id).Select(s => s.Title).FirstOrDefault();
            Event k = list2.Where(s => s.Id == id).FirstOrDefault();
            var viewmodel = new GuestSearch
            {
                Event = k,
                Guests = guests.ToList(),
            };
            return View(viewmodel);
        }


        public async Task<IActionResult> EditPicture(string? id)
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);
            Edit_EventPicture viewmodel = new Edit_EventPicture
            {
                Event = data,
                Desc = data.Title
            };
            return View(viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPicture(string id, Edit_EventPicture viewmodel)
        {
            client = new FireSharp.FirebaseClient(config);
            if (id != viewmodel.Event.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfileImage != null)

                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Event.Poster = uniqueFileName;

                    }
                    else
                    {
                        viewmodel.Event.Poster = viewmodel.Desc;

                    }
                    SetResponse response = client.Set("events/" + viewmodel.Event.Id, viewmodel.Event);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(viewmodel.Event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Details", new { id = viewmodel.Event.Id });
            }
            return View(viewmodel);
        }

        public async Task<IActionResult> Going(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("events/" + id);
            Event data = JsonConvert.DeserializeObject<Event>(response.Body);
            if (data == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.Name);
            FirebaseResponse response1 = client.Get("guests/");
            dynamic data1 = JsonConvert.DeserializeObject<dynamic>(response1.Body);
            var list = new List<Guest>();
            foreach (var item in data1)
            {
                list.Add(JsonConvert.DeserializeObject<Guest>(((JProperty)item).Value.ToString()));
            }
            Guest coursesQuery = list.AsQueryable().Where(m => m.Username == userId).First();
            FirebaseResponse response3 = client.Get("eventguests/");
            dynamic data3 = JsonConvert.DeserializeObject<dynamic>(response3.Body);
            var list3 = new List<EventGuest>();
            if (data3 != null) {
                foreach (var item in data3)
                {
                    list3.Add(JsonConvert.DeserializeObject<EventGuest>(((JProperty)item).Value.ToString()));
                }
            }
            AddInterest viewmodel = new AddInterest
            {
                Event = data,
                EventId = data.Id,
                Guest = coursesQuery,
                GuestId = coursesQuery.Username,
            };
            foreach (var item in list3)
            {
                if (item.EventId == data.Id && item.GuestId == coursesQuery.Username)
                {
                    viewmodel.Event = item.Event;
                    viewmodel.Id = item.Id;
                    viewmodel.EventId = item.EventId;
                    viewmodel.Guest = item.Guest;
                    viewmodel.GuestId = item.GuestId;
                    if (item.Interest != null)
                    { 
                        viewmodel.Interest = item.Interest;
                    }
                    return View(viewmodel);
                }
            }



            PushResponse r = client.Push("eventguests/", viewmodel);
            viewmodel.Id = r.Result.name;
            SetResponse setResponse = client.Set("eventguests/" + viewmodel.Id, viewmodel);
            await _context.SaveChangesAsync();
            return View(viewmodel);

            return RedirectToAction(nameof(Index));
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Going(string? id,AddInterest k)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.Name);
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response1 = client.Get("guests/");
            dynamic data1 = JsonConvert.DeserializeObject<dynamic>(response1.Body);
            var list = new List<Guest>();
            foreach (var item in data1)
            {
                list.Add(JsonConvert.DeserializeObject<Guest>(((JProperty)item).Value.ToString()));
            }
            Guest coursesQuery = list.AsQueryable().Where(m => m.Username == userId).First();
            FirebaseResponse response3 = client.Get("eventguests/");
            dynamic data3 = JsonConvert.DeserializeObject<dynamic>(response3.Body);
            var list3 = new List<EventGuest>();
            if (data3 != null)
            {
                foreach (var item in data3)
                {
                    list3.Add(JsonConvert.DeserializeObject<EventGuest>(((JProperty)item).Value.ToString()));
                }
            }
            foreach (var item in list3)
            {
                if (item.EventId == id && item.GuestId == coursesQuery.Username)
                {
                    var m1 = item;
                    if (m1.Interest == null)
                    {
                        m1.Interest = 1;
                    }
                    else if (m1.Interest == 0)
                    {
                        m1.Interest = 1;
                    }
                    else if (m1.Interest == 1)
                    {
                        m1.Interest = 0;
                    }
                    SetResponse changeIInterest = client.Set("eventguests/" + m1.Id, m1);
                    return RedirectToAction(nameof(Index)); 
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EventsJoined(string? id, string? userID)
        {
            client = new FireSharp.FirebaseClient(config);

            FirebaseResponse response = client.Get("guests");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Guest>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Guest>(((JProperty)item).Value.ToString()));
            }
            if (userID != null)
            {
                var stu = list.FirstOrDefault(x => x.Username == userID);
                id = (string?)stu.Id;
            }
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.student = userID;
            FirebaseResponse response1 = client.Get("eventguests");
            dynamic data1 = JsonConvert.DeserializeObject<dynamic>(response1.Body);
            var list1 = new List<EventGuest>();
            var enrollment = new List<EventGuest>();

            if (data1 != null)
            {
                foreach (var item in data1)
                {
                    list1.Add(JsonConvert.DeserializeObject<EventGuest>(((JProperty)item).Value.ToString()));
                }
                foreach (var item in list1)
                {
                    if (item.GuestId == userID&&item.Interest==1)
                        enrollment.Add(item);
                }
            }
            List<Event> z = new List<Event>();
            foreach (var item in enrollment)
            {
                z.Add(item.Event);
            }
            var search = new EventSearch
            {
                Events = z
            };
            return View(search);
        }



    }
}
