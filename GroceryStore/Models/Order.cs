using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class Order
    {
        public Order()
        {
            orderdate = DateTime.Today;
        }
        public DateTime orderdate { get; set; }
    }
}