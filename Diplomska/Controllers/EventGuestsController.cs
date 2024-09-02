using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Diplomska.Models;
using Diplomska.Data;

namespace Diplomska.Controllers
{
    public class EventGuestsController : Controller
    {
        private readonly DiplomskaContext _context;

        public EventGuestsController(DiplomskaContext context)
        {
            _context = context;
        }

        // GET: EventGuests
        public async Task<IActionResult> Index()
        {
            var diplomskaContext = _context.EventGuest.Include(e => e.Event).Include(e => e.Guest);
            return View(await diplomskaContext.ToListAsync());
        }

        // GET: EventGuests/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.EventGuest == null)
            {
                return NotFound();
            }

            var eventGuest = await _context.EventGuest
                .Include(e => e.Event)
                .Include(e => e.Guest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventGuest == null)
            {
                return NotFound();
            }

            return View(eventGuest);
        }

        // GET: EventGuests/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Set<Event>(), "Id", "Id");
            ViewData["GuestId"] = new SelectList(_context.Guest, "Id", "Id");
            return View();
        }

        // POST: EventGuests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GuestId,EventId,Interest")] EventGuest eventGuest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventGuest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Set<Event>(), "Id", "Id", eventGuest.EventId);
            ViewData["GuestId"] = new SelectList(_context.Guest, "Id", "Id", eventGuest.GuestId);
            return View(eventGuest);
        }

        // GET: EventGuests/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.EventGuest == null)
            {
                return NotFound();
            }

            var eventGuest = await _context.EventGuest.FindAsync(id);
            if (eventGuest == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Set<Event>(), "Id", "Id", eventGuest.EventId);
            ViewData["GuestId"] = new SelectList(_context.Guest, "Id", "Id", eventGuest.GuestId);
            return View(eventGuest);
        }

        // POST: EventGuests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,GuestId,EventId,Interest")] EventGuest eventGuest)
        {
            if (id != eventGuest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventGuest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventGuestExists(eventGuest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Set<Event>(), "Id", "Id", eventGuest.EventId);
            ViewData["GuestId"] = new SelectList(_context.Guest, "Id", "Id", eventGuest.GuestId);
            return View(eventGuest);
        }

        // GET: EventGuests/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.EventGuest == null)
            {
                return NotFound();
            }

            var eventGuest = await _context.EventGuest
                .Include(e => e.Event)
                .Include(e => e.Guest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventGuest == null)
            {
                return NotFound();
            }

            return View(eventGuest);
        }

        // POST: EventGuests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.EventGuest == null)
            {
                return Problem("Entity set 'DiplomskaContext.EventGuest'  is null.");
            }
            var eventGuest = await _context.EventGuest.FindAsync(id);
            if (eventGuest != null)
            {
                _context.EventGuest.Remove(eventGuest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventGuestExists(string id)
        {
          return (_context.EventGuest?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
