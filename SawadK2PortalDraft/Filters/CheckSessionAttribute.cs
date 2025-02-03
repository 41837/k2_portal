using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SawadK2PortalDraft.Filters
{
    public class CheckSessionAttribute :ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (filterContext.HttpContext.Session["username"] == null)
            {
                // Redirect to the login page if session is not set
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Logon",
                        action = "formlogin"
                    }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}