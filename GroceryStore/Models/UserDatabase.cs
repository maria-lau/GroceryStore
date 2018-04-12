using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace GroceryStore.Models
{
    /// <summary>
    /// This class is used to manipulate and read the Authentication Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class UserDatabase : AbstractDatabase
    {
        public static int count = 0;
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private UserDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static UserDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new UserDatabase();
            }
            return instance;
        }

        public Response Login(string username, string password)
        {
            bool result = false;
            string message = "";
            string query = @"SELECT * FROM " + databaseName + @".user " +
                @"WHERE username='" + username + @"' " +
                @"AND password='" + password + @"';";

            if (openConnection())
            {

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    result = dataReader.Read();
                    dataReader.Close();
                    if (result == true)
                    {
                        message = "Login successful.";
                        Globals.setUser(username);
                    }
                    else
                    {
                        message = "Incorrect username and/or password. Please try again.";
                    }
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                result = false;
                message = "Could not connect to database.";
            }
            return new Response(result, message);
        }
        /// <summary>
        /// Attempts to insert a new user account into the database
        /// </summary>
        /// <param name="accountInfo">Contains information about the </param>
        /// <returns>A message indicating the result of the attempt</returns>
        public Response insertNewUserAccount(UserAccount accountInfo)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    // check if username is unique
                    string query = @"SELECT * FROM " + databaseName + @".user " + @"WHERE username='" + accountInfo.username + @"';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();

                    if (dataReader.Read())
                    {
                        // username exists
                        dataReader.Close();
                        result = false;
                        message = "Username is already taken. Please try another.";
                        return new Response(result, message);
                    }
                    dataReader.Close();

                    query = @"INSERT INTO " + dbname + @".user(username, password, fname, lname, street, city, province, postalcode, email, phone, type) " +
                        @"VALUES('" + accountInfo.username + @"', '" + accountInfo.password +
                        @"', '" + accountInfo.fname + @"', '" + accountInfo.lname +
                        @"', '" + accountInfo.street + @"', '" + accountInfo.city +
                        @"', '" + accountInfo.province + @"', '" + accountInfo.postalcode +
                        @"', '" + accountInfo.email + @"', '" + accountInfo.phone + @"', '" + accountInfo.type + @"');";


                    command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "Account created successfully.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new user into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to Unable to complete insert new user into database." +
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

        public Response insertNewEmployeeAccount(EmployeeAccount accountInfo)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    if (!(accountInfo.type == "manager"))
                    {
                        UserAccount useraccount = new UserAccount();
                        useraccount.username = accountInfo.username;
                        useraccount.password = accountInfo.password;
                        useraccount.fname = accountInfo.fname;
                        useraccount.lname = accountInfo.lname;
                        useraccount.street = accountInfo.street;
                        useraccount.city = accountInfo.city;
                        useraccount.province = accountInfo.province;
                        useraccount.postalcode = accountInfo.postalcode;
                        useraccount.email = accountInfo.email;
                        useraccount.phone = accountInfo.phone;
                        useraccount.type = accountInfo.type;

                        Response insertnewuser = insertNewUserAccount(useraccount);
                        if (!insertnewuser.result)
                        {
                            // insert failed
                            return new Response(result, "failure to insert new user account.");
                        }
                    }
                    if (openConnection() == true)
                    {

                        string query = @"INSERT INTO " + dbname + @".employee(username, sin, startdate, hourlywage) " +
                        @"VALUES('" + accountInfo.username + @"', '" + accountInfo.sin +
                        @"', '" + accountInfo.startdate + @"', '" + accountInfo.hourlywage + @"');";


                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "Employee Account created successfully.";
                    }
                    else
                    {
                        message = "Unable to connect to database";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new employee into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to complete insert new employee into database." +
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

        public Response insertNewManagerAccount(ManagerAccount accountInfo)
        {
            bool result = false;
            string message = "";
            if (openConnection() == true)
            {
                try
                {
                    UserAccount useraccount = new UserAccount();
                    useraccount.username = accountInfo.username;
                    useraccount.password = accountInfo.password;
                    useraccount.fname = accountInfo.fname;
                    useraccount.lname = accountInfo.lname;
                    useraccount.street = accountInfo.street;
                    useraccount.city = accountInfo.city;
                    useraccount.province = accountInfo.province;
                    useraccount.postalcode = accountInfo.postalcode;
                    useraccount.email = accountInfo.email;
                    useraccount.phone = accountInfo.phone;
                    useraccount.type = accountInfo.type;

                    Response insertnewuser = insertNewUserAccount(useraccount);
                    if (!insertnewuser.result)
                    {
                        // insert failed
                        return new Response(result, "failure to insert new user account.");
                    }

                    EmployeeAccount employeeaccount = new EmployeeAccount();
                    employeeaccount.type = "manager";
                    employeeaccount.username = accountInfo.username;
                    employeeaccount.sin = accountInfo.sin;
                    employeeaccount.hourlywage = accountInfo.hourlywage;
                    employeeaccount.startdate = accountInfo.startdate;

                    Response insertnewemployee = insertNewEmployeeAccount(employeeaccount);
                    if (!insertnewemployee.result)
                    {
                        // insert failed
                        return new Response(result, "failure to insert new employee account.");
                    }

                    if (openConnection() == true)
                    {
                        string query = @"INSERT INTO " + dbname + @".manager(username, storeid) " +
                        @"VALUES('" + accountInfo.username + @"', '" + accountInfo.storeid + @"');";


                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        result = true;
                        message = "Manager Account created successfully.";
                    }
                    else
                    {
                        message = "Unable to connect to database";
                    }
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to complete insert new manager into database." +
                        " Error :" + e.Number + e.Message);
                    message = e.Message;
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to Unable to complete insert new manager into database." +
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

        public List<UserAccount> getAllCustomers()
        {
            List<UserAccount> customers = new List<UserAccount>();
            if (openConnection())
            {
                try
                {
                    string query = "SELECT * FROM userdb.user WHERE type = 'customer';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            UserAccount temp = new UserAccount();
                            temp.username = dataReader.GetString(0);
                            temp.fname = dataReader.GetString(2);
                            temp.lname = dataReader.GetString(3);
                            temp.street = dataReader.GetString(4);
                            temp.city = dataReader.GetString(5);
                            temp.province = dataReader.GetString(6);
                            temp.postalcode = dataReader.GetString(7);
                            temp.email = dataReader.GetString(8);
                            temp.phone = dataReader.GetString(9);

                            customers.Add(temp);
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
                    Debug.consoleMsg("Unable to get all customers from database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to get all customers from database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                Debug.consoleMsg("Unable to connect to database");
            }
            return customers;
        }

        public List<EmployeeAccount> getAllEmployees()
        {
            List<EmployeeAccount> employees = new List<EmployeeAccount>();
            if (openConnection())
            {
                try
                {
                    string query = "SELECT * FROM userdb.user NATURAL JOIN userdb.employee WHERE type='employee';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            EmployeeAccount temp = new EmployeeAccount();
                            temp.username = dataReader.GetString(0);
                            temp.fname = dataReader.GetString(2);
                            temp.lname = dataReader.GetString(3);
                            temp.street = dataReader.GetString(4);
                            temp.city = dataReader.GetString(5);
                            temp.province = dataReader.GetString(6);
                            temp.postalcode = dataReader.GetString(7);
                            temp.email = dataReader.GetString(8);
                            temp.phone = dataReader.GetString(9);
                            temp.sin = dataReader.GetInt32(11);
                            temp.startdate = dataReader.GetString(12);
                            temp.hourlywage = dataReader.GetFloat(13);

                            employees.Add(temp);
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
                    Debug.consoleMsg("Unable to get all employees from database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to get all employees from database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                Debug.consoleMsg("Unable to connect to database");
            }
            return employees;
        }
        public Response deleteUserAccount(string username)
        {
            bool result = false;
            string message = "";
            if(openConnection() == true)
            {
                try
                {
                    string query = "DELETE FROM " + dbname + ".user WHERE username='" + username + "';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    result = true;
                    message = "User account deleted successfully.";
                }
                catch (MySqlException e)
                {
                    Debug.consoleMsg("Unable to delete " + username + " from database." +
                        " Error :" + e.Number + e.Message);
                    message = "Unable to delete " + username + " from database.";
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to get all employees from database." +
                        " Error:" + e.Message);
                    message = "Unable to delete " + username + " from database.";
                }
                finally
                {
                    closeConnection();
                }
            }
            return new Response(result, message);
        }
        public List<ManagerAccount> getAllManagers()
        {
            List<ManagerAccount> managers = new List<ManagerAccount>();
            if (openConnection())
            {
                try
                {
                    string query = "SELECT * FROM userdb.user NATURAL JOIN userdb.employee NATURAL JOIN userdb.manager WHERE type='manager';";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            ManagerAccount temp = new ManagerAccount();
                            temp.username = dataReader.GetString(0);
                            temp.fname = dataReader.GetString(2);
                            temp.lname = dataReader.GetString(3);
                            temp.street = dataReader.GetString(4);
                            temp.city = dataReader.GetString(5);
                            temp.province = dataReader.GetString(6);
                            temp.postalcode = dataReader.GetString(7);
                            temp.email = dataReader.GetString(8);
                            temp.phone = dataReader.GetString(9);
                            temp.sin = dataReader.GetInt32(11);
                            temp.startdate = dataReader.GetString(12);
                            temp.hourlywage = dataReader.GetFloat(13);
                            temp.storeid = dataReader.GetInt32(14);

                            managers.Add(temp);
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
                    Debug.consoleMsg("Unable to get all employees from database." +
                        " Error :" + e.Number + e.Message);
                }
                catch (Exception e)
                {
                    Debug.consoleMsg("Unable to get all employees from database." +
                        " Error:" + e.Message);
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                Debug.consoleMsg("Unable to connect to database");
            }
            return managers;
        }


        public Response getUserType(string username)
        {
            bool result = false;
            string message = "";
            string query = @"SELECT type FROM " + databaseName + @".user " +
                @"WHERE username='" + username + @"';";

            if (openConnection())
            {

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    result = dataReader.Read();
                    if (result == true)
                    {
                        message = dataReader.GetString(0);
                    }
                    else
                    {
                        message = "Username doesn't exist.";
                    }
                    dataReader.Close();
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                result = false;
                message = "Could not connect to database.";
            }
            return new Response(result, message);
        }

        public string getNextDeliveryWorker(int nextdeliveryID)
        {
            string message = "";
            string query = @"SELECT COUNT(*) FROM " + databaseName + @".user " +
                @"WHERE type='employee';";

            if (openConnection())
            {

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    int employeenum = Convert.ToInt32(command.ExecuteScalar());

                    int whosturnindex = nextdeliveryID % employeenum;
                    query = @"SELECT username FROM " + databaseName + @".user " + @"WHERE type='employee';";
                    command = new MySqlCommand(query, connection);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    // if tuples are returned
                    if (dataReader.HasRows)
                    {
                        for (int i = 0; i <= whosturnindex; i++)
                        {
                            dataReader.Read();
                        }

                        string username = dataReader.GetString(0);
                        dataReader.Close();
                        closeConnection();
                        return username;
                    }
                    // no tuples returned
                    else
                    {
                        dataReader.Close();
                        closeConnection();
                        return "error in reading the rows.";
                    }

                }
                catch (Exception e)
                {
                    message = e.Message;
                }
            }
            else
            {
                message = "Could not connect to database.";
            }
            return message;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class UserDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "userdb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        private static UserDatabase instance;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "user",
                    false,
                    "",
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
                            "password", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "fname", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "lname", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
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
                            "city", "VARCHAR(20)",
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
                            "email", "VARCHAR(50)",
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
                        ),
                        new Column
                        (
                            "type", "VARCHAR(20)",
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
                    "employee",
                    true,
                    "FOREIGN KEY (username) REFERENCES userdb.user(username) ON UPDATE CASCADE ON DELETE CASCADE",
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
                            "sin", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, false
                        ),
                        new Column
                        (
                            "startdate", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                         new Column
                        (
                            "hourlywage", "FLOAT(6)",
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
                    "manager",
                    true,
                    "FOREIGN KEY (username) REFERENCES userdb.user(username) ON UPDATE CASCADE ON DELETE CASCADE",
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
                            "storeid", "INT",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, false
                        )
                    }
                )
        };
    }
}
