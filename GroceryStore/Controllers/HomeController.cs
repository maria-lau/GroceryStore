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

        public ActionResult ViewStores()
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

            UserDatabase db = UserDatabase.getInstance();
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

            UserDatabase db = UserDatabase.getInstance();
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

        [HttpPost]
        public ActionResult CreateEmployeeAccountPost(string usernameData, string passwordData, string firstName, string lastName,
            string street, string city, string province, string postalCode, string emailData, string phoneData, int sin, string startdate, float hourlywage)
        {
            //Get form data from HTML web page
            EmployeeAccount employeeaccount = new EmployeeAccount();
            employeeaccount.username = usernameData;
            employeeaccount.password = passwordData;
            employeeaccount.fname = firstName;
            employeeaccount.lname = lastName;
            employeeaccount.street = street;
            employeeaccount.city = city;
            employeeaccount.province = province;
            employeeaccount.postalcode = postalCode;
            employeeaccount.email = emailData;
            employeeaccount.phone = phoneData;
            employeeaccount.sin = sin;
            employeeaccount.startdate = startdate;
            employeeaccount.hourlywage = hourlywage;

            UserDatabase db = UserDatabase.getInstance();
            Response createemployeeaccountresponse = db.insertNewEmployeeAccount(employeeaccount);

            //Check if account created successfully
            if (createemployeeaccountresponse.result)
            {
                Response.Write("<script>alert('" + createemployeeaccountresponse.response + "')</script>");
                return View("ViewEmployees");
            }
            else
            {
                Response.Write("<script>alert('" + createemployeeaccountresponse.response + "')</script>");
                return View("CreateEmployeeAccount");
            }
        }

        [HttpPost]
        public ActionResult CreateManagerAccountPost(string usernameData, string passwordData, string firstName, string lastName,
            string street, string city, string province, string postalCode, string emailData, string phoneData, int sin, 
            string startdate, float hourlywage, int storeid)
        {
            //Get form data from HTML web page
            ManagerAccount manageraccount = new ManagerAccount();
            manageraccount.username = usernameData;
            manageraccount.password = passwordData;
            manageraccount.fname = firstName;
            manageraccount.lname = lastName;
            manageraccount.street = street;
            manageraccount.city = city;
            manageraccount.province = province;
            manageraccount.postalcode = postalCode;
            manageraccount.email = emailData;
            manageraccount.phone = phoneData;
            manageraccount.sin = sin;
            manageraccount.startdate = startdate;
            manageraccount.hourlywage = hourlywage;
            manageraccount.storeid = storeid;

            UserDatabase db = UserDatabase.getInstance();
            Response createmanageraccountresponse = db.insertNewManagerAccount(manageraccount);

            //Check if account created successfully
            if (createmanageraccountresponse.result)
            {
                Response.Write("<script>alert('" + createmanageraccountresponse.response + "')</script>");
                return View("ViewEmployees");
            }
            else
            {
                Response.Write("<script>alert('" + createmanageraccountresponse.response + "')</script>");
                return View("CreateManagerAccount");
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAccount()
        {
            return View();
        }

        public ActionResult CreateEmployeeAccount()
        {
            return View();
        }

        public ActionResult CreateManagerAccount()
        {
            return View();
        }

        public ActionResult ViewEmployees()
        {
            return View();
        }

        public ActionResult CreateStore()
        {
            return View();
        }

        public ActionResult ViewOrders()
        {
            OrderDatabase db = OrderDatabase.getInstance();
            // <orderid, orderdate, deliveredboolean
            List<Tuple<int, string, string>> orderInfo = db.getOrders(Globals.getUser());
            return View();
        }


        public ActionResult ViewCart()
        {
            string username = Globals.getUser();
            GroceryDatabase db = GroceryDatabase.getInstance();
            Cart mycart = db.getCart(username);

            if (mycart.cartcontents == null)
            {
                ViewBag.EmptyCart = "true";
            }
            else
            {
                ViewBag.EmptyCart = "false";
                //for(int i = 0; i < mycart.cartcontents.Count; i++)
                //{
                //    int sku = mycart.cartcontents.ElementAt(i).Item1;
                //    string itemname = mycart.cartcontents.ElementAt(i).Item2;
                //    int quantity = mycart.cartcontents.ElementAt(i).Item3;
                //}
      
            }
            return View();
        }
        
        [HttpPost]
        public ActionResult CreateStorePost(int storeid, string street, string city, string province,string postalCode, string phone)
        {
            Store newstore = new Store();
            newstore.storeid = storeid;
            newstore.street = street;
            newstore.city = city;
            newstore.province = province;
            newstore.postalcode = postalCode;
            newstore.phone = phone;

            StoreDatabase db = StoreDatabase.getInstance();
            Response addstoreresponse = db.insertNewStore(newstore);

            if (addstoreresponse.result)
            {
                Response.Write("<script>alert('" + addstoreresponse.response + "')</script>");
                return View("ViewStores");
            }
            else
            {
                Response.Write("<script>alert('" + addstoreresponse.response + "')</script>");
                return View("CreateStore");
            }
        }

        public ActionResult Shop()
        {
            return View();
        }

        public ActionResult AddGroceryItem()
        {
            return View();
        }

        public ActionResult AddGroceryItemPost(int sku, string name, double purchaseprice, double sellingprice, int quantity)
        {
            GroceryItem newitem = new GroceryItem();
            newitem.sku = sku;
            newitem.name = name;
            newitem.purchaseprice = purchaseprice;
            newitem.sellingprice = sellingprice;
            newitem.quantity = quantity;

            GroceryDatabase db = GroceryDatabase.getInstance();
            Response additemresponse = db.insertNewGroceryItem(newitem);
            if (additemresponse.result)
            {
                Response.Write("<script>alert('" + additemresponse.response + "')</script>");
                return View("Shop");
            }
            else
            {
                Response.Write("<script>alert('" + additemresponse.response + "')</script>");
                return View("AddGroceryItem");
            }
        }
    }
}