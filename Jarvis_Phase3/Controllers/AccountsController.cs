using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jarvis_Phase3.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Logout()
        {
            return View();
        }
        public ActionResult ConfirmEmail()
        {
            return View();
        }
        public ActionResult VerifiedEmail()
        {
            return View();
        }

        /* ======================= */
        /* ===== ADMIN ROLES ===== */
        /* ======================= */

        public ActionResult AddRole()
        {
            return View();
        }
        public ActionResult AddUserToRole()
        {
            return View();
        }

        /* ============================= */
        /* ===== PASSWORD RECOVERY ===== */
        /* ============================= */

        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult PasswordEmail()
        {
            return View();
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        public ActionResult SucessPassword()
        {
            return View();
        }

        /* ============================= */

        public ActionResult AdminDashBoard()
        {
            return View();
        }
        public ActionResult ConsumerDashboard()
        {
            return View();
        }
        public ActionResult AccountView()
        {
            return View();
        }
        public ActionResult Data()
        {
            return View();
        }
        public ActionResult Insights()
        {
            return View();
        }
        public ActionResult DeviceManager()
        {
            return View();
        }
        public ActionResult RegisterDevices()
        {
            return View();
        }
        public ActionResult ViewAllConsumerAccounts()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
    }
}