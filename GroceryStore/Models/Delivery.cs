using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class Delivery
    {
        public Delivery(){
            planneddeliverydate = DateTime.Today.AddDays(3);
        }
        public DateTime planneddeliverydate { get; set; }
        public DateTime actualdeliverydate { get; set; }
        public bool delivered { get; set; }
        public string employeetodeliver { get; set; }
    }
}