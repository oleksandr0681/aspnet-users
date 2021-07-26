using AuthWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AuthWebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                if (model.Email != null)
                {
                    if (model.Password != null)
                    {
                        using (UserContext db = new UserContext())
                        {
                            user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                        }
                        if (user == null)
                        {
                            using (UserContext db = new UserContext())
                            {
                                db.Users.Add(new User
                                {
                                    Name = model.Name,
                                    Email = model.Email,
                                    Password = model.Password,
                                    RegistrationDate = DateTime.Now,
                                    LoginDate = DateTime.Now,
                                    Status = "Unblock"
                                });
                                db.SaveChanges();
                                user = db.Users.Where(u => u.Email == model.Email && u.Password == model.Password).FirstOrDefault();
                            }
                            if (user != null)
                            {
                                FormsAuthentication.SetAuthCookie(model.Email, true);
                                HttpCookie idCookie = new HttpCookie("idUser", user.Id.ToString());
                                idCookie.Expires = DateTime.Now.AddMinutes(2880.0);
                                Response.Cookies.Add(idCookie);
                                HttpCookie nameCookie = new HttpCookie("nameUser", user.Name);
                                nameCookie.Expires = DateTime.Now.AddMinutes(2880.0);
                                Response.Cookies.Add(nameCookie);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "User with this e-mail exist.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please enter password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Please enter e-mail.");
                }
                
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                    if(user !=null)
                    {
                        user.LoginDate = DateTime.Now;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    HttpCookie idCookie = new HttpCookie("idUser", user.Id.ToString());
                    idCookie.Expires = DateTime.Now.AddMinutes(2880.0);
                    Response.Cookies.Add(idCookie);
                    HttpCookie nameCookie = new HttpCookie("nameUser", user.Name);
                    nameCookie.Expires = DateTime.Now.AddMinutes(2880.0);
                    Response.Cookies.Add(nameCookie);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login failed. Please check your e-mail and password and try again.");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpCookie idCookie = Request.Cookies["idUser"];
            if (idCookie != null)
            {
                idCookie.Expires = DateTime.Now.AddMinutes(-100.0);
                Response.Cookies.Add(idCookie);
            }
            HttpCookie nameCookie = Request.Cookies["nameUser"];
            if (nameCookie != null)
            {
                nameCookie.Expires = DateTime.Now.AddMinutes(-100.0);
                Response.Cookies.Add(nameCookie);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}