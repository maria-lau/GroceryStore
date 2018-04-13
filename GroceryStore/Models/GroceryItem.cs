using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class GroceryItem
    {
        public int sku { get; set; }
        public string name { get; set; }
        public double purchaseprice { get; set; }
        public double sellingprice { get; set; }
        public int quantity { get; set; }
    }
}