using CV.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CV.Web.Controllers
{
    /// <summary>
    /// Base controller for other controller types
    /// </summary>
    [HandleError(ExceptionType = typeof(Exception), Master = "Error", View = "Error")]
    public class BaseController : Controller
    {
        /// <summary>
        /// Special flag that specifies if action uses database or not
        /// </summary>
        public bool UsesDatabase { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseController()
        {
            UsesDatabase = false;
        }

        #region Overrides

        
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                return;

            var ex = filterContext.Exception ?? new Exception("No further information exists.");
            EventLog.WriteException(ex);

            filterContext.ExceptionHandled = true;
            System.Web.Mvc.HandleErrorInfo errorInfo = new System.Web.Mvc.HandleErrorInfo(
                ex,
                Convert.ToString(filterContext.RouteData.Values["controller"]),
                Convert.ToString(filterContext.RouteData.Values["action"]));

            filterContext.Result = RedirectToError(errorInfo, ex);
        }
            

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UsesDatabase)
            {
                if (!CV.DataLayer.CVDbContext.Initialize())
                    ThrowException(typeof(DatabaseConnectionLostException));
            }

            ViewBag.Title = SetTitle(filterContext.ActionDescriptor.ActionName);

            base.OnActionExecuting(filterContext);
        }

        #endregion

        #region Redirections

        /// <summary>
        /// Redirects to company index page
        /// </summary>
        /// <returns>Redirection result</returns>
        protected RedirectToRouteResult RedirectToCompanyIndex()
        {
            return base.RedirectToAction("Index", "Company");
        }

        /// <summary>
        /// Redirects to project index page
        /// </summary>
        /// <returns>Redirection result</returns>
        protected RedirectToRouteResult RedirectToProjectIndex()
        {
            return base.RedirectToAction("CommercialProjects", "Project");
        }
        /// <summary>
        /// Redirects to home page
        /// </summary>
        /// <returns>Redirection result</returns>
        protected RedirectToRouteResult RedirectToIndex()
        {
            return base.RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Redirects to login page
        /// </summary>
        /// <returns>Redirection result</returns>
        protected RedirectToRouteResult RedirectToLogon()
        {
            return base.RedirectToAction("Logon", "User");
        }

        /// <summary>
        /// Redirects to error page with valid error details
        /// </summary>
        /// <param name="info">Handled error info</param>
        /// <param name="ex">Exception itself to decide where to redirect</param>
        /// <returns>Redirection result</returns>
        protected RedirectToRouteResult RedirectToError(HandleErrorInfo info, Exception ex)
        {
            TempData["ErrorInfo"] = info;
            return base.RedirectToAction("Error", "Error");
        }

        #endregion

        #region Protected Utility Members

        protected string HandleError(Exception ex)
        {
            return CV.DataLayer.EventLog.WriteException(ex);
        }

        /// <summary>
        /// Current culture
        /// </summary>
        protected string Culture
        {
            get
            {
                if (Session["culture"] == null)
                    Session["culture"] = Configuration.InitialCulture;
                return Convert.ToString(Session["culture"]);
            }
        }

        /// <summary>
        /// Throws specified type exception
        /// </summary>
        /// <param name="type"></param>
        protected void ThrowException(Type type)
        {
            CV.Web.BaseException.Throw(type, Culture);
        }

        /// <summary>
        /// A virtual method for overriders to set title
        /// </summary>
        /// <param name="action">Action to execute is returned</param>
        /// <returns>Title (localized) is expected</returns>
        protected virtual string SetTitle(string action)
        {
            return action;
        }

        /// <summary>
        /// Flag, if we have a registered user
        /// </summary>
        public bool IsRegisteredUser
        {
            get
            {
                try
                {
                    if (Membership.GetUser() != null) return true;
                }
                catch
                {

                }
                return false;
            }
        }

        /// <summary>
        /// Flag, specifies if user is administrator
        /// </summary>
        public bool IsAdministrator
        {
            get
            {
                if (!IsRegisteredUser) return false;

                MembershipUser user = Membership.GetUser();
                if (user == null) return false;

                System.Web.Profile.ProfileBase pp = System.Web.Profile.ProfileBase.Create(user.UserName, true);
                if (pp == null) return false;
                object value = pp.GetPropertyValue("IsAdmin");
                if (value == null) return false;
                return Convert.ToBoolean(value);
            }
        }

        /// <summary>
        /// Flag that specifies if user delete operation is active
        /// </summary>
        public bool IsDeleteUserActive
        {
            get
            {
                return Configuration.IsDeleteUserActive;
            }
        }

        #endregion
    }
}