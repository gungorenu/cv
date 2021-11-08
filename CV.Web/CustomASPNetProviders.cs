using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web
{
    public class CustomMembershipProvider : System.Web.Security.SqlMembershipProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config.Remove("connectionString");
            config.Remove("ConnectionStringName");
            config.Add("connectionString", CV.DataLayer.DataLayerConstants.SqlProviderConnectionString);
            base.Initialize(name, config);
        }
    }

    public class CustomProfileProvider : System.Web.Profile.SqlProfileProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config.Remove("connectionString");
            config.Remove("ConnectionStringName");
            config.Add("connectionString", CV.DataLayer.DataLayerConstants.SqlProviderConnectionString);
            base.Initialize(name, config);
        }
    }

    public class CustomRoleProvider : System.Web.Security.SqlRoleProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config.Remove("connectionString");
            config.Remove("ConnectionStringName");
            config.Add("connectionString", CV.DataLayer.DataLayerConstants.SqlProviderConnectionString);
            base.Initialize(name, config);
        }
    }

    public class CustomTokenRoleProvider : System.Web.Security.WindowsTokenRoleProvider
    {
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            config.Remove("connectionString");
            config.Remove("ConnectionStringName");
            config.Add("connectionString", CV.DataLayer.DataLayerConstants.SqlProviderConnectionString);
            base.Initialize(name, config);
        }
    }
}