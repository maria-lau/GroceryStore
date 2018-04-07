using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class Store
    {
        public int storeid { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postalcode { get; set; }
        public string phone { get; set; }
    }
}