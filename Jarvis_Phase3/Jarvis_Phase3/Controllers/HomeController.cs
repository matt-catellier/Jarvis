using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jarvis_Phase3.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var userStore = new UserStore<IdentityUser>();
            UserManager<IdentityUser> manager = new UserManager<IdentityUser>(userStore);
            var user = manager.FindByName(User.Identity.Name);
            if(user == null) // not logged in
            {
                return View();
            }

            string myID = user.Id;
            JarvisEntities context = new JarvisEntities();
            var query = context.AspNetUsers.Where(u => u.Id == myID).FirstOrDefault();        
            if(query != null)
            {
                if (query.AspNetRoles.Single().Name == "admin")
                {
                    ViewBag.Role = "admin";
                    return RedirectToAction("AdminDashboard", "Accounts");
                }
                else if (query.AspNetRoles.Single().Name == "consumer")
                {
                    ViewBag.Role = "consumer";
                    return RedirectToAction("ConsumerDashboard2", "Accounts");
                }
            }      
            return View();
        }

        public ActionResult AboutSupport()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Support()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult DashTest()
        {
            return View();
        }

        public ActionResult API()
        {
            return View();
        }
    }
}