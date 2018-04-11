using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
                        Response delResponse = addDelivery(newdeliverynum, order.orderdate.AddDays(3));
                        if (!delResponse.result)
                        {
                            return new Response(false, "Delivery could not be added, order creation aborted.");
                        }
                    }
                    if (openConnection() == true)
                    {
                        for(int i = 0; i < order.ordercontents.Count; i++)
                        {
                            string query = @"INSERT INTO " + dbname + @".order " +
                            @"VALUES(" + newordernum + @", " + order.ordercontents[i].Item1 + @", " + order.ordercontents[i].Item2 + @", '" +
                                username + @"', " + order.orderdate.Date.ToString("d") + @"', " + deliverynumber + @", " + order.orderprice + @");";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }                    
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

        public List<Tuple<int, string, string>> getOrders(string username)
        {
            List<Tuple<int, string, string>> orders = new List<Tuple<int, string, string>>();
            if (openConnection() == true)
            {
                try
                {
                    //search for all orders that belong to a user
                    string query = @"SELECT orderid, orderdate, deliveryid FROM " + dbname + @".order WHERE username = '" + username + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                   
                    //query may retain many orders and they may not all belong to same delivery, store values from same tuples together
                    List<Tuple<int, string, int>> orderTuples = new List<Tuple<int, string, int>>();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int orderid = dataReader.GetInt32(0);
                            string orderDate = dataReader.GetString(2);
                            int delID = dataReader.GetInt32(3);  
                            Tuple<int, string, int> tuple = new Tuple<int, string, int>(orderid, orderDate, delID);
                            orderTuples.Add(tuple);
                        }

                    }
                    else
                    {
                        return orders; //return empty list
                    }
                    dataReader.Close(); //close connection, no longer need anything from order table

                    int firstDeliveryID = orderTuples[0].Item3; //used to see if orders for this user belong to more than one delivery

                    bool moreThanOneDelivery = false;
                    for(int i = 0; i < orderTuples.Count; i++)
                    {
                        if (orderTuples[i].Item3 != firstDeliveryID)
                            moreThanOneDelivery = true;
                    }
                    if (!moreThanOneDelivery) //if orders belong to only one delivery, check if delivery has been delivered
                    {
                        query = @"SELECT delivered(y/n) FROM " + dbname + @".delivery WHERE deliveryid = '" + firstDeliveryID + @"';";
                        command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        String delivered = "";
                        if (reader.HasRows)
                        {
                            reader.Read();
                            delivered = reader.GetString(0);
                            reader.Close();
                        }
                        for(int i = 0; i < orderTuples.Count; i++) //add the delivered(y/n) field to the orderTuples previously collected
                        {
                            Tuple<int, string, string> order = new Tuple<int, string, string>(orderTuples[i].Item1, orderTuples[i].Item2, delivered);
                            orders.Add(order);
                        }
                        return orders;
                    }
                    else
                    {
                        orders = searchMultipleDeliveries(orderTuples);
                        return orders;
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
                return orders;
        }

        public List<Tuple<int, string, string>> searchMultipleDeliveries(List<Tuple<int, string, int>> orderTuples)
        {
            List<Tuple<int, string, string>> allOrders = new List<Tuple<int, string, string>>();
            if(openConnection() == true)
            {
                try
                {
                    //since not all orders were a part of the same delivery, must check for delivered(y/n) individually
                    for(int i = 0; i < orderTuples.Count; i++)
                    {
                        string query = @"SELECT delivered(y/n) FROM " + dbname + @".delivery WHERE deliveryid = '" + orderTuples[i].Item3 + @"';";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        String delivered = "";
                        if (reader.HasRows)
                        {
                            reader.Read();
                            delivered = reader.GetString(0);
                            Tuple<int, string, string> allOrdersTuple = new Tuple<int, string, string>(orderTuples[i].Item1, orderTuples[i].Item2, delivered);
                            allOrders.Add(allOrdersTuple);
                        }
                        reader.Close();
                    }
                    
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add delivery into database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }

            return allOrders;
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
                                "NOT NULL"
                            }, true
                        ),
                         new Column
                        (
                            "sku", "INT",
                            new string[]
                            {
                                "NOT NULL"
                            }, true
                        ),
                          new Column
                        (
                            "quantity", "INT",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
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
                        ),
                          new Column
                        (
                            "orderPrice", "DECIMAL(5, 2)",
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

