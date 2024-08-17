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
    /// Factory for creating <see cref="ILog" /> instances that write data to <see cref="Console.Out" />.
    /// </summary>
    public class ConsoleOutLoggerFactory : ILoggerFactoryAdapter
    {
        private Hashtable loggers = Hashtable.Synchronized(new Hashtable());
        private LogLevel level = LogLevel.All;
        private bool showDateTime = true;
        private bool showLogName = true;
        private string dateTimeFormat = string.Empty;

        /// <summary>
        /// Looks for level, showDateTime, showLogName, dateTimeFormat items from 
        /// <paramref name="properties" /> for use when the GetLogger methods are called.
        /// </summary>
        /// <param name="properties">Contains user supplied configuration information.</param>
        public ConsoleOutLoggerFactory(NameValueCollection properties)
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
        /// Get a ILog instance by <see cref="Type" />.
        /// </summary>
        /// <param name="type">Usually the <see cref="Type" /> of the current class.</param>
        /// <returns>An ILog instance that will write data to <see cref="Console.Out" />.</returns>
        public ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        /// Get a ILog instance by name.
        /// </summary>
        /// <param name="name">Usually a <see cref="Type" />'s Name or FullName property.</param>
        /// <returns>An ILog instance that will write data to <see cref="Console.Out" />.</returns>
        public ILog GetLogger(string name)
        {
            ILog _logger = this.loggers[name] as ILog;
            if (_logger == null)
            {
                _logger = new ConsoleOutLogger(name, this.level, showDateTime, showLogName, dateTimeFormat);
                this.loggers.Add(name, _logger);
            }
            return _logger;
        }

        #endregion
    }
}
