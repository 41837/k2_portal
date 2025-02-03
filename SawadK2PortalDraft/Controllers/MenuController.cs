using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SawadK2PortalDraft.Models;
using System.Web.DynamicData;

namespace SawadK2PortalDraft.Controllers
{
    public class MenuController : Controller
    {
        private readonly string conString = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ConnectionString.Replace("__DB_PASSWORD__", Environment.GetEnvironmentVariable("DB_Password_ReturnTax_LoanDev"));
        private readonly string ADName = System.Configuration.ConfigurationManager.ConnectionStrings["ADName"].ConnectionString;

        // GET: Menu
        //public ActionResult Index()
        //{
        //    //var menu = new List<SPDM0102_GetMenu_Result>();
        //    //var dbo = new ContractorPortalEntities();
        //    //menu = dbo.SPDM0102_GetMenu().ToList();
        //    //foreach (var item in menu)
        //    //{
        //    //    item.UrlDev += item.UrlDev.Contains("&Company=") ? Session[SessionConstants.CompanyGUID].ToString() : "";
        //    //}


        //    DataTable dataTable = new DataTable();
        //    using (SqlConnection connection = new SqlConnection(conString))
        //    {
        //        string query = "exec SPGD0102_GetMenu ";
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {

        //            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
        //            dataAdapter.Fill(dataTable);
        //        }
        //    }

        //    return PartialView("_Menu", dataTable);
        //}


        //public ActionResult Index()
        //{

        //    List<MenuList> list = new List<MenuList>();

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    {
        //        string query = "exec SPGD0102_GetMenu"; // Adjust query as needed

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            connection.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    MenuList model = new MenuList
        //                    {
        //                        MenuID = Convert.ToInt32(reader["MenuID"]),
        //                        MenuNameTH = reader["MenuNameTH"].ToString(),
        //                        MenuNameEN = reader["MenuNameEN"].ToString(),
        //                        MenuLevel = Convert.ToInt32(reader["MenuLevel"]),
        //                        MenuOrder = Convert.ToInt32(reader["MenuOrder"]),
        //                        ParentID = Convert.ToInt32(reader["ParentID"]),
        //                        Icon = reader["Icon"].ToString(),
        //                        UrlDev = reader["UrlDev"].ToString(),
        //                        UrlPro = reader["UrlPro"].ToString()

        //                    };
        //                    list.Add(model);
        //                }
        //            }
        //        };
        //    }
        //    return PartialView("_Menu", list);
        //}

        public ActionResult Index()
        {
            List<MenuList> list = new List<MenuList>();

            try
            {
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    //string query = "exec SPGD0102_GetMenu"; // Adjust query as needed
                    string query = "exec SPGD0106_GetMenubyUser @UserName"; // Adjust query as needed

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        if (Session["AuthenType"].ToString() == "form") //k2sql
                        {
                            command.Parameters.AddWithValue("@UserName", "k2sql:" + Session["username"].ToString());
                        }
                        else //AD
                        {
                            command.Parameters.AddWithValue("@UserName", Session["username"].ToString());
                        }


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                           

                            while (reader.Read())
                            {
                                MenuList model = new MenuList
                                {
                                    MenuID = Convert.ToInt32(reader["MenuID"]),
                                    MenuNameTH = reader["MenuNameTH"].ToString(),
                                    MenuNameEN = reader["MenuNameEN"].ToString(),
                                    MenuLevel = Convert.ToInt32(reader["MenuLevel"]),
                                    MenuOrder = Convert.ToInt32(reader["MenuOrder"]),
                                    ParentID = Convert.ToInt32(reader["ParentID"]),
                                    Icon = reader["Icon"].ToString(),
                                    UrlDev = reader["UrlDev"].ToString(),
                                    UrlPro = reader["UrlPro"].ToString()
                                };
                                list.Add(model);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                ViewBag.ErrorMessage = "An error occurred while retrieving the menu data.";
                return View("Error");
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                Console.WriteLine("An error occurred: " + ex.Message);
                ViewBag.ErrorMessage = "An unexpected error occurred.";
                return View("Error");
            }

            return PartialView("_Menu", list);
        }



    }
}