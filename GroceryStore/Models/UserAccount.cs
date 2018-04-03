using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class UserAccount
    {
        public string username { get; set; }
        public string password { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postalcode { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string type { get; set; } = "customer";
    }
}