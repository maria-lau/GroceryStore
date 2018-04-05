using System.Threading;
using System.Web;

namespace GroceryStore.Models
{
    public class Globals
    {
        public const int patienceLevel_ms = 600000 * 3;

        public static bool isLoggedIn()
        {
            if ("Log In".Equals(getUser()))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the name of the current user
        /// </summary>
        /// <param name="user">The user name</param>
        public static void setUser(string user)
        {
            HttpContext.Current.Session["user"] = user;
        }

        /// <summary>
        /// gets the name of the current user
        /// </summary>
        /// <returns>The name of the current user</returns>
        public static string getUser()
        {
            return (string)HttpContext.Current.Session["user"];
        }

        public static string getUserType(string username)
        {
            UserDatabase db = UserDatabase.getInstance();
            Response usertype = db.getUserType(username);
            if (usertype.result)
            {
                return usertype.response;
            }
            else
            {
                return ("");
            }
        }
    }
}
