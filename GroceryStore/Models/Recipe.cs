using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class Recipe
    {
        public Recipe(){

            ingredients = new List<Tuple<int, int>>();

        }

        public int recipeid { get; set; }
        // Each tuple in the list will have <int SKU, int quantity>
        public List<Tuple<int, int>> ingredients { get; set; }
        public int timerequired { get; set; }
        public string instructions { get; set; }
    }
}