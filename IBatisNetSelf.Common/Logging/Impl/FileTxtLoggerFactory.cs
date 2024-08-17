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
    /// Summary description for  FileTxtLoggerFA.
    /// </summary>
    public class FileTxtLoggerFactory : ILoggerFactoryAdapter
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
        public FileTxtLoggerFactory(NameValueCollection properties)
        {
            try
            {
                this.level = (LogLevel)Enum.Parse(typeof(LogLevel), properties["level"], true);
            }
            catch (Exception)
            {
                this.level = LogLevel.All;
            }
            try
            {
                this.showDateTime = bool.Parse(properties["showDateTime"]);
            }
            catch (Exception)
            {
                this.showDateTime = true;
            }
            try
            {
                this.showLogName = bool.Parse(properties["showLogName"]);
            }
            catch (Exception)
            {
                this.showLogName = true;
            }
            this.dateTimeFormat = properties["dateTimeFormat"];
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
                _logger = new FileTxtLogger(name, level, showDateTime, showLogName, dateTimeFormat);
                this.loggers.Add(name, _logger);
            }
            return _logger;
        }

        #endregion
    }
}
