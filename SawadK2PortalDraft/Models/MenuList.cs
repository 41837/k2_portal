using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SawadK2PortalDraft.Models
{
     public partial class MenuList
    {
        public int MenuID { get; set; }
        public string MenuNameTH { get; set; }
        public string MenuNameEN { get; set; }
        public Nullable<int> MenuLevel { get; set; }
        public Nullable<int> MenuOrder { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Icon { get; set; }
        public string UrlDev { get; set; }
        public string UrlPro { get; set; }
    }
}