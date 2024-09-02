// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Diplomska.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using Diplomska.Models;
using Firebase.Auth;
using Newtonsoft.Json;

namespace Diplomska.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<DiplomskaUser> _signInManager;
        private readonly UserManager<DiplomskaUser> _userManager;
        private readonly IUserStore<DiplomskaUser> _userStore;
        private readonly IUserEmailStore<DiplomskaUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<DiplomskaUser> userManager,
            IUserStore<DiplomskaUser> userStore,
            SignInManager<DiplomskaUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        static string authSecret = "Vf2xi4idcUd9yD7YfnrCiAokIY3oe2pUdjYRpVN3";
        static string basePath = "https://diplomska-11f45-default-rtdb.europe-west1.firebasedatabase.app/";

        IFirebaseClient client;

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            AuthSecret = authSecret,
            BasePath = basePath
        };
        FirebaseAuthProvider auth;


        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            auth = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyA1pRiT45NVTY0s584bC0oajSoTXd26s2U"));
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded) {
                    Reg(Input.Email,Input.Password);
                }


                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Guest");

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    Guest guest = new Guest
                    {
                        Id = user.Id,
                        Username = Input.UserName,
                        Email = Input.Email,

                    };
                    try
                    {
                        AddToFirebaseU(user);
                        AddToFirebaseG(guest,Input.Password);

                        ModelState.AddModelError(string.Empty, "ADded");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    return RedirectToAction("index", "home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async void Reg(String Email,String Password)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(Email, Password);
                //log in the new user

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                
            }

            return;
        }

        private void AddToFirebaseG(Guest guest,String password)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = guest;
            PushResponse response = client.Push("guests/", data);
            SetResponse setResponse = client.Set("guests/" + data.Id, data);
            FirebaseResponse responseDEL = client.Delete("guests/" + response.Result.name);
        }

        private void AddToFirebaseU(DiplomskaUser user)
        {
                client = new FireSharp.FirebaseClient(config);
                var data = user;
                PushResponse response = client.Push("users/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("users/" + data.Id, data);
        }

        private DiplomskaUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<DiplomskaUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(DiplomskaUser)}'. " +
                    $"Ensure that '{nameof(DiplomskaUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<DiplomskaUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<DiplomskaUser>)_userStore;
        }
    }
}
