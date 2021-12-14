using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PastorNub.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PastorNub
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected async void Application_Start()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var user = await userManager.FindByNameAsync("admin");
            var roleAdmin = await roleManager.FindByNameAsync("administrator");
            var roleUser = await roleManager.FindByNameAsync("user");
            var rolePastor = await roleManager.FindByNameAsync("pastor");

            if (roleUser == null)
            {
                var role1 = new IdentityRole { Name = "user" };

                roleManager.Create(role1);
            }
            if (rolePastor == null)
            {
                var role1 = new IdentityRole { Name = "pastor" };
                roleManager.Create(role1);
            }
 
            if (roleAdmin == null)
            {
                var role1 = new IdentityRole { Name = "administrator" };

                roleManager.Create(role1);
            }
            if (user == null)
            {
                var admin = new ApplicationUser { Email = "aquaitstep@gmail.com", UserName = "admin", Pastor = new Pastor() };
                string password = "Admin 11111";
                var result = userManager.Create(admin, password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(admin.Id, "administrator");
                    userManager.AddToRole(admin.Id, "pastor");
                }
            }

          


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
