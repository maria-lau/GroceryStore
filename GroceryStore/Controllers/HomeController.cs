using GroceryStore.Models;
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

            AuthenticationDatabase db = AuthenticationDatabase.getInstance();
            Response loginresponse = db.Login(usernameData, passwordData);

            if (loginresponse.result)
            {
                Response.Write("<script>alert('" + loginresponse.response + "')</script>");
                return View("../Home/Index");
            }
            else
            {
                Response.Write("<script>alert('" + loginresponse.response + "')</script>");
                return View("Login");
            }
        }

        public ActionResult LogOut()
        {
            Globals.setUser("Log In");
            return View("Index");
        }

        [HttpPost]
        public ActionResult CreateAccountPost(string usernameData, string passwordData, string firstName, string lastName,
            string street, string city, string province, string postalCode, string emailData, string phoneData)
        {
            //Get form data from HTML web page
            UserAccount newaccount = new UserAccount();
            newaccount.username = usernameData;
            newaccount.password = passwordData;
            newaccount.fname = firstName;
            newaccount.lname = lastName;
            newaccount.street = street;
            newaccount.city = city;
            newaccount.province = province;
            newaccount.postalcode = postalCode;
            newaccount.email = emailData;
            newaccount.phone = phoneData;

            AuthenticationDatabase db = AuthenticationDatabase.getInstance();
            Response createaccountresponse = db.insertNewUserAccount(newaccount);

            //Check if account created successfully
            if (createaccountresponse.result)
            {
                Globals.setUser(newaccount.username);
                Response.Write("<script>alert('" + createaccountresponse.response + "')</script>");
                return View("Index");
            }
            else
            {
                Response.Write("<script>alert('" + createaccountresponse.response + "')</script>");
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