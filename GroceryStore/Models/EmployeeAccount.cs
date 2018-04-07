using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class EmployeeAccount: UserAccount
    {
        public EmployeeAccount()
        {
            type = "employee";
        }
        public int sin { get; set; }
        public string startdate { get; set; }
        public double hourlywage { get; set; }
    }
}