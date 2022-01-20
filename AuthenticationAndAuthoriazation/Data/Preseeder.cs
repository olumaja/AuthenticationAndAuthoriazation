using AuthenticationAndAuthoriazation.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAndAuthoriazation.Data
{
    public static class Preseeder
    {
        public static async Task Seeder(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDBContext>();
            var userManager = serviceProvider.GetService<UserManager<Employee>>();
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            if (!context.Roles.Any())
            {
                //var roles = new List<IdentityRole> {
                //    new IdentityRole("Junior Staff"),
                //    new IdentityRole("Suppervisor"),
                //    new IdentityRole("Manager"),
                //    new IdentityRole("Admin")
                //};
                var roles = new List<string> { "Junior Staff", "Suppervisor", "Manager", "Admin" };
                //var claimList = new List<string> { "CanEdit", "CanDelete", "Update" };

                foreach (var item in roles)
                {
                   var role = new IdentityRole(item);
                   await roleManager.CreateAsync(role);
                }
            };

            //if (!context.Claims.Any())
            //{

            //}

            if (!userManager.Users.Any())
            {
                var user = new Employee
                {
                    FirstName = "Segun",
                    LastName = "Maja",
                    UserName = "oluenoch84@gmail.com",
                    Email = "oluenoch84@gmail.com",
                    Photo = "maleAvarter.jpg",
                    Gender = "Male"
                };

                var result = await userManager.CreateAsync(user, "Ayomaja84@");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    //await _userManager.AddClaimAsync(userToAdd, new Claim("CanEdit", "true"));
                }

            }

        }
    }
}
