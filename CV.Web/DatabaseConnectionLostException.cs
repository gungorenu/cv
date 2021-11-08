using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web
{
    public class DatabaseConnectionLostException : BaseException
    {
        public DatabaseConnectionLostException(string culture)
            : base("DatabaseConnectionLost", culture)
        {
        }
    }
}