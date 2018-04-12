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
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private OrderDatabase() { }

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
                    string query = "SELECT * FROM " + dbname + ".order ORDER BY id DESC LIMIT 1;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    int newordernum = 0;
                    int newdeliverynum = 0;
                    if (!(dataReader.HasRows))
                    {
                        // first insert
                        dataReader.Close();
                        newordernum = 1;
                        newdeliverynum = 1;
                        Response delResponse = addDelivery(newdeliverynum, order.orderdate.AddDays(3));
                        if (openConnection() == true)
                        {
                            for (int i = 0; i < order.ordercontents.Count; i++)
                            {
                                //insert the contents of the cart into the order table
                                query = @"INSERT INTO " + dbname + @".order(orderid, sku, quantity, username, orderdate, deliveryid, orderprice)" +
                                @"VALUES(" + newordernum + @", " + order.ordercontents[i].Item1 + @", " + order.ordercontents[i].Item2 + @", '" +
                                    username + @"','" + order.orderdate.Date.ToString("d") + @"', " + newdeliverynum + @", " + Math.Round(order.orderprice,2) + @");";
                                command = new MySqlCommand(query, connection);
                                command.ExecuteNonQuery();

                                //update the quantity of the grocery items that were sold
                                query = "UPDATE grocerydb.groceryitem SET quantity= quantity-" + order.ordercontents[i].Item2 + " WHERE sku=" + order.ordercontents[i].Item1 + ";";
                                command = new MySqlCommand(query, connection);
                                command.ExecuteNonQuery();
                            }
                            result = true;
                            message = "New Order Added";
                        }
                    }
                    else
                    {
                        // not the first insert
                        dataReader.Read();
                        int currentid = dataReader.GetInt32(1);
                        newordernum = dataReader.GetInt32(1) + 1;
                        newdeliverynum = dataReader.GetInt32(6);
                        dataReader.Close();
                        if ((currentid % 3) == 0)
                        {
                            newdeliverynum += 1;
                            Response delResponse = addDelivery(newdeliverynum, order.orderdate.AddDays(3));
                        }
                        if (openConnection() == true)
                        {
                            for (int i = 0; i < order.ordercontents.Count; i++)
                            {
                                query = @"INSERT INTO " + dbname + @".order(orderid, sku, quantity, username, orderdate, deliveryid, orderprice)" +
                                    @"VALUES(" + newordernum + @", " + order.ordercontents[i].Item1 + @", " + order.ordercontents[i].Item2 + @", '" +
                                    username + @"','" + order.orderdate.Date.ToString("d") + @"', " + newdeliverynum + @", " + Math.Round(order.orderprice, 2) + @");";
                                command = new MySqlCommand(query, connection);
                                command.ExecuteNonQuery();

                                //Update the quantity of the grocery items that were sold
                                query = "UPDATE grocerydb.groceryitem SET quantity= quantity-" + order.ordercontents[i].Item2 + " WHERE sku=" + order.ordercontents[i].Item1 + ";";
                                command = new MySqlCommand(query, connection);
                                command.ExecuteNonQuery();
                            }
                            result = true;
                            message = "New Order Added";
                        }
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

        //Returns a list of delivery attributes for a specific employee
        /*
         * viewDeliveries.Item1 = deliveryid
         * viewDeliveries.Item2 = planneddeliverydate
         * viewDeliveries.Item3 = actualdeliverydate
         * viewDeliveries.Item4 = delivered
         * viewDeliveries.Item5 = employeetodeliver
         * */
        public List<Tuple<int, string, string, string, string>> viewDeliveries(string username)
        {
            List<Tuple<int, string, string, string, string>> userDeliveries = new List<Tuple<int, string, string, string, string>>();

            if(openConnection() == true)
            {
                try
                {
                    //retrieve all deliveries that belong to a specific employee
                    string query = "SELECT * FROM  " + dbname + ".delivery WHERE employeetodeliver='" + username +"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int deliveryid = dataReader.GetInt32(0);
                            string plannedDate = dataReader.GetString(1);
                            string actualDate;
                            if (dataReader.IsDBNull(2))
                            {
                                actualDate = "N/A";
                            }
                            else
                            {
                                actualDate = dataReader.GetString(2);
                            }
                            string delivered = dataReader.GetString(3);
                            string employee = dataReader.GetString(4);

                            Tuple<int, string, string, string, string> delivery = new Tuple<int, string, string, string, string>(
                                deliveryid, plannedDate, actualDate, delivered, employee);

                            userDeliveries.Add(delivery);
                        }
                        dataReader.Close();
                    }
                    else
                    {
                        dataReader.Close();
                    }

                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to view " + username + "'s deliveries." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to view " + username + "'s deliveries." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }

            return userDeliveries;
        }

        //Let employee confirm their devliery
        public Response confirmDelivery(string username, int deliveryid)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    //set delivered field to yes where the username and deliveryid match the parameters and set delivery date
                    string query = "UPDATE " + dbname + ".delivery SET delivered='y', actualdeliverydate=" + DateTime.Today.Date.ToString("d") + " WHERE employeetodeliver='" + username + "' AND deliveryid=" + deliveryid + ";";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "Delivery: " + deliveryid + " confirmed.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to confirm " + username + "'s delivery." + 
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to confirm " + username + "'s delivery." +
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
        
        //Get all the orders currently in the database and return relevant information for the manager to see
        public List<Tuple<string, int, string, double, int, string, string>> getAllOrders()
        {
            //username, orderid, orderDate, orderprice, deliveryid, assignedworker, delivered(y/n)
            List<Tuple<string, int, string, double, int, string, string>> allOrders = new List<Tuple<string, int, string, double, int, string, string>>();
            if (openConnection() == true)
            {
                try
                {
                    //SELECT username, orderid, orderdate, orderprice, orderdb.order.deliveryid, employeetodeliver, delivered
                    // FROM orderdb.`order`, orderdb.delivery WHERE orderdb.`order`.deliveryid = orderdb.delivery.deliveryid;
                    string query = "SELECT DISTINCT username, orderid, orderdate, orderprice, orderdb.order.deliveryid, employeetodeliver, delivered FROM orderdb.order, orderdb.delivery WHERE orderdb.order.deliveryid = orderdb.delivery.deliveryid;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            //get the correct attributes from the tuple being read, create tuple object, add to list AllOrders to be returned
                            string username = dataReader.GetString(0);
                            int orderid = dataReader.GetInt32(1);
                            string orderdate = dataReader.GetString(2);
                            double orderprice = dataReader.GetDouble(3);
                            int deliveryid = dataReader.GetInt32(4);
                            string empToDeliver = dataReader.GetString(5);
                            string delivered = dataReader.GetString(6);
                            Tuple<string, int, string, double, int, string, string> order = new Tuple<string, int, string, double, int, string, string>(
                                username, orderid, orderdate, orderprice, deliveryid, empToDeliver, delivered);
                            allOrders.Add(order);
                        }
                        dataReader.Close();
                    }
                    else
                    {
                        dataReader.Close();
                        closeConnection();
                        return allOrders;
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to retrieve all orders." +
                        " Error :" + e.Number + e.Message);                    
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to retrieve all orders." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }

            return allOrders;
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
                    string employeetodeliver = db.getNextDeliveryWorker(deliverynum);
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

        //Get all orders pertaining to a specific user
        public List<Tuple<int, string, double, string>> getOrders(string username)
        {
            // <orderid, orderdate, orderprice, delivered?
            List<Tuple<int, string, double, string>> orders = new List<Tuple<int, string, double, string>>();
            if (openConnection() == true)
            {
                try
                {
                    //search for all orders that belong to a user
                    string query = @"SELECT DISTINCT orderid, orderdate, deliveryid, orderprice FROM " + dbname + @".order WHERE username = '" + username + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                   
                    //query may retain many orders and they may not all belong to same delivery, store values from same tuples together
                    List<Tuple<int, string, int, double>> orderTuples = new List<Tuple<int, string, int, double>>();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int orderid = dataReader.GetInt32(0);
                            string orderDate = dataReader.GetString(1);
                            int delID = dataReader.GetInt32(2);
                            double price = dataReader.GetDouble(3);
                            Tuple<int, string, int, double> tuple = new Tuple<int, string, int, double>(orderid, orderDate, delID, price);
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
                        {
                            moreThanOneDelivery = true;
                            break;
                        }
                    }
                    if (!moreThanOneDelivery) //if orders belong to only one delivery, check if delivery has been delivered
                    {
                        query = @"SELECT delivered FROM " + dbname + @".delivery WHERE deliveryid = '" + firstDeliveryID + @"';";
                        command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        String delivered = "";
                        if (reader.HasRows)
                        {
                            reader.Read();
                            delivered = reader.GetString(0);
                            reader.Close();
                        }
                        for(int i = 0; i < orderTuples.Count; i++) //add the delivered field to the orderTuples previously collected
                        {
                            Tuple<int, string, double, string> order = new Tuple<int, string, double, string>(orderTuples[i].Item1, orderTuples[i].Item2, orderTuples[i].Item4, delivered);
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
                    Debug.consoleMsg("Unable to retrieve " + username +"'s orders." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to retrieve " + username + "'s orders." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
                return orders;
        }

        public List<Tuple<int, string, double, string>> searchMultipleDeliveries(List<Tuple<int, string, int, double>> orderTuples)
        {
            List<Tuple<int, string, double, string>> allOrders = new List<Tuple<int, string, double, string>>();
            if(openConnection() == true)
            {
                try
                {
                    //since not all orders were a part of the same delivery, must check for delivered individually
                    for(int i = 0; i < orderTuples.Count; i++)
                    {
                        string query = @"SELECT delivered FROM " + dbname + @".delivery WHERE deliveryid = '" + orderTuples[i].Item3 + @"';";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        String delivered = "";
                        if (reader.HasRows)
                        {
                            reader.Read();
                            delivered = reader.GetString(0);
                            Tuple<int, string, double, string> allOrdersTuple = new Tuple<int, string, double, string>(orderTuples[i].Item1, orderTuples[i].Item2, orderTuples[i].Item4, delivered);
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
                    "delivery",
                    false,
                    "",
                    new Column[]
                    {
                        new Column
                        (
                            "deliveryid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
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
                            "delivered", "VARCHAR(1)",
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
                ),
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
                            "id", "INT",
                            new string[]
                            {   
                                "AUTO_INCREMENT",
                                "NOT NULL"
                            }, true
                        ),
                        new Column
                        (
                            "orderid", "INT",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "sku", "INT",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
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
                            "username", "VARCHAR(50)",
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
                            "orderprice", "DECIMAL(5, 2)",
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

