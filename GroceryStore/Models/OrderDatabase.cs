using MySql.Data.MySqlClient;
using System;

namespace GroceryStore.Models
{
    /// <summary>
    /// This class is used to manipulate and read the Authentication Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class OrderDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private OrderDatabase() { }
        public static int ordernumber = 0;
        public static int deliverynumber = 0;

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static OrderDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new OrderDatabase();
            }
            return instance;
        }

        public Response placeOrder(string username, Order order)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    int newordernum = ordernumber + 1;
                    int newdeliverynum = (newordernum / 5) + 1;

                    if (newdeliverynum != deliverynumber)
                    {
                        addDelivery(newdeliverynum, order.orderdate.AddDays(3));
                    }
                    if (openConnection() == true)
                    {
                        string query = @"INSERT INTO " + dbname + @".order(username, orderdate, deliveryid) " +
                            @"VALUES('" + username + @"', '" + order.orderdate.Date.ToString("d") +
                            @"', '" + newdeliverynum + @"');";


                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        ordernumber = newordernum;
                        newdeliverynum = deliverynumber;
                        message = "New Order Added";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add order into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add order into database." +
                        " Error:" + e.Message);
                    message = e.Message;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                message = "Unable to connect to database";
            }
            return new Response(result, message);
        }

        public Response addDelivery(int deliverynum, DateTime planneddeliverydate)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    UserDatabase db = UserDatabase.getInstance();
                    string employeetodeliver = db.getNextDeliveryWorker();
                    if (openConnection() == true)
                    {
                        string query = @"INSERT INTO " + dbname + @".delivery(deliveryid, planneddeliverydate, employeetodeliver) " +
                        @"VALUES('" + deliverynum + @"', '" + planneddeliverydate.Date.ToString("d") +
                        @"', '" + employeetodeliver + @"');";


                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "New Delivery Added";
                    }
                    else
                    {
                       message = "error getting next delivery worker name.";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error:" + e.Message);
                    message = e.Message;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                message = "Unable to connect to database";
            }
            return new Response(result, message);
        }
    }
    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class OrderDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "orderdb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        private static OrderDatabase instance;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "order",
                    true,
                    "FOREIGN KEY (username) REFERENCES userdb.user(username) ON UPDATE CASCADE ON DELETE CASCADE, FOREIGN KEY (deliveryid) REFERENCES orderdb.delivery(deliveryid)",
                    new Column[]
                    {
                        new Column
                        (
                            "orderid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE",
                                 "AUTO_INCREMENT"
                            }, true
                        ),
                        new Column
                        (
                            "username", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "orderdate", "VARCHAR(20)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "deliveryid", "INT",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        )
                    }
                ),
            new Table
                (
                    dbname,
                    "delivery",
                    true,
                    "FOREIGN KEY (username) REFERENCES userdb.user(username) ON UPDATE CASCADE ON DELETE CASCADE",
                    new Column[]
                    {
                        new Column
                        (
                            "deliveryid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE",
                            }, true
                        ),
                        new Column
                        (
                            "planneddeliverydate", "VARCHAR(20)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "actualdeliverydate", "VARCHAR(20)",
                            new string[]
                            {
                                "DEFAULT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "delivered(y/n)", "VARCHAR(1)",
                            new string[]
                            {
                                "NOT NULL",
                                "DEFAULT 'n'"
                            }, false
                        ),
                         new Column
                        (
                            "employeetodeliver", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        )
                    }
                )
        };
    }
}

