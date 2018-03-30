using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroceryStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Message = "This is a a web application created by CPSC 471 - Database Management Systems - students:" +
                                " Maria Lau, Sara Li & Juan Nieto as their final project.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Fresh Direct Locations";

            return View();
        }

        public ActionResult GoToLogin()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(string usernameData, string passwordData)
        {
            string username = usernameData;
            string password = passwordData;


            return View("../Home/Index");
        }

        public ActionResult LogOut()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult CreateAccountPost(string usernameData, string passwordData, string firstName, string lastName,
            string street, string city, string province, string postalCode, string emailData, string phoneData)
        {
            //Get form data from HTML web page

            bool accountCreation = true;
            //Check if account created successfull
            string message = "Account created successfully.";
            if (accountCreation)
            {
                Response.Write("<script>alert('" + message + "')</script>");
                return View("Index");
            }
            else
            {
                message = "Failed to create account.";
                Response.Write("<script>alert('" + message + "')</script>");
                return View("CreateAccount");
            }
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }
    }
}