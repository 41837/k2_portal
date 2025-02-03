using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using SawadK2PortalDraft.Models;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Workflow.Client;
using SawadK2PortalDraft.Filters;

namespace SawadK2PortalDraft.Controllers
{
    [CheckSession]
    public class TrackingController : Controller
    {
        // GET: Tracking

        private readonly string conString = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ConnectionString.Replace("__DB_PASSWORD__", Environment.GetEnvironmentVariable("DB_Password_ReturnTax_LoanDev"));
        private readonly string K2App = System.Configuration.ConfigurationManager.ConnectionStrings["K2ServerName"].ConnectionString;
        private readonly string ADName = System.Configuration.ConfigurationManager.ConnectionStrings["ADName"].ConnectionString;

        public ActionResult Portal()
        {

            var MyTask = new List<MyTaskList>();
            var AllTask = new List<AllTaskList>();

            MyTask = getMyTask();
            AllTask = getAllTask();

            var model = new TaskListModel
            {
                MyTaskList = MyTask,
                AllTaskList = AllTask
            };

            return View(model);
        }


        public ActionResult MyTask()
        {

            var MyTask = new List<MyTaskList>();

            if (Session["AuthenType"].ToString() == "form")
            {
                MyTask = getMyTaskTypeForm();
            }
            else
            {
                MyTask = getMyTaskTypeAD();
            }


            var model = new TaskListModel
            {
                MyTaskList = MyTask
            };


            return View(model);
        }

        private List<MyTaskList> getMyTask()
        {
            Connection K2Conn = new Connection();
            K2Conn.Open("K2-App");

            SourceCode.Workflow.Client.Worklist K2WList = K2Conn.OpenWorklist();
            //var data = new List<TaskList>();
            //foreach (SourceCode.Workflow.Client.WorklistItem K2WLItem in K2WList)
            //{

            //    string DocName = K2WLItem.ProcessInstance.Folio;
            //    string SystemName = K2WLItem.ProcessInstance.Name;
            //    string ReceiveDate = K2WLItem.ProcessInstance.StartDate.ToString();
            //    string K2Link = K2WLItem.Data;

            //    data.Add(new TaskList { DocName = DocName, SystemName = SystemName, ReceiveDate = ReceiveDate, K2Link = K2Link });
            //}

            //return data.ToDataTable();



            var Task = new List<MyTaskList>();
            foreach (SourceCode.Workflow.Client.WorklistItem K2WLItem in K2WList)
            {

                string DocName = K2WLItem.ProcessInstance.Folio;
                string SystemName = K2WLItem.ProcessInstance.Name;
                DateTime ReceiveDate = K2WLItem.ProcessInstance.StartDate;
                string K2Link = K2WLItem.Data;

                Task.Add(new MyTaskList { DocName = DocName, SystemName = SystemName, ReceiveDate = ReceiveDate.Date, K2Link = K2Link });
            }

            K2Conn.Close();

            return Task;


        }

        private List<MyTaskList> getMyTaskTypeAD()
        {


            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder builder = new SCConnectionStringBuilder();
            builder.Authenticate = true;
            builder.Host = K2App;
            builder.Port = 5252;
            builder.Integrated = false;
            builder.IsPrimaryLogin = true;
            builder.SecurityLabelName = "K2";
            builder.WindowsDomain = ADName + "\\";
            builder.UserID = ADName + "\\" + Session["username"].ToString();
            //builder.WindowsDomain = "SWPCORP\\";
            //builder.UserID = "SWPCORP\\" + Session["username"].ToString();
            builder.Password = Session["password"].ToString();
            builder.CachePassword = false;


            SourceCode.Workflow.Client.Connection WorkflowClientConnection = new SourceCode.Workflow.Client.Connection();
            WorkflowClientConnection.Open(K2App, builder.ToString());

            SourceCode.Workflow.Client.Worklist K2WList = WorkflowClientConnection.OpenWorklist();

            var Task = new List<MyTaskList>();
            foreach (SourceCode.Workflow.Client.WorklistItem K2WLItem in K2WList)
            {

                string DocName = K2WLItem.ProcessInstance.Folio;
                string SystemName = K2WLItem.ProcessInstance.Name;
                DateTime ReceiveDate = K2WLItem.ProcessInstance.StartDate;
                string K2Link = K2WLItem.Data;

                Task.Add(new MyTaskList { DocName = DocName, SystemName = SystemName, ReceiveDate = ReceiveDate, K2Link = K2Link });
            }

            Task = UpdateTaskSystemNames(Task);

            WorkflowClientConnection.Close();

            return Task;


        }

