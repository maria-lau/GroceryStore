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
           cartcontents = new List<Tuple<int, int>>();
        }

        public List<Tuple<int, int>> cartcontents { get; set; }

        public void AddtoCart(int SKU, int quantity)
        {
            cartcontents.Add(Tuple.Create(SKU, quantity));
        }

        public void DeleteFromCart(int SKU, int quantity)
        {
            cartcontents.RemoveAt(cartcontents.IndexOf(Tuple.Create(SKU, quantity)));
        }
    }
}