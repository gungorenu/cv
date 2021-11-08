using CV.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace CV.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            CV.DataLayer.DataLayerConstants.IsWebApplication = true;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Url.ToString().EndsWith("favicon.ico", StringComparison.CurrentCultureIgnoreCase))
                return;

            Exception unhandledException = Server.GetLastError();
            if (unhandledException != null)
                EventLog.WriteException(unhandledException);

            HttpException httpException = unhandledException as HttpException;
            if (httpException == null)
            {
                Exception innerException = unhandledException.InnerException;
                httpException = innerException as HttpException;
            }

            if (httpException != null)
            {
                int httpCode = httpException.GetHttpCode();
                Response.Redirect(Configuration.UrlForHttpError(httpCode));
            }
            else
                Response.Redirect("~/");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            MembershipUser user = null;

            try
            {
                user = Membership.GetUser();
                if (user != null)
                {
                    if (!user.IsApproved || user.IsLockedOut)
                    {
                        FormsAuthentication.SignOut();
                        Response.Redirect("~/");
                    }
                }
            }
            catch
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/");
            }
        }
    }
}