        private List<MyTaskList> getMyTaskTypeForm()
        {
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder builder = new SCConnectionStringBuilder();
            builder.Authenticate = true;
            builder.Host = K2App;
            builder.Port = 5252;
            builder.Integrated = false;
            builder.IsPrimaryLogin = true;
            builder.SecurityLabelName = "K2SQL";
            builder.UserID = Session["username"].ToString();
            builder.Password = Session["password"].ToString();


            SourceCode.Workflow.Client.Connection WorkflowClientConnection = new SourceCode.Workflow.Client.Connection();
            WorkflowClientConnection.Open(K2App, builder.ToString());

            SourceCode.Workflow.Client.Worklist K2WList = WorkflowClientConnection.OpenWorklist();

            var Task = new List<MyTaskList>();
            foreach (SourceCode.Workflow.Client.WorklistItem K2WLItem in K2WList)
            {

                string DocName = K2WLItem.ProcessInstance.Folio;
                string SystemName = K2WLItem.ProcessInstance.Name;
                DateTime ReceiveDate = K2WLItem.ProcessInstance.StartDate;
                string K2Link = K2WLItem.Data;

                Task.Add(new MyTaskList { DocName = DocName, SystemName = SystemName, ReceiveDate = ReceiveDate, K2Link = K2Link });
            }


            
            Task = UpdateTaskSystemNames(Task);


            WorkflowClientConnection.Close();

            return Task;

        }

        private List<MyTaskList> UpdateTaskSystemNames(List<MyTaskList> tasks)
        {
            // Fetch the replacement map from your data source
            var replacementMap = getReplaceFlowName().AsEnumerable()
                                .ToDictionary(row => row.Field<string>("OldName"),
                                              row => row.Field<string>("NewName"));

            // Update SystemName in each task based on the replacement map
            foreach (var task in tasks)
            {
                if (replacementMap.ContainsKey(task.SystemName))
                {
                    task.SystemName = replacementMap[task.SystemName];
                }
            }

            return tasks;
        }

        private DataTable getReplaceFlowName()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    string query = "exec SPGD0105_GetReplaceFlowName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions here
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                // Consider logging the exception or rethrowing it if necessary
            }
            catch (Exception ex)
            {
                // Handle all other exceptions here
                Console.WriteLine("An error occurred: " + ex.Message);
                // Consider logging the exception or rethrowing it if necessary
            }

            return dataTable;
        }




        public ActionResult AllTask()
        {

            var AllTask = new List<AllTaskList>();

            if (Session["AuthenType"].ToString() == "form")
            {
                AllTask = getAllTaskTypeForm();
            }
            else
            {

                AllTask = getAllTaskTypeAD();
            }


            var model = new TaskListModel
            {

                AllTaskList = AllTask
            };

            return View(model);
        }

        private List<AllTaskList> getAllTask()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(conString))
            {
                //string query = "exec SPGD0201_GetListAllSystemForFirstLoad " + " @DateStart='" + "Jan 1,2021" + "',@DateEnd=" + "'Jan 1,2025' ,@Username='k2SQL:Branch01' ";
                string query = "exec SPGD0201_GetListAllSystemForFirstLoad  @Username,@SystemID,@IsComplete,@DateStart ,@DateEnd";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", "k2SQL:" + Session["username"].ToString());
                    command.Parameters.AddWithValue("@SystemID", 1);
                    command.Parameters.AddWithValue("@IsComplete", false);
                    command.Parameters.AddWithValue("@DateStart", "Jan 1,2024");
                    command.Parameters.AddWithValue("@DateEnd", "Jan 1,2025");


                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                }

                List<AllTaskList> taskList = dataTable.ToList<AllTaskList>();

                return taskList;

            }


        }

        private List<AllTaskList> getAllTaskTypeAD()
        {
            List<AllTaskList> taskList = new List<AllTaskList>();
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    string query = "exec SPGD0201_GetListAllSystemForFirstLoad @Username";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", ADName + "\\" + Session["username"].ToString());

                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);
                    }
                }

                // Convert DataTable to List
                taskList = dataTable.ToList<AllTaskList>();
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL exceptions here
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                // You might want to log the exception or handle it according to your application's needs
            }
            catch (Exception ex)
            {
                // Handle all other exceptions here
                Console.WriteLine("An error occurred: " + ex.Message);
                // You might want to log the exception or handle it according to your application's needs
            }

            return taskList;


        }

        private List<AllTaskList> getAllTaskTypeForm()
        {
            DataTable dataTable = new DataTable();
            List<AllTaskList> taskList = dataTable.ToList<AllTaskList>();

            using (SqlConnection connection = new SqlConnection(conString))
            {
                string query = "exec SPGD0201_GetListAllSystemForFirstLoad  @Username";

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", "k2SQL:" + Session["username"].ToString());


                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);
                    }

                    taskList = dataTable.ToList<AllTaskList>();

                }
                catch (SqlException sqlEx)
                {
                    // Handle SQL exceptions here
                    Console.WriteLine("SQL Error: " + sqlEx.Message);
                    // You might want to log the exception or handle it according to your application's needs
                }
                catch (Exception ex)
                {
                    // Handle all other exceptions here
                    Console.WriteLine("An error occurred: " + ex.Message);
                    // You might want to log the exception or handle it according to your application's needs
                }

                return taskList;

            }


        }

    }
}