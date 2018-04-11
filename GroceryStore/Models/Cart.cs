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
            // Each tuple in the list will have <int SKU, string name, int quantity, double price>
           // cartcontents = new List<Tuple<int, string, int, double price>>();
        }

        public List<Tuple<int, string, int, double>> cartcontents { get; set; } = null;

        public void AddtoCart(int SKU, string itemname, int quantity, double totalItemPrice)
        {
            cartcontents.Add(Tuple.Create(SKU, itemname, quantity, totalItemPrice));
        }

        public void DeleteFromCart(int SKU, string itemname, int quantity, double totalItemPrice)
        {
            cartcontents.RemoveAt(cartcontents.IndexOf(Tuple.Create(SKU, itemname, quantity, totalItemPrice)));
        }
    }
}