using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
// using System.Web.Http.Cors; // dont need this for now...
using System.Web.Mvc;
using System.Threading.Tasks;
using Jarvis_Phase3.Models;
using Jarvis_Phase3.BusinessLogic;

namespace Jarvis_Phase3.Controllers
{
    public class AccountsController : Controller
    {
        // leave blank
        /* ================================== */
        /* ====== WEB SECURITY IDENTITY ===== */
        /* ================================== */
        const string EMAIL_CONFIRMATION = "EmailConfirmation";
        const string PASSWORD_RESET = "ResetPassword";

        void CreateTokenProvider(UserManager<IdentityUser> manager, string tokenType)
        {
            manager.UserTokenProvider = new EmailTokenProvider<IdentityUser>();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login login)
        {
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
            IdentityUser identityUser = manager.Find(login.UserName,
                                                             login.Password);

            if (ModelState.IsValid)
            {
                if (ValidLogin(login))
                {
                    IAuthenticationManager authenticationManager
                                           = HttpContext.GetOwinContext().Authentication;
                    authenticationManager
                   .SignOut(DefaultAuthenticationTypes.ExternalCookie);

                    var identity = new ClaimsIdentity(new[] {
                                            new Claim(ClaimTypes.Name, login.UserName),
                                        },
                                        DefaultAuthenticationTypes.ApplicationCookie,
                                        ClaimTypes.Name, ClaimTypes.Role);

                    authenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, identity);
                    System.Threading.Thread.Sleep(2000);


                    JarvisEntities context = new JarvisEntities();
                    var query = context.AspNetUsers.Where(u => u.Id == identityUser.Id).FirstOrDefault();

                    if (query.AspNetRoles.Single().Name == "admin")
                    {
                        return RedirectToAction("AdminDashboard", "Accounts");
                    }
                    else if (query.AspNetRoles.Single().Name == "consumer")
                    {
                        return RedirectToAction("ConsumerDashboard", "Accounts");
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisteredUser newUser)
        {
            // TAKING THE WRONG MODEL AS INPUT???
            var userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore)
            {
                UserLockoutEnabledByDefault = true,
                DefaultAccountLockoutTimeSpan = new TimeSpan(0, 10, 0),
                MaxFailedAccessAttemptsBeforeLockout = 3
            };

            var identityUser = new IdentityUser()
            {
                UserName = newUser.UserName,
                Email = newUser.Email
            };

            IdentityResult result = manager.Create(identityUser, newUser.Password);
            if (result.Succeeded)
            {
                return View();
            }
            else
            {
                return View();
            }

            //    // this threw an error, but it also worked so what gives???
            //    IdentityResult result = manager.Create(identityUser, newUser.Password);
            //    if (result.Succeeded)
            //    {
            //        CreateTokenProvider(manager, EMAIL_CONFIRMATION);
            //        // identityUser.Id use this to create an entry in our accounts table 
            //        var code = manager.GenerateEmailConfirmationToken(identityUser.Id);
            //        var callbackUrl = Url.Action("VerifiedEmail", "Accounts",
            //                                        new { userId = identityUser.Id, code = code },
            //                                            protocol: Request.Url.Scheme);

            //        string email = "Please confirm your account by clicking this link: <a href=\""
            //                        + callbackUrl + "\">Confirm Registration</a>";


            //        ViewBag.FakeConfirmation = email;
            //        UserAccountVMRepo uaRepo = new UserAccountVMRepo();
            //        uaRepo.CreateAccount(newUser.FirstName, newUser.LastName, identityUser.Id);

            //        // CREATE WITH CONSUMER ROLE BY DEFAULT
            //        SecurityEntities context = new SecurityEntities();
            //        AspNetUser user = context.AspNetUsers
            //                         .Where(u => u.UserName == newUser.UserName).FirstOrDefault();
            //        AspNetRole role = context.AspNetRoles
            //                         .Where(r => r.Name == "consumer").FirstOrDefault();

            //        user.AspNetRoles.Add(role);
            //        context.SaveChanges();

            //        MailHelper mailer = new MailHelper();
            //        string response = mailer.EmailFromArvixe(
            //                                   new RegisteredUser(newUser.Email, newUser.Subject = "Confirm Email", newUser.Body = email));

            //        ViewBag.Response = response;
            //        return View("ConfirmEmail");
            //    }
            //return View();
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        bool ValidLogin(Login login)
        {
            UserStore<IdentityUser> userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(userStore)
            {
                UserLockoutEnabledByDefault = true,
                DefaultAccountLockoutTimeSpan = new TimeSpan(0, 10, 0),
                MaxFailedAccessAttemptsBeforeLockout = 3
            };
            var user = userManager.FindByName(login.UserName);

            if (user == null)
                return false;

            // User is locked out.
            if (userManager.SupportsUserLockout && userManager.IsLockedOut(user.Id))
                return false;

            // Validated user was locked out but now can be reset.
            if (userManager.CheckPassword(user, login.Password) && userManager.IsEmailConfirmed(user.Id))

            {
                if (userManager.SupportsUserLockout
                 && userManager.GetAccessFailedCount(user.Id) > 0)
                {
                    userManager.ResetAccessFailedCount(user.Id);
                }
            }
            // Login is invalid so increment failed attempts.
            else {
                bool lockoutEnabled = userManager.GetLockoutEnabled(user.Id);
                if (userManager.SupportsUserLockout && userManager.GetLockoutEnabled(user.Id))
                {
                    userManager.AccessFailed(user.Id);
                    return false;
                }
            }
            return true;
        }
        public ActionResult ConfirmEmail()
        {

            return View();
        }
        public ActionResult VerifiedEmail(string userID, string code)
        {
            var userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
            var user = manager.FindById(userID);
            CreateTokenProvider(manager, EMAIL_CONFIRMATION);
            try
            {
                IdentityResult result = manager.ConfirmEmail(userID, code);
                if (result.Succeeded)
                    ViewBag.Message = "You are now registered!";
            }
            catch
            {
                ViewBag.Message = "Validation attempt failed!";
            }
            return View();

        }
        /* ============================ */
        /* ===== ADMIN PRIVILEGES ===== */
        /* ============================ */
        [HttpGet]
        public ActionResult AddRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        [HttpPost]
        public ActionResult AddRole(AspNetRole role)
        {
            JarvisEntities context = new JarvisEntities();
            context.AspNetRoles.Add(role);
            context.SaveChanges();
            return View();
        }

        [HttpGet]
        public ActionResult AddUserToRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        [HttpPost]
        public ActionResult AddUserToRole(string userName, string roleName)
        {
            JarvisEntities context = new JarvisEntities();
            AspNetUser user = context.AspNetUsers
                             .Where(u => u.UserName == userName).FirstOrDefault();
            AspNetRole role = context.AspNetRoles
                             .Where(r => r.Name == roleName).FirstOrDefault();

            user.AspNetRoles.Add(role);
            context.SaveChanges();
            return View();
        }
        /* ============================= */
        /* ===== PASSWORD RECOVERY ===== */
        /* ============================= */
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email, RegisteredUser userRecovery)
        {
            var userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
            var user = manager.FindByEmail(email);
            CreateTokenProvider(manager, PASSWORD_RESET);

            var code = manager.GeneratePasswordResetToken(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Accounts",
                                         new { userId = user.Id, code = code },
                                         protocol: Request.Url.Scheme);
            var body = "Please reset your password by clicking <a href=\""
                                     + callbackUrl + "\">here</a>";

            MailHelper mailer = new MailHelper();
            string response = mailer.EmailFromArvixe(
                                       new RegisteredUser(userRecovery.Email = email, userRecovery.Subject = "Password Recovery Email", userRecovery.Body = body));
            return View("PasswordEmail");
        }
        public ActionResult PasswordEmail()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword(string userID, string code)
        {
            ViewBag.PasswordToken = code;
            ViewBag.UserID = userID;
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string password, string passwordConfirm,
                                          string passwordToken, string userID)
        {

            var userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
            var user = manager.FindById(userID);
            CreateTokenProvider(manager, PASSWORD_RESET);

            IdentityResult result = manager.ResetPassword(userID, passwordToken, password);
            if (result.Succeeded)
                ViewBag.Result = "The password has been successfully reset.";
            else
                ViewBag.Result = "The password has not been reset.";
            return View("SuccessPassword");
        }
        public ActionResult SuccessPassword()
        {
            return View();
        }
        /* ================= */
        /* ===== PAGES ===== */
        /* ================= */

        public ActionResult AdminDashBoard()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        public ActionResult ConsumerDashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        public ActionResult AccountView()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        public ActionResult Data()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        public ActionResult Insights()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        public async Task<ActionResult> DeviceManager()
        {
            if (User.Identity.IsAuthenticated)
            {
                ThermostatVMRepo thermoRepo = new ThermostatVMRepo();
                IEnumerable<ThermostatVM> therms = await thermoRepo.GetThermostats();

                CameraVMRepo camRepo = new CameraVMRepo();
                IEnumerable<CameraVM> cams = await camRepo.GetCameras();

                SmokeCoAlarmVMRepo alarmRepo = new SmokeCoAlarmVMRepo();
                IEnumerable<SmokeCoAlarmVM> alarms = await alarmRepo.GetAlarms();

                NestVM nestModel = new NestVM(cams, therms, alarms);

                return View(nestModel);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
            
        }
        public ActionResult RegisterDevices()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        public ActionResult ViewAllConsumerAccounts()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
    }
}