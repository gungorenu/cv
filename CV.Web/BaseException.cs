using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CV.Web
{
    public class BaseException : Exception
    {
        protected BaseException(string message)
            : base(message)
        {
            Culture = Configuration.InitialCulture;
        }

        protected BaseException(string message, string culture)
            : base(message)
        {
            Culture = culture;
        }

        public string Culture { get; set; }

        public string LocalizedMessage
        {
            get { return Message; }
        }

        public static void Throw(Type exceptionType, string culture)
        {
            System.Reflection.ConstructorInfo constructor = exceptionType.GetConstructor(new Type[] { typeof(string) });
            if (constructor == null) throw new Exception("Base exception constructor could not be extracted from type: {0}" + exceptionType.FullName);
            object exc = constructor.Invoke(new object[] { culture });
            Exception exception = exc as Exception;
            if (exception == null) throw new Exception("Base exception could not be created from type: {0}" + exceptionType.FullName);
            throw exception;
        }

        public static void Throw(Type exceptionType, string culture, Dictionary<string, object> args)
        {
            System.Reflection.ConstructorInfo constructor = exceptionType.GetConstructor(new Type[] { typeof(string) });
            if (constructor == null) throw new Exception("Base exception constructor could not be extracted from type: {0}" + exceptionType.FullName);
            object exc = constructor.Invoke(new object[] { culture });
            Exception exception = exc as Exception;
            if (exception == null) throw new Exception("Base exception could not be created from type: {0}" + exceptionType.FullName);

            if (args != null)
            {
                foreach (string prop in args.Keys)
                {
                    System.Reflection.PropertyInfo propInfo = exception.GetType().GetProperty(prop);
                    if (propInfo == null) throw new Exception("Exception does not have a property specified in arguments!");
                    System.Reflection.MethodInfo propSet = propInfo.GetSetMethod();
                    if (propSet == null) throw new Exception("Exception does not have a settable property!");
                    propSet.Invoke(exception, new object[] { args[prop] });
                }
            }

            throw exception;
        }
    }
}