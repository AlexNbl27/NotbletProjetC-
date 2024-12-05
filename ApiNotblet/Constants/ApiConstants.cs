using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notblet.Constants
{
    public class ApiConstants
    {
        public static string BaseApiUrl = "http://localhost:6262/api/";
        public static string Users = "users";
        public static string Clients = "clients";
        public static string Categories = "categories";
        public static string Products = "products";
        public static string Orders = "orders";
        public static string Login = Users + "/login";
        public static string Register = Users + "/register";
    }
}
