using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging.Impl
{
    /// <summary>
    /// Summary description for  TraceLoggerFA.
    /// </summary>
    public class TraceLoggerFactory : ILoggerFactoryAdapter
    {
        private Hashtable loggers = Hashtable.Synchronized(new Hashtable());
        private LogLevel level = LogLevel.All;
        private bool showDateTime = true;
        private bool showLogName = true;
        private string dateTimeFormat = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="properties"></param>
        public TraceLoggerFactory(Dictionary<string, string> properties)
        {
            try
            {
                level = (LogLevel)Enum.Parse(typeof(LogLevel), properties["level"], true);
            }
            catch (Exception)
            {
                level = LogLevel.All;
            }
            try
            {
                showDateTime = bool.Parse(properties["showDateTime"]);
            }
            catch (Exception)
            {
                showDateTime = true;
            }
            try
            {
                showLogName = bool.Parse(properties["showLogName"]);
            }
            catch (Exception)
            {
                showLogName = true;
            }
            dateTimeFormat = properties["dateTimeFormat"];
        }

        #region ILoggerFactoryAdapter Members

        /// <summary>
        /// Get a ILog instance by type 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        /// Get a ILog instance by type name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILog GetLogger(string name)
        {
            ILog _logger = this.loggers[name] as ILog;
            if (_logger == null)
            {
                _logger = new TraceLogger(name, level, showDateTime, showLogName, dateTimeFormat);
                this.loggers.Add(name, _logger);
            }
            return _logger;
        }

        #endregion
    }
}
