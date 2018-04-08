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
                    message = "Item added inventory successfully.";
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

        public Response addItemtoCart(string username, int sku, int quantity)
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
                        query = "UPDATE " + databaseName + ".cart SET quantity='" + newquantity + "' WHERE sku='" + sku + "' AND username='" + username + "';";
                        command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "Item quantity updated successfully.";
                    }
                    else
                    {
                        dataReader.Close();

                        query = @"INSERT INTO " + dbname + @".cart(username, sku, quantity) " +
                            @"VALUES('" + username + @"', '" + sku +
                            @"', '" + quantity + @"');";


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
             if(openConnection() == true)
            {
                try
                {
                    string query = @"SELECT * FROM " + databaseName + @".cart " + @"WHERE username='" + username + @"' " + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        cartresult.cartcontents = new List<Tuple<int, string, int>>();
                        while (dataReader.Read())
                        {
                            int sku = dataReader.GetInt32(1);
                            int quantity= dataReader.GetInt32(2);
                            string groceryitemname = dataReader.GetString(0);
                            
                         
                            cartresult.AddtoCart(sku, groceryitemname, quantity);
                            
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
                    "FOREIGN KEY (sku) REFERENCES grocerydb.groceryitems(sku) ON UPDATE CASCADE ON DELETE CASCADE, FOREIGN KEY (recipeid) REFERENCES grocerydb.recipe(recipeid) ON UPDATE CASCADE ON DELETE CASCADE",
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

