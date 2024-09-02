using Diplomska.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Diplomska.Data;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Firebase.Auth;
namespace Diplomska.Models
    {
        public class SeedData
        {

        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            string authSecret = "Vf2xi4idcUd9yD7YfnrCiAokIY3oe2pUdjYRpVN3";
            string basePath = "https://diplomska-11f45-default-rtdb.europe-west1.firebasedatabase.app/";

            IFirebaseClient client;

            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = authSecret,
                BasePath = basePath
            };


            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<DiplomskaUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            DiplomskaUser user = await UserManager.FindByEmailAsync("admin@event.com");
            if (user == null)
            {
                var User = new DiplomskaUser();
                User.Email = "admin@event.com";
                User.UserName = "admin@event.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            // creating Client role     
            var x = await RoleManager.RoleExistsAsync("Guest");
            if (!x)
            {
                var role = new IdentityRole();
                role.Name = "Guest";
                await RoleManager.CreateAsync(role);
            }
            // creating Host role     
            var y = await RoleManager.RoleExistsAsync("Hoster");
            if (!y)
            {
                var role = new IdentityRole();
                role.Name = "Hoster";
                await RoleManager.CreateAsync(role);
            }


            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("guests");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data != null) { 
            var list = new List<Guest>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Guest>(((JProperty)item).Value.ToString()));
            }
            foreach (var item in list)
            {
                if (item.Email != null) { 
                DiplomskaUser user1 = await UserManager.FindByEmailAsync(item.Email);
                if (user1 == null)
                {
                    var User = new DiplomskaUser();
                    User.Email = item.Email;
                    User.UserName = item.Username;
                    IdentityResult chkUser = await UserManager.CreateAsync(User, item.Password);
                    }
                }             
            }
            }

        }


        public static void Initialize(IServiceProvider serviceProvider)
            {

            using (var context = new DiplomskaContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<DiplomskaContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();


                if (context.Event.Any() || context.Guest.Any() || context.EventGuest.Any())
                {
                    return; // DB has been seeded
                }
            }
        }
        }
    }

