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
            // Each tuple in the list will have <int SKU, int quantity>
            ordercontents = new List<Tuple<int, int>>();
        }
        public DateTime orderdate { get; set; }
        public List<Tuple<int, int>> ordercontents { get; set; }
    }
}