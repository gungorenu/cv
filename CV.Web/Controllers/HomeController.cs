using CV.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CV.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Overrides
        protected override string SetTitle(string action)
        {
            switch (action.ToLower())
            {
                case "index": return "Welcome to Ugur Gungoren's website!";
                case "about": return "About this webpage";
                case "self": return "Resume of Ugur Gungoren";
                default: return base.SetTitle(action);
            }
        }
        #endregion

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns>View result</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// About page
        /// </summary>
        /// <returns>View Result</returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Self page
        /// </summary>
        /// <returns>View Result</returns>
        public ActionResult Self()
        {
            return View( new SelfInfoContentListModel());
        }

    }
}