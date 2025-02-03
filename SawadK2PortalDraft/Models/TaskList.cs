using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SawadK2PortalDraft.Models
{
    public class TaskList
    {
        public string DocName { get; set; }
        public string SystemName { get; set; }
        public string ReceiveDate { get; set; }
        public string K2Link { get; set; }

    }

    public class DataTablesViewModel
    {
        public DataTable dtMyTaskList { get; set; }
        public DataTable dtAllTaskList { get; set; }
    }



    public class TaskListModel
    {
        public IEnumerable<MyTaskList> MyTaskList { get; set; }
        public IEnumerable<AllTaskList> AllTaskList { get; set; }
    }

    public class MyTaskList
    {
        public string DocName { get; set; }
        public string SystemName { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string K2Link { get; set; }

    }

    public class AllTaskList
    {

        public string DocNo { get; set; }
        public string FormSubject { get; set; }
        public string ReqCreateDate { get; set; }
        public string ReqEmpName { get; set; }
        public string FormStatus { get; set; }
        public string SystemName { get; set; }
        public string LinkForm { get; set; }
        public string LinkFlow { get; set; }
        public string dtmCreated { get; set; }

    }

}