using Diplomska.Areas.Identity.Data;
using Diplomska.Data;
using Diplomska.Models;
using Diplomska.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Diplomska.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DiplomskaContext _context;
        private readonly UserManager<DiplomskaUser> _userManager;
        public HomeController(ILogger<HomeController> logger, DiplomskaContext context, UserManager<DiplomskaUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AllUsers()
        {
            var users = _userManager.Users.Where(x => !x.Email.Contains("admin"));
            return View(users);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }
            var model = new EditUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
            };
            return View(model);
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(EditUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{model.Id}'.");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("AllUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Name {email} is already in use.");
            }
        }
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsNameInUse(string email)
        {
            var user = await _userManager.FindByNameAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Name {email} is already in use.");
            }
        }

    }
}
