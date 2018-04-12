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

        public ActionResult ViewStores(int storeDeleted = 0)
        {
            if (storeDeleted == 1)
            {
                Response.Write("<script>alert('Successfully Deleted Store.')</script>");
            }
            else if(storeDeleted == 2)
            {
                Response.Write("<script>alert('The store could not be deleted. Store ID does not exist.')</script>");
            }

            ViewBag.Message = "Fresh Direct Locations";

            StoreDatabase db = StoreDatabase.getInstance();
            List <Store> stores = db.getAllStores();
            if (stores.Count > 0)
            {
                List<string> templist = new List<string>();
                ViewBag.foundStores = true;
                for(int i = 0; i < stores.Count(); i++)
                {
                    string temp ="Store #: " + stores[i].storeid + "<br />" + stores[i].street + "<br />" + stores[i].city + ", " + stores[i].province + "<br />" + stores[i].phone;
                    templist.Add(temp);
                }
                ViewBag.storelist = templist;

            }
            else
            {
                ViewBag.foundStores = false;
            }

            return View();
        }


        public ActionResult DeleteStore(int storeid)
        {
            StoreDatabase db = StoreDatabase.getInstance();
            Response deletestoreresponse = db.deleteStore(storeid);
            if (deletestoreresponse.result)
            {   
                return RedirectToAction("ViewStores", new { storeDeleted = 1 });
            }
            else
            {
                return RedirectToAction("ViewStores", new { storeDeleted = 2 });
            }
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

        public ActionResult addToCart(int quantity, int sku, double price)
        {
            GroceryDatabase db = GroceryDatabase.getInstance();
            Response addtocartresponse = db.addItemtoCart(Globals.getUser(), sku, quantity, (quantity*price));
            if (addtocartresponse.result)
            {
                return RedirectToAction("Shop", new { itemoperation = 1, message = addtocartresponse.response});
            }
            else
            {
                return RedirectToAction("Shop", new { itemoperation = 2, message = addtocartresponse.response});
            }
        }

        public ActionResult ViewCart(int itemoperation  = 0, string message = "")
        {
            if (itemoperation == 1)
            {
                Response.Write("<script>alert('" + message + "')</script>");
            }
            else if (itemoperation == 2)
            {
                Response.Write("<script>alert('" + message + "')</script>");
            }

            string username = Globals.getUser();
            GroceryDatabase db = GroceryDatabase.getInstance();
            Cart tempcart = db.getCart(username);
            double carttotal = 0;

            if (tempcart.cartcontents.Count < 1)
            {
                ViewBag.EmptyCart = true;
            }
            else
            {
                ViewBag.EmptyCart = false;
                Cart mycart = new Cart();
                mycart.cartcontents = new List<Tuple<int, string, int, double>>();
                for(int i = 0; i < tempcart.cartcontents.Count(); i++)
                {
                    Response getitemnameresponse = db.getItemName(tempcart.cartcontents[i].Item1);
                    string itemname = "";
                    if (getitemnameresponse.result)
                    {
                        itemname = getitemnameresponse.response;
                    }
                    else
                    {
                        itemname = "ItemName Error";
                    }
                    mycart.AddtoCart(tempcart.cartcontents[i].Item1, itemname, tempcart.cartcontents[i].Item3, tempcart.cartcontents[i].Item4);
                    carttotal = carttotal + tempcart.cartcontents[i].Item4;
                }
                ViewBag.cartlist = mycart.cartcontents;
                ViewBag.carttotal = carttotal;
            }
            return View();
        }

        public ActionResult deleteFromCart(int sku)
        {
            GroceryDatabase db = GroceryDatabase.getInstance();
            Response deletecartitemresponse = db.deleteItemFromCart(Globals.getUser(), sku);
            if (deletecartitemresponse.result)
            {
                return RedirectToAction("ViewCart", new { itemoperation = 1, message = deletecartitemresponse.response });
            }
            else
            {
                return RedirectToAction("ViewCart", new { itemoperation = 2, message = deletecartitemresponse.response });
            }
        }

       [HttpPost]
       public ActionResult SubmitOrder(double finalcartprice) {
            GroceryDatabase db = GroceryDatabase.getInstance();
            Cart mycart = db.getCart(Globals.getUser());

            if (mycart.cartcontents.Count() < 1)
            {
                Response.Write("<script>alert('There is nothing in your cart. So no order may be submitted.')</script>");
                return RedirectToAction("ViewCart");
            }
            else
            {
                Order neworder = new Order();
                neworder.orderprice = finalcartprice;
                for (int i = 0; i < mycart.cartcontents.Count(); i++)
                {
                    neworder.AddtoOrder(mycart.cartcontents[i].Item1, mycart.cartcontents[i].Item3);
                }

                OrderDatabase orderdb = OrderDatabase.getInstance();
                Response submitorderresponse = orderdb.placeOrder(Globals.getUser(), neworder);
                if (submitorderresponse.result)
                {
                    return RedirectToAction("ViewCart", new { itemoperation = 1, message = submitorderresponse.response });
                }
                else
                {
                    return RedirectToAction("ViewCart", new { itemoperation = 2, message = submitorderresponse.response });
                }

            }
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

        public ActionResult Shop(int itemoperation = 0, string message = "")
        {
            if (itemoperation == 1)
            {
                Response.Write("<script>alert('" + message + "')</script>");
            }
            else if (itemoperation == 2)
            {
                Response.Write("<script>alert('" + message + "')</script>");
            }

            GroceryDatabase db = GroceryDatabase.getInstance();
            List<Tuple<int, string, double, int>> templist = db.getGroceryItems();
            if(templist.Count() > 0)
            {
                ViewBag.foundgroceries = true;
                ViewBag.grocerylist = templist;
            }
            else
            {
                ViewBag.foundgroceries = false;
            }
            
            return View();
        }

        public ActionResult AddGroceryItem(int itemoperation = 0, string message = "")
        {
            if (itemoperation == 1)
            {
                Response.Write("<script>alert('" + message + "')</script>");
            }
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
                return RedirectToAction("Shop", new { itemoperation = 1, message = additemresponse.response});
            }
            else
            {
                return RedirectToAction("AddGroceryItem", new { itemoperation = 1, message = additemresponse.response });
            }
        }

        public ActionResult DeleteGroceryItem(int sku)
        {
            GroceryDatabase db = GroceryDatabase.getInstance();
            Response deletegroceryitemresponse = db.deleteGroceryItem(sku);
            if (deletegroceryitemresponse.result)
            {
                return RedirectToAction("Shop", new { itemoperation = 1, message = deletegroceryitemresponse.response});
            }
            else
            {
                return RedirectToAction("Shop", new { itemoperation = 2, message = deletegroceryitemresponse.response });
            }
        }
    }
}