using System;
using System.Linq;
using Entrevista.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Configuration;


namespace Entrevista.Classes
{
    public class UsersHelper : IDisposable
    {
        private static ApplicationDbContext userContext = new ApplicationDbContext();
        private static EntrevistaContext db = new EntrevistaContext();

        // Deletar Usuários

        public static bool DeleteUser(string UserName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(UserName);
            if (userASP == null)
            {
                return false;
            }
            var response = userManager.Delete(userASP);
            return response.Succeeded;
        }

        // Atualizar Usuários

        public static bool UpdateUserName(string CurrentUserName, string NewUserName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(CurrentUserName);
            if (userASP == null)
            {
                return false;
            }
            userASP.UserName = NewUserName;
            userASP.Email = NewUserName;
            var response = userManager.Update(userASP);
            return response.Succeeded;
        }

        public static bool UpdateUserPassword(string UserName, string NewPassword)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(UserName);
            if (userASP == null)
            {
                return false;
            }
            userManager.RemovePassword(userASP.Id);
            var response = userManager.AddPassword(userASP.Id, NewPassword);
            return response.Succeeded;
        }

        public static void CheckRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            // Check to see if Role Exists, if not create it
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        public static void CheckSuperUser()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var email = WebConfigurationManager.AppSettings["AdminUser"];
            var password = WebConfigurationManager.AppSettings["AdminPassWord"];
            var userASP = userManager.FindByName(email);
            if (userASP == null)
            {
                CreateUserASP(email, "Admin", password);
                return;
            }

            userManager.AddToRole(userASP.Id, "Admin");
        }

        public static void CreateUserASP(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            userManager.Create(userASP, email);
            userManager.AddToRole(userASP.Id, roleName);
        }

        public static void CreateUserASP(string email, string roleName, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            userManager.Create(userASP, password);
            userManager.AddToRole(userASP.Id, roleName);
        }

        public static async Task PasswordRecovery(string email)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return;
            }

            var random = new Random();
            var newPassword = string.Format("{0}{1}{2:04}*",
                "Martenick".Trim().ToUpper().Substring(0, 1),
                "Penchel".Trim().ToLower(),
                random.Next(10000));

            userManager.RemovePassword(userASP.Id);
            userManager.AddPassword(userASP.Id, newPassword);

            var subject = "A senha foi modificada";
            var body = string.Format(@"
                <h1>Modificação de senha</h1>
                <p>Sua nova senha é: <strong>{0}</strong></p>
                <p>Por favor altere para uma senha de fácil recuperação",
                newPassword);

            await MailHelper.SendMail(email, subject, body);
        }

        public static void ChangePassword(string email, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return;
            }

            userManager.RemovePassword(userASP.Id);
            userManager.AddPassword(userASP.Id, password);

        }

        public void Dispose()
        {
            userContext.Dispose();
            db.Dispose();
        }
    }
}
