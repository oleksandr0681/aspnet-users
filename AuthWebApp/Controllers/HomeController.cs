using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AuthWebApp.Models;

namespace AuthWebApp.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            List<SummaryUserModel> summaryUsers = null;
            using (UserContext db = new UserContext())
            {
                List<User> users = null;
                users = db.Users.ToList();
                if (users != null)
                {
                    summaryUsers = new List<SummaryUserModel>();
                    for (int i = 0; i < users.Count; i++)
                    {
                        summaryUsers.Add(new SummaryUserModel
                        {
                            Id = users[i].Id,
                            Name = users[i].Name,
                            Email = users[i].Email,
                            RegistrationDate = users[i].RegistrationDate,
                            LoginDate = users[i].LoginDate,
                            Status = users[i].Status
                        });
                    }
                }
                HttpCookie nameCookie = Request.Cookies["nameUser"];
                if (nameCookie != null)
                {
                    ViewBag.NameUser = nameCookie.Value;
                }
                HttpCookie logoutCookie = Request.Cookies["logout"];
                if (logoutCookie != null)
                {
                    if (logoutCookie.Value == "1")
                    {
                        FormsAuthentication.SignOut();
                        HttpCookie idCookie = Request.Cookies["idUser"];
                        if (idCookie != null)
                        {
                            idCookie.Expires = DateTime.Now.AddMinutes(-100.0);
                            Response.Cookies.Add(idCookie);
                        }
                        if (nameCookie != null)
                        {
                            nameCookie.Expires = DateTime.Now.AddMinutes(-100.0);
                            Response.Cookies.Add(nameCookie);
                        }
                    }
                    logoutCookie.Expires = DateTime.Now.AddMinutes(-100.0);
                    Response.Cookies.Add(logoutCookie);
                }
            }
            return View(summaryUsers);
        }

        [HttpPost]
        public ActionResult Index(List<SummaryUserModel> summaryUsers, string action)
        {
            bool isOtherChecked = false;
            bool logoutNow = false;
            int id = -1;
            HttpCookie idCookie = Request.Cookies["idUser"];
            HttpCookie nameCookie = Request.Cookies["nameUser"];
            if (nameCookie != null)
            {
                ViewBag.NameUser = nameCookie.Value;
            }
            User user = null;
            if (summaryUsers != null && idCookie != null)
            {
                bool parseResult = int.TryParse(idCookie.Value, out id);
                if (parseResult == true)
                {
                    foreach (var item in summaryUsers)
                    {
                        if (item.IsChecked && item.Id != id)
                        {
                            isOtherChecked = true;
                        }
                    }
                }
            }
            if (action == "Block")
            {
                SummaryUserModel currentUser = null;
                if (summaryUsers != null && idCookie != null)
                {
                    currentUser = summaryUsers.Find(u => u.Id == id);
                }
                if (currentUser != null && currentUser.Status != "Block")
                {
                    if (currentUser.IsChecked == true)
                    {
                        using (UserContext db = new UserContext())
                        {
                            user = db.Users.Find(id);
                            if (user != null)
                            {
                                user.Status = "Block";
                                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                logoutNow = true;
                            }
                        }
                    }
                    if (isOtherChecked)
                    {
                        foreach (var item in summaryUsers)
                        {
                            if (item.IsChecked)
                            {
                                using (UserContext db = new UserContext())
                                {
                                    user = db.Users.Find(item.Id);
                                    if (user != null)
                                    {
                                        user.Status = "Block";
                                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        if (logoutNow == false)
                        {
                            HttpCookie logoutCookie = new HttpCookie("logout", "1");
                            Response.Cookies.Add(logoutCookie);
                        }
                    }
                }
                if (logoutNow == true)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            if (action == "Unblock")
            {
                SummaryUserModel currentUser = null;
                if (summaryUsers != null && idCookie != null)
                {
                    currentUser = summaryUsers.Find(u => u.Id == id);
                }
                if (currentUser != null)
                {
                    if (currentUser.IsChecked)
                    {
                        using (UserContext db = new UserContext())
                        {
                            user = db.Users.Find(id);
                            if (user != null)
                            {
                                user.Status = "Unblock";
                                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                    if (currentUser.Status != "Block")
                    {
                        foreach (var item in summaryUsers)
                        {
                            if (item.IsChecked)
                            {
                                using (UserContext db = new UserContext())
                                {
                                    user = db.Users.Find(item.Id);
                                    if (user != null)
                                    {
                                        user.Status = "Unblock";
                                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (action == "Delete")
            {
                SummaryUserModel currentUser = null;
                if (summaryUsers != null && idCookie != null)
                {
                    currentUser = summaryUsers.Find(u => u.Id == id);
                }
                if (currentUser != null && currentUser.Status != "Block")
                {
                    if (currentUser.IsChecked == true)
                    {
                        using (UserContext db = new UserContext())
                        {
                            user = db.Users.Find(id);
                            if (user != null)
                            {
                                db.Users.Remove(user);
                                db.SaveChanges();
                                logoutNow = true;
                            }
                        }
                    }
                    if (isOtherChecked)
                    {
                        foreach (var item in summaryUsers)
                        {
                            if (item.IsChecked)
                            {
                                using (UserContext db = new UserContext())
                                {
                                    user = db.Users.Find(item.Id);
                                    if (user != null)
                                    {
                                        db.Users.Remove(user);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        if (logoutNow == false)
                        {
                            HttpCookie logoutCookie = new HttpCookie("logout", "1");
                            Response.Cookies.Add(logoutCookie);
                        }
                    }
                }
                if (logoutNow == true)
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            return RedirectToAction("ReloadUsers", "Home");
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Users application.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ReloadUsers()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}