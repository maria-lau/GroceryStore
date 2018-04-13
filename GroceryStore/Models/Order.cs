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
        public double orderprice { get; set; }

        public void AddtoOrder(int SKU, int quantity)
        {
            ordercontents.Add(Tuple.Create(SKU, quantity));
        }

        public void DeleteFromCart(int SKU, int quantity)
        {
            ordercontents.RemoveAt(ordercontents.IndexOf(Tuple.Create(SKU, quantity)));
        }
    }
}