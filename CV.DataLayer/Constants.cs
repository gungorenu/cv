using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.DataLayer
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class DataLayerConstants
    {
        private static string AssemblyName
        {
            get
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return path.Substring(path.LastIndexOf('\\') + 1);
            }
        }

        private static System.Configuration.Configuration GetConfiguration()
        {
            string path = string.Empty;
            if (IsWebApplication) path = System.Web.Hosting.HostingEnvironment.MapPath(string.Format("~/bin/{0}", AssemblyName));
            else
            {
                path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                path = string.Format("{0}{1}", path.Substring(0, path.LastIndexOf("\\") + 1), AssemblyName);
            }
            return System.Configuration.ConfigurationManager.OpenExeConfiguration(path);
        }

        private static string ReadConnectionString(string connectionStrKey)
        {
            System.Configuration.Configuration conf = GetConfiguration();
            return conf.ConnectionStrings.ConnectionStrings[connectionStrKey].ConnectionString;
        }

        private static string ReadFromConfig(string key, string defaultValue = "")
        {
            try
            {
                System.Configuration.Configuration conf = GetConfiguration();
                System.Configuration.KeyValueConfigurationElement ele = conf.AppSettings.Settings[key];
                string value = ele.Value;
                if (string.IsNullOrEmpty(value)) return defaultValue;
                value = value.Trim();
                return value;
            }
            catch (Exception ex)
            {
                if (key != "EventSourceName")
                    EventLog.WriteEventLog(System.Diagnostics.EventLogEntryType.Error, "Reading key '{0}' from config failed! Error: {1}", key, ex.Message);
                return defaultValue;
            }
        }

        /// <summary>
        /// CV App Database connection string
        /// </summary>
        public static string ConnectionStringName { get { return ReadFromConfig("ConnectionStringName"); } }

        /// <summary>
        /// Full connection string 
        /// </summary>
        public static string ConnectionString { get { return ReadConnectionString(ConnectionStringName); } }

        /// <summary>
        /// Sql provider supported connection string value
        /// </summary>
        public static string SqlProviderConnectionString
        {
            get
            {
                string cs = ConnectionString;
                cs = cs.Substring(cs.IndexOf("\""));
                return cs.Trim('"');
            }
        }

        /// <summary>
        /// CV App Database event source name
        /// </summary>
        public static string EventSourceName { get { return ReadFromConfig("EventSourceName"); } }

        /// <summary>
        /// Flag that specifies if the application is 
        /// </summary>
        public static bool IsWebApplication { get; set; }

        /// <summary>
        /// Special company reserved for personal projects
        /// </summary>
        public static string PersonalProjectCompany { get { return "<<PERSONAL>>"; } }
    }
}
