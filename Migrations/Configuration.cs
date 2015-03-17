namespace Blog.Migrations
{
    using Blog.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //ContextKey = "Blog.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser user = new ApplicationUser()
            {
                FirstName="Admin",
                LastName="Admin",
                UserName="Exemple@exemple.com",
                Email = "Exemple@exemple.com"

            };
            userManager.Create(user,"Password1_" );

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            roleManager.Create(new IdentityRole("Admin"));
            ApplicationUser dbUser = userManager.FindByEmail("Exemple@exemple.com");
            userManager.AddToRole(dbUser.Id, "Admin");


          
        }
    }
}
