using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CV.Web.Controllers
{
    /// <summary>
    /// Error controller for custom error views
    /// </summary>
    public class ErrorController : BaseController
    {
        #region Static Members
        private static List<string> knownErrors;
        private static List<string> logoutErrors;

        /// <summary>
        /// Logout error list, each error type needs a seperate view, these errorswill logout current user also
        /// </summary>
        internal static List<string> LogoutErrors
        {
            get
            {
                if (logoutErrors == null)
                {
                    logoutErrors = new List<string>();
                    logoutErrors.Add("HashFailure");
                }
                return logoutErrors;
            }
        }

        /// <summary>
        /// Known error list, each error type needs a seperate view
        /// </summary>
        internal static List<string> KnownErrors
        {
            get
            {
                if (knownErrors == null)
                {
                    knownErrors = new List<string>();
                    knownErrors.Add("DatabaseConnectionLost");
                    knownErrors.Add("HashFailure");
                }
                return knownErrors;
            }
        }
        #endregion

        #region Overrides
        protected override string SetTitle(string action)
        {
            return "Error";
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (TempData.ContainsKey("ErrorInfo"))
            {
                HandleErrorInfo info = TempData["ErrorInfo"] as HandleErrorInfo;
                string excName = info.Exception.GetType().Name;

                if (excName.EndsWith("Exception") && !"Exception".Equals(excName, StringComparison.CurrentCultureIgnoreCase))
                    excName = excName.Substring(0, excName.Length - 9);

                if (LogoutErrors.Contains(excName))
                {
                    FormsAuthentication.SignOut();
                }

                if (KnownErrors.Contains(excName))
                    filterContext.Result = View(excName, info);
            }
            else filterContext.Result = RedirectToIndex();
        }
        #endregion

        #region Actions
        /// <summary>
        /// Handles error and shows error information
        /// </summary>
        /// <returns>Action result</returns>
        public ActionResult Error()
        {
            if (TempData.ContainsKey("ErrorInfo"))
            {
                HandleErrorInfo info = TempData["ErrorInfo"] as HandleErrorInfo;
                TempData.Remove("ErrorInfo");
                return View(info);
            }

            return RedirectToIndex();
        }

        /// <summary>
        /// Http error action
        /// </summary>
        /// <param name="code">Http Error code</param>
        /// <returns>View as result</returns>
        public ActionResult HttpError(int code)
        {
            return View();
        }

        /// <summary>
        /// Index for errors, not supposed to happen, directed to application index
        /// </summary>
        /// <returns>View as result</returns>
        public ActionResult Index()
        {
            return RedirectToIndex();
        }
        #endregion
    }
}