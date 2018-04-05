using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroceryStore.Models
{
    public class ManagerAccount : EmployeeAccount
    {
        public ManagerAccount()
        {
            type = "manager";
        }

        public int storeid { get; set; }
    }
}