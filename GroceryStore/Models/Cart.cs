using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class Cart
    {
        public Cart()
        {
            // Each tuple in the list will have <int SKU, string name, int quantity>
           // cartcontents = new List<Tuple<int, string, int>>();
        }

        public List<Tuple<int, string, int>> cartcontents { get; set; } = null;

        public void AddtoCart(int SKU, string itemname, int quantity)
        {
            cartcontents.Add(Tuple.Create(SKU, itemname, quantity));
        }

        public void DeleteFromCart(int SKU, string itemname, int quantity)
        {
            cartcontents.RemoveAt(cartcontents.IndexOf(Tuple.Create(SKU, itemname, quantity)));
        }
    }
}