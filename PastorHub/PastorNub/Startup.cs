using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using PastorNub.Models;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(PastorNub.Startup))]
namespace PastorNub
{
    public partial class Startup
    {
        private async void Init()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if(!await roleManager.RoleExistsAsync("pastor"))
            {
                var role = new IdentityRole { Name = "pastor" };
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("user"))
            {
                var role = new IdentityRole { Name = "user" };
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("administrator"))
            {
                var role = new IdentityRole { Name = "administrator" };
                await roleManager.CreateAsync(role);
            }
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var admin = await userManager.FindByNameAsync("Administrator"); 
            if(admin == null)
            {
                var Admin = new ApplicationUser() 
                { 
                    UserName = "Administrator",
                    Email = "admin@admin.admin",
                    Avatar = "standart.png"
                };

                var result = await userManager.CreateAsync(Admin, "Admin 9009009");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(Admin.Id, "pastor");
                    await userManager.AddToRoleAsync(Admin.Id, "administrator");
                }
            }
        }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Init();
        }
    }
}
