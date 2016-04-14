﻿using Microsoft.AspNet.Identity;
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
                    var user = context.AspNetUsers.Where(u => u.Id == identityUser.Id).FirstOrDefault();

                    // check users role
                    if (user.AspNetRoles.Single().Name == "admin")
                    {
                        return RedirectToAction("AdminDashboard", "Accounts");
                    }
                    else if (user.AspNetRoles.Single().Name == "consumer")
                    {
                        return RedirectToAction("ConsumerDashboard2", "Accounts");
                    }
                    /*
                        This isn't a reliable way-- causes problem at times where the role's name can't be found for certain users.
                        AndrewH.
                        Think that was becuase they didnt exist in the DB, now when  ever we register need to assign roleID 2
                    */
                }
                else
                {
                    ViewBag.Error = "Invalid username or password.";
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
            JarvisEntities context = new JarvisEntities();
            AspNetUser user = context.AspNetUsers
                             .Where(u => u.Id == identityUser.Id).FirstOrDefault();
            AspNetRole role = context.AspNetRoles
                             .Where(r => r.Name == "consumer").FirstOrDefault(); 

            //user roles is abridge table so can't select it directly
            user.AspNetRoles.Add(role);
            context.SaveChanges();

            if (result.Succeeded)
            {


                CreateTokenProvider(manager, EMAIL_CONFIRMATION);

                var code = manager.GenerateEmailConfirmationToken(identityUser.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Accounts",
                                                new { userId = identityUser.Id, code = code },
                                                    protocol: Request.Url.Scheme);

                string email = "Please confirm your account by clicking this link: <a href=\""
                                + callbackUrl + "\">Confirm Registration</a>";
                ViewBag.FakeConfirmation = email;

                MailHelper mailer = new MailHelper();
                string response = mailer.EmailFromArvixe(
                                           new RegisteredUser(newUser.Email, newUser.Subject = "Confirm Email", newUser.Body = email));
                ViewBag.Response = response;
                return View("EmailSent");
            }
            else
            {
                ViewBag.Error = "Oops, something whent wrong. Could not register new user. Please try again.";
                return View();
            }
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
        public ActionResult ConfirmEmail(string userID, string code)
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
            return View("ResultPassword");
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
        [Authorize(Roles = "admin, consumer")]
        public ActionResult ConsumerDashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                if(User.IsInRole("admin"))
                {
                    return RedirectToAction("AdminDashboard");
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        public async Task<ActionResult> ConsumerDashboard2()
        {
            if (User.Identity.IsAuthenticated)
            {
                ThermostatVMRepo thermoRepo = new ThermostatVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<ThermostatVM> therms = await thermoRepo.GetThermostats();

                CameraVMRepo camRepo = new CameraVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<CameraVM> cams = await camRepo.GetCameras();

                SmokeCoAlarmVMRepo alarmRepo = new SmokeCoAlarmVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<SmokeCoAlarmVM> alarms = await alarmRepo.GetAlarms();

                NestVM nestModel = new NestVM(cams, therms, alarms);

                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;

                JarvisEntities context = new JarvisEntities();
                var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();
                
                if (user.Roles.Count == 1)
                {
                    ViewBag.Role = "admin";
                    return View(nestModel);
                }
                else
                {
                    ViewBag.Role = "consumer";
                    return View(nestModel);
                }
                //if (query.AspNetRoles.Single().Name == "admin")
                //{
                //    ViewBag.Role = "admin";
                //    return View(nestModel);
                //}
                //else
                //{
                //    ViewBag.Role = "consumer";
                //    return View(nestModel);
                //}
                
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
                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;
                EditableUserRepo editRepo = new EditableUserRepo();
                EditableUser editUser = editRepo.getUser(myID);

                JarvisEntities context = new JarvisEntities();
                var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();

                if (query.AspNetRoles.Single().Name == "admin")
                {
                    ViewBag.Role = "admin";
                    return View(editUser);
                }
                else
                {
                    ViewBag.Role = "consumer";
                    return View(editUser);
                }
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        [HttpGet]
        public ActionResult AdminAccountView(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                EditableUserRepo editRepo = new EditableUserRepo();
                EditableUser editUser = editRepo.getUser(id);
                return View(editUser);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        [HttpPost]
        public ActionResult AdminAccountView(EditableUser editedUser)
        {
            if (ModelState.IsValid)
            {
                EditableUserRepo editRepo = new EditableUserRepo();
                editRepo.updateUser(editedUser);
                IAuthenticationManager authenticationManager
                                           = HttpContext.GetOwinContext().Authentication;
                authenticationManager
               .SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var identity = new ClaimsIdentity(new[] {
                                            new Claim(ClaimTypes.Name, editedUser.UserName),
                                        },
                                    DefaultAuthenticationTypes.ApplicationCookie,
                                    ClaimTypes.Name, ClaimTypes.Role);

                authenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false
                }, identity);
            }
            return RedirectToAction("ViewAllConsumerAccounts");
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
        // NOT IN USE
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
                ThermostatVMRepo thermoRepo = new ThermostatVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<ThermostatVM> therms = await thermoRepo.GetThermostats();

                CameraVMRepo camRepo = new CameraVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<CameraVM> cams = await camRepo.GetCameras();

                SmokeCoAlarmVMRepo alarmRepo = new SmokeCoAlarmVMRepo("c.QY4JkcdwELewWkIDfbgCm2WSEHlaKSvI6g6dpWVOf7levs96rMRByP4xRQksCJUfxrSgYKPwiUKzj1OcgIad2nxerddqp4QvMleuC55br637xaGnychVSl4yMUoQBoWI8uFg1dI9uiK2hZ49");
                IEnumerable<SmokeCoAlarmVM> alarms = await alarmRepo.GetAlarms();

                NestVM nestModel = new NestVM(cams, therms, alarms);

                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;

                JarvisEntities context = new JarvisEntities();
                var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();

                if (query.AspNetRoles.Single().Name == "admin")
                {
                    ViewBag.Role = "admin";
                    return View(nestModel);
                }
                else
                {
                    ViewBag.Role = "consumer";
                    return View(nestModel);
                }
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
                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;

                JarvisEntities context = new JarvisEntities();
                var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();

                if (query.AspNetRoles.Single().Name == "admin")
                {
                    ViewBag.Role = "admin";
                    return View();
                }
                else
                {
                    ViewBag.Role = "consumer";
                    return View();
                }
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
                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);

                EditableUserRepo editRepo = new EditableUserRepo();
                var allUsers = editRepo.getUsers();
                return View(allUsers);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        [HttpGet]
        public ActionResult EditAccount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;
                EditableUserRepo editRepo = new EditableUserRepo();
                EditableUser editUser = editRepo.getUser(myID);

                JarvisEntities context = new JarvisEntities();
                var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();

                if (query.AspNetRoles.Single().Name == "admin")
                {
                    ViewBag.Role = "admin";
                    return View(editUser);
                }
                else
                {
                    ViewBag.Role = "consumer";
                    return View(editUser);
                }
                
            }else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        [HttpPost]
        public ActionResult EditAccount(EditableUser editedUser)
        {
            if (ModelState.IsValid)
            {
                EditableUserRepo editRepo = new EditableUserRepo();
                editRepo.updateUser(editedUser);
                IAuthenticationManager authenticationManager
                                           = HttpContext.GetOwinContext().Authentication;
                authenticationManager
               .SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var identity = new ClaimsIdentity(new[] {
                                            new Claim(ClaimTypes.Name, editedUser.UserName),
                                        },
                                    DefaultAuthenticationTypes.ApplicationCookie,
                                    ClaimTypes.Name, ClaimTypes.Role);

                authenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false
                }, identity);
            }
            return RedirectToAction("AccountView");
        }

        //need a view to confirm deletion, or maybe just a popup from original page using js?
        // can use JS to pop up another window whose conteent will call the delete post method...
        [HttpGet]
        public ActionResult DeleteAccount()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<IdentityUser>();
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
                var user = manager.FindByName(User.Identity.Name);
                string myID = user.Id;
                EditableUserRepo editRepo = new EditableUserRepo();
                EditableUser editUser = editRepo.getUser(myID);
                return View(editUser);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        public ActionResult DeleteAccount(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                EditableUserRepo editRepo = new EditableUserRepo();
                EditableUser editUser = editRepo.getUser(id);
                return View(editUser);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }
        [HttpPost]
        public ActionResult DeleteAccount(EditableUser editedUser)
        {
            if (ModelState.IsValid)
            {
                EditableUserRepo editRepo = new EditableUserRepo();
                editRepo.updateUser(editedUser);
                IAuthenticationManager authenticationManager
                                           = HttpContext.GetOwinContext().Authentication;
                authenticationManager
               .SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var identity = new ClaimsIdentity(new[] {
                                            new Claim(ClaimTypes.Name, editedUser.UserName),
                                        },
                                    DefaultAuthenticationTypes.ApplicationCookie,
                                    ClaimTypes.Name, ClaimTypes.Role);

                authenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false
                }, identity);
            }
            return RedirectToAction("AccountView");
        }


    }
}