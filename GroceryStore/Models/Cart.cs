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
            // Each tuple in the list will have <int SKU, int quantity>
          // cartcontents = new List<Tuple<int, string, int>>();
        }

        public List<Tuple<int, string, int>> cartcontents { get; set; }

        public void AddtoCart(int SKU, int quantity, string itemname)
        {
            cartcontents.Add(Tuple.Create(SKU, itemname, quantity));
        }

        public void DeleteFromCart(int SKU, int quantity, string itemname)
        {
            cartcontents.RemoveAt(cartcontents.IndexOf(Tuple.Create(SKU, itemname, quantity)));
        }
    }
}