using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace GroceryStore.Models
{
    /// <summary>
    /// This class is used to manipulate and read the Authentication Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class GroceryDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private GroceryDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static GroceryDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new GroceryDatabase();
            }
            return instance;
        }

        public List<Tuple<int, string, double, int>> getGroceryItems()
        {
            List<Tuple<int, string, double, int>> items = new List<Tuple<int, string, double, int>>(); ;
            if (openConnection() == true)
            {
                try
                {
                    string query = @"SELECT * FROM " + databaseName + @".groceryitem" + @";";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    //if groceryItems were found
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            int sku = dataReader.GetInt32(0);
                            string name = dataReader.GetString(1);
                            double price = dataReader.GetDouble(3);
                            int quantity = dataReader.GetInt32(4);
                            items.Add(Tuple.Create(sku, name, price, quantity));
                        }
                        dataReader.Close();
                        return items;
                    }
                    else
                    {
                        dataReader.Close();
                        return items;
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
            return new List<Tuple<int, string, double, int>>();
        }


        public Response insertNewGroceryItem(GroceryItem item)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    // check if sku is unique
                    string query = @"SELECT * FROM " + databaseName + @".groceryitem " + @"WHERE sku='" + item.sku + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.Read())
                    {
                        // sku exists
                        dataReader.Close();
                        result = false;
                        message = "SKU is already in the system Please try another.";
                        return new Response(result, message);
                    }
                    dataReader.Close();

                    query = @"INSERT INTO " + dbname + @".groceryitem(sku, name, purchaseprice, sellingprice, quantity) " +
                        @"VALUES('" + item.sku + @"', '" + item.name +
                        @"', '" + item.purchaseprice + @"', '" + item.sellingprice +
                        @"', '" + item.quantity + @"');";


                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "Item added to inventory successfully.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new grocery item into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete insert new grocery item into database." +
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

        public Response deleteGroceryItem(int sku)
        {
            bool result = false;
            string message = "";
            if(openConnection() == true)
            {
                try
                {
                    //check that the grocery item that is to be deleted exists in the database by searching for its sku
                    string query = @"SELECT * FROM " + databaseName + @".groceryitem " + @"WHERE sku='" + sku + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (!dataReader.HasRows)
                    {
                        dataReader.Close();
                        result = false;
                        message = "Error deleting grocery item. No grocery item with this sku exists";
                    }
                    else //if the grocery item exists, delete the tuple
                    {
                        dataReader.Close();
                        query = @"DELETE FROM " + databaseName + @".groceryitem " + @"WHERE sku='" + sku + @"';";
                        command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        result = true;
                        message = "Success, The grocery item with sku # = " + sku + " has been deleted";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
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

           
        

        public Response addItemtoCart(string username, int sku, int quantity, double price)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    // check if cart already has item
                    string query = @"SELECT * FROM " + databaseName + @".cart " + @"WHERE sku='" + sku + @"' " + @"AND username='" + username + @"';";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.Read())
                    {
                        // cart has item, so just update quantity
                        int oldquantity = dataReader.GetInt32(2);
                        dataReader.Close();
                        int newquantity = oldquantity + quantity;
                        double oldprice = dataReader.GetDouble(3);
                        double newprice = oldprice + price;
                        query = "UPDATE " + databaseName + ".cart SET quantity='" + newquantity + "', totalItemPrice= '" + newprice + "' WHERE sku='" + sku + "' AND username='" + username + "';";
                        command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "Item quantity updated successfully.";
                    }
                    else
                    {
                        dataReader.Close();

                        query = @"INSERT INTO " + dbname + @".cart(username, sku, quantity, totalItemPrice) " +
                            @"VALUES('" + username + @"', '" + sku +
                            @"', " + quantity + @"," + price +  @");";


                        command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "Item added inventory successfully.";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
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
        public Cart getCart(string username)
        {
            Cart cartresult = new Cart();
            cartresult.cartcontents = new List<Tuple<int, string, int, double>>();
             if(openConnection() == true)
            {
                try
                {
                    string query = @"SELECT * FROM " + databaseName + @".cart " + @"WHERE username='" + username + @"' " + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        cartresult.cartcontents = new List<Tuple<int, string, int, double>>();
                        while (dataReader.Read())
                        {
                            int sku = dataReader.GetInt32(1);
                            int quantity= dataReader.GetInt32(2);
                            string groceryitemname = dataReader.GetString(0);
                            double price = dataReader.GetDouble(3);
                         
                            cartresult.AddtoCart(sku, groceryitemname, quantity, price);
                            
                        }
                    }
                    dataReader.Close();
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }

            return cartresult;
        }

        public Response deleteItemFromCart(string username, int sku)
        {
            bool result = false;
            string message = "";
            if(openConnection() == true)
            {
                try
                {
                    string query = @"DELETE FROM " + databaseName + @".cart " + @"WHERE sku='" + sku + @"' AND username='" + username + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "Item with sku= " + sku + " deleted from your cart.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete add cart item into database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                message = "Unable to connect to database.";
            }

            return new Response(result, message);
        }
    }


    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class GroceryDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "grocerydb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        private static GroceryDatabase instance;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "groceryitem",
                    false,
                    "",
                    new Column[]
                    {
                        new Column
                        (
                            "sku", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                        new Column
                        (
                            "name", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "purchaseprice", "DECIMAL(5, 2)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "sellingprice", "DECIMAL(5, 2)",
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
                        )
                    }
                ),
            new Table
                (
                    dbname,
                    "cart",
                    true,
                    "FOREIGN KEY (username) REFERENCES userdb.user(username) ON UPDATE CASCADE ON DELETE CASCADE, FOREIGN KEY (sku) REFERENCES grocerydb.groceryitem(sku) ON UPDATE CASCADE ON DELETE CASCADE",
                    new Column[]
                    {
                        new Column
                        (
                            "username", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
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
                            "totalItemPrice", "DECIMAL(5, 2)",
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
                    "recipe",
                    false,
                    "",
                    new Column[]
                    {
                        new Column
                        (
                            "recipeid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                        new Column
                        (
                            "instructions", "VARCHAR(10000)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "timerequired", "INT",
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
                    "recipecontainsgroceryitem",
                    true,
                    "FOREIGN KEY (sku) REFERENCES grocerydb.groceryitem(sku) ON UPDATE CASCADE ON DELETE CASCADE, FOREIGN KEY (recipeid) REFERENCES grocerydb.recipe(recipeid) ON UPDATE CASCADE ON DELETE CASCADE",
                    new Column[]
                    {
                        new Column
                        (
                            "sku", "INT",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        ),
                        new Column
                        (
                            "recipeid", "INT",
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
                        )
                    }
                )
        };
    }
}

