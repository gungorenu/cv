using CV.DataLayer;
using System;
using System.Net;

namespace CV.Web
{
    public static class Configuration
    {
        private static string ReadFromConfig(string key, string defaultValue = "")
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(value)) return defaultValue;
                value = value.Trim();
                return value;
            }
            catch (Exception ex)
            {
                EventLog.WriteEventLog(System.Diagnostics.EventLogEntryType.Error, "Reading key '{0}' from config failed! Error: {1}", key, ex.Message);
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the Http Error page
        /// </summary>
        public static string HttpErrorPage { get { return "~/Error/HttpError"; } }

        /// <summary>
        /// Calculates the Http Error page for given code
        /// </summary>
        /// <param name="httpCode">Http Error code</param>
        /// <returns>Url to redirect on http error</returns>
        public static string UrlForHttpError(int httpCode)
        {
            switch (httpCode)
            {
                case (int)HttpStatusCode.NotFound:
                case (int)HttpStatusCode.Forbidden:
                case (int)HttpStatusCode.Unauthorized:
                    return HttpErrorPage + "?code=" + httpCode;
                default:
                    return "~/";
            }
        }

        /// <summary>
        /// Flag that enables registration
        /// </summary>
        public static bool IsRegistrationEnabled { get { return Convert.ToBoolean(ReadFromConfig("RegistrationEnabled", "false")); } }

        /// <summary>
        /// Flag that specifies if new user is admin
        /// </summary>
        public static bool IsNewUserAdmin { get { return Convert.ToBoolean(ReadFromConfig("NewUserAdmin", "false")); } }

        /// <summary>
        /// Flag that specifies if delete user is active
        /// </summary>
        public static bool IsDeleteUserActive { get { return Convert.ToBoolean(ReadFromConfig("DeleteUserActive", "false")); } }

        /// <summary>
        /// CV database initial culture
        /// </summary>
        public static string InitialCulture { get { return ReadFromConfig("InitialCulture", "en-US"); } }

        /// <summary>
        /// Email address for contact
        /// </summary>
        public static string EmailAddress { get { return ReadFromConfig("ContactEmail", "___"); } }

        /// <summary>
        /// Linkedin contact
        /// </summary>
        public static string Linkedin { get { return ReadFromConfig("Linkedin", "___"); } }
              

    }
}