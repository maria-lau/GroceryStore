using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace GroceryStore.Models
{
    /// <summary>
    /// This class is used to manipulate and read the Authentication Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class StoreDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private StoreDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static StoreDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new StoreDatabase();
            }
            return instance;
        }


        public Response insertNewStore(Store store)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    // check if storeid is unique
                    string query = @"SELECT * FROM " + databaseName + @".store " + @"WHERE storeid='" + store.storeid + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.Read())
                    {
                        // storeid exists
                        dataReader.Close();
                        result = false;
                        message = "StoreID is already taken. Please try another.";
                        return new Response(result, message);
                    }
                    dataReader.Close();

                    query = @"INSERT INTO " + dbname + @".store(storeid, street, city, province, postalcode, phone) " +
                        @"VALUES('" + store.storeid + @"', '" + store.street +
                        @"', '" + store.city + @"', '" + store.province +
                        @"', '" + store.postalcode + @"', '" + store.phone +  @"');";


                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "Store added successfully.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new store into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete insert new store into database." +
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

        public List<Store> getAllStores()
        {
            List<Store> allStores = new List<Store>();

            if (openConnection() == true)
            {
                try
                {
                    string query = @"SELECT * FROM " + databaseName + @".store " + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    //if there nothing is sent, return empty list
                    if (!dataReader.HasRows)
                    {
                        return allStores;
                    }

                    //for every tuple that is read, insert the attributes into an individual store model(object)
                    while (dataReader.Read())
                    {
                        Store currentStore = new Store();
                        currentStore.storeid = dataReader.GetInt32(0);
                        currentStore.street = dataReader.GetString(1);
                        currentStore.city = dataReader.GetString(2);
                        currentStore.province = dataReader.GetString(3);
                        currentStore.postalcode = dataReader.GetString(4);
                        currentStore.phone = dataReader.GetString(5);
                        allStores.Add(currentStore);
                    }
                    dataReader.Close();
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new store into database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete insert new store into database." +
                        " Error:" + e.Message);                    
                }
                finally
                {
                    closeConnection();
                }
            }

            return allStores;
        }
    }
    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class StoreDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "storedb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        private static StoreDatabase instance;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "store",
                    false,
                    "",
                    new Column[]
                    {
                        new Column
                        (
                            "storeid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                        new Column
                        (
                            "street", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "city", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "province", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "postalcode", "VARCHAR(7)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "phone", "VARCHAR(20)",
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

