using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace CV.DataLayer
{
    /// <summary>
    /// Handles event log operations
    /// </summary>
    public static class EventLog
    {
        /// <summary>
        /// Writes and event log to system
        /// </summary>
        /// <param name="type">Event type</param>
        /// <param name="message">Message format, will be passed to string.Format method</param>
        /// <param name="args">Arguments for event message</param>
        public static void WriteEventLog(System.Diagnostics.EventLogEntryType type, string message, params object[] args)
        {
            try
            {
                //: get event source name
                string eventSourceName = DataLayerConstants.EventSourceName;

                //: event source name doesnot exist?
                if (string.IsNullOrEmpty(eventSourceName)) return;
                //: try to create the event source name
                else if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
                    System.Diagnostics.EventLog.CreateEventSource(eventSourceName, string.Empty);

                //: write log
                System.Diagnostics.EventLog.WriteEntry(eventSourceName, string.Format(message, args), type);
            }
            catch
            {
                //: if still came here we can do nothing further
            }
        }

        /// <summary>
        /// Logs given exception to event log
        /// </summary>
        /// <param name="ex">Exception info</param>
        /// <returns>Error message back</returns>
        public static string WriteException(Exception ex)
        {
            try
            {
                string text = String.Format("Error: {0}\r\nSource: {2}\r\nStack Trace: {1}", ex.Message, ex.StackTrace, ex.Source);

                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    text += "\r\n" + String.Format("Error: {0}\r\nSource: {2}\r\nStack Trace: {1}", inner.Message, inner.StackTrace, inner.Source);
                    inner = inner.InnerException;
                }

                WriteEventLog(System.Diagnostics.EventLogEntryType.Error, text.Replace("{", "{{").Replace("}", "}}"));
                WriteExceptiontoDatabase(text);
                return text;
            }
            catch
            {
                return "ERROR IN ERROR";
            }
        }

        /// <summary>
        /// Logs error message to database
        /// </summary>
        /// <param name="message">Error message</param>
        public static void WriteExceptiontoDatabase(string message)
        {
            try
            {
                DbSet<Error> dbErrors = CVDbContext.DatabaseContext.Set<Error>();
                Error error = dbErrors.Create();
                error.Message = message;
                error.CreatedOn = DateTime.Now;
                dbErrors.Add(error);
                CVDbContext.DatabaseContext.SaveChanges();
            }
            catch { }
        }
    }
}
