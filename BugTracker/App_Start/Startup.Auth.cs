using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using BugTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin.Security.Providers.LinkedIn;


namespace BugTracker
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");
            app.UseLinkedInAuthentication("772bf9jm6hnj2a", "gyLmcfeFXPvYEOwi");


            app.UseFacebookAuthentication(
               appId: "797028353691072",
               appSecret: "8288e1bc75b991b56f96fadf56ca0c31");


            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "449310710709-gvhl0c5qfvkcc1utmqdjt4prkv1jbbus.apps.googleusercontent.com",
                ClientSecret = "nqVJY9W6Cb5cU2hqTiJLkOfj"
            });


            // seed the admin role and user. 
            Seed(new ApplicationDbContext());
        }


        protected void Seed(ApplicationDbContext context)
        {
            // make Mangers for Roles and User stuff
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // action to help Create Roles
            Action<string> SeedRole = role =>
            {
                if (!roleManager.RoleExists(role)) 
                    roleManager.Create(new IdentityRole(role));
            };



            // Create Roles
            SeedRole("Administrator");
            SeedRole("Developer");

            
            // function to easily and safely seed users.
            Action<string, string, string, string> SeedUser = (name, role, email, pass) =>
                {
                    // find the user.
                    var user = userManager.FindByName(name);

                    // if no user then make it.
                    if (user == null)
                    {
                        // create user
                        user = new ApplicationUser { UserName = name, Email = email };
                        var result = userManager.Create(user, pass);

                        if (result.Succeeded && role != "")
                            userManager.AddToRole(user.Id, role);
                    }
                    else
                    {
                        // if user isn't in the given role, add him.
                        if (!userManager.IsInRole(user.Id, role))
                            userManager.AddToRole(user.Id, role);
                    }
                };


            // seed an admin, developer and some users
            SeedUser("AdminUser", "Administrator", "admin@admin.com", "Password");
            SeedUser("DevUser1", "Developer", "dev1@dev1.com", "Password");
            SeedUser("DevUser2", "Developer", "dev2@dev2.com", "Password");
            SeedUser("User1", "Administrator", "user1@user1.com", "Password");
            SeedUser("User2", "Administrator", "user2@user2.com", "Password");

        }
    }
}