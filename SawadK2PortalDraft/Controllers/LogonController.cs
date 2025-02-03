using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;

namespace SawadK2PortalDraft.Controllers
{
    public class LogonController : Controller
    {

        //private readonly string conString = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ConnectionString;
        private readonly string conString = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ConnectionString.Replace("__DB_PASSWORD__", Environment.GetEnvironmentVariable("DB_Password_ReturnTax_LoanDev"));
        private readonly string ADName = System.Configuration.ConfigurationManager.ConnectionStrings["ADName"].ConnectionString;

        // GET: Logon
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(string Username, string Password)
        //{
        //    DataTable dtLogin = new DataTable();
        //    DataTable dtUser = new DataTable();

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    {
        //        //string queryLogin = "SPGD0103_CheckLogInK2SQL " + " @Username='" + Username + "',@Password=" + Password;
        //        string queryLogin = "SPGD0103_CheckLogInK2SQL @Username,@Password";

        //        using (SqlCommand command = new SqlCommand(queryLogin, connection))
        //        {
        //            command.Parameters.AddWithValue("@Username", Username);
        //            command.Parameters.AddWithValue("@Password", Password);
        //            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
        //            dataAdapter.Fill(dtLogin);
        //        }
        //    }

        //    if (dtLogin.Rows[0][0].ToString()  == "True")
        //    {

        //            Session["Username"] = Username.ToString();
        //            Session["Password"] = Password.ToString();
        //            Session["AuthenType"] = "K2SQL";
        //            return RedirectToAction("MyTask", "Tracking");
 
        //    }
        //    else
        //    {

        //        ViewBag.Message = "Username หรือ Password ไม่ถูกต้อง";
        //        return View();
                 
        //    }

          
        //}


        public ActionResult ADlogin()
        {
            return View();
        }
        [HttpPost]

        public ActionResult ADlogin(string Username, string Password)
        {

            if (ValidateUser(Username, Password))
            {
                FormsAuthentication.SetAuthCookie(Username, false);                 
                Session["Username"] = Username.ToString();
                Session["Password"] = Password.ToString();
                Session["AuthenType"] = "ad";
                return RedirectToAction("MyTask", "Tracking");
            }
            else
            {

                ViewBag.Message = "Username หรือ Password ไม่ถูกต้อง";
                return View();

            }

        }


        private bool ValidateUser(string username, string password)
        {
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, ADName))
                {
                    return context.ValidateCredentials(username, password);
                }
            }
            catch (Exception)
            {
                // Log or handle exceptions as needed
                return false;
            }
        }









        public ActionResult Formlogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Formlogin(string Username, string Password)
        {
            DataTable dtLogin = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    string queryLogin = "SPGD0103_CheckLogInK2SQL @Username,@Password";

                    using (SqlCommand command = new SqlCommand(queryLogin, connection))
                    {
                        command.Parameters.AddWithValue("@Username", Username);
                        command.Parameters.AddWithValue("@Password", Password);
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dtLogin);
                    }
                }

                if (dtLogin.Rows.Count > 0 && dtLogin.Rows[0][0].ToString() == "True")
                {
                    Session["Username"] = Username.ToString();
                    Session["Password"] = Password.ToString();
                    Session["AuthenType"] = "form";
                    return RedirectToAction("MyTask", "Tracking");
                }
                else
                {
                    ViewBag.Message = "Username หรือ Password ไม่ถูกต้อง";
                    return View();
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                ViewBag.Message = "เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล";
                return View();
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                Console.WriteLine("An error occurred: " + ex.Message);
                ViewBag.Message = "เกิดข้อผิดพลาดที่ไม่คาดคิด";
                return View();
            }
        }
        //public ActionResult Formlogin(string Username, string Password)
        //{
        //    DataTable dtLogin = new DataTable();
        //    DataTable dtUser = new DataTable();

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    {
        //        string queryLogin = "SPGD0103_CheckLogInK2SQL @Username,@Password";

        //        using (SqlCommand command = new SqlCommand(queryLogin, connection))
        //        {
        //            command.Parameters.AddWithValue("@Username", Username);
        //            command.Parameters.AddWithValue("@Password", Password);
        //            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
        //            dataAdapter.Fill(dtLogin);
        //        }
        //    }

        //    if (dtLogin.Rows[0][0].ToString() == "True")
        //    {

        //        Session["Username"] = Username.ToString();
        //        Session["Password"] = Password.ToString();
        //        Session["AuthenType"] = "form";
        //        return RedirectToAction("MyTask", "Tracking");

        //    }
        //    else
        //    {

        //        ViewBag.Message = "Username หรือ Password ไม่ถูกต้อง";
        //        return View();

        //    }


        //}






    }



}