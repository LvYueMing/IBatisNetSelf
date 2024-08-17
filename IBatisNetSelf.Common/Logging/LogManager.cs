using IBatisNetSelf.Common.Logging.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// Uses the specified <see cref="ILoggerFactoryAdapter" /> to create <see cref="ILog" /> instances
    /// that are used to log messages. Inspired by log4net.
    /// </summary>
    public sealed class LogManager
    {
        private static ILoggerFactoryAdapter adapter = null;
        private static object loadLock = new object();
        private static readonly string LOGGING_SECTION = "IBatisNetSelf/Logging";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager" /> class. 
        /// </summary>
        /// <remarks>
        /// Uses a private access modifier to prevent instantiation of this class.
        /// </remarks>
        private LogManager()
        { }


        /// <summary>
        /// Gets or sets the adapter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The IBatisNe .Common assembly ships with the following built-in <see cref="ILoggerFactoryAdapter" /> implementations:
        /// </para>
        ///	<list type="table">
        ///	<item><term><see cref="ConsoleOutLoggerFactory" /></term><description>Writes output to Console.Out</description></item>
        ///	<item><term><see cref="TraceLoggerFactory" /></term><description>Writes output to the System.Diagnostics.Trace sub-system</description></item>
        ///	<item><term><see cref="NoOpLoggerFactory" /></term><description>Ignores all messages</description></item>
        ///	</list>
        /// </remarks>
        /// <value>The adapter.</value>
        public static ILoggerFactoryAdapter Adapter
        {
            get
            {
                if (adapter == null)
                {
                    lock (loadLock)
                    {
                        if (adapter == null)
                        {
                            adapter = BuildLoggerFactoryAdapter();
                        }
                    }
                }
                return adapter;
            }
            set
            {
                lock (loadLock)
                {
                    adapter = value;
                }
            }

        }


        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            return Adapter.GetLogger(type);
        }


        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            return Adapter.GetLogger(name);
        }


        /// <summary>
        /// Builds the logger factory adapter.
        /// </summary>
        /// <returns></returns>
        private static ILoggerFactoryAdapter BuildLoggerFactoryAdapter()
        {
            LogSetting _setting = null;
            try
            {
                _setting = (LogSetting)System.Configuration.ConfigurationManager.GetSection(LOGGING_SECTION);
            }
            catch (Exception ex)
            {
                ILoggerFactoryAdapter _defaultFactory = BuildDefaultLoggerFactoryAdapter();
                ILog _log = _defaultFactory.GetLogger(typeof(LogManager));
                _log.Warn("Unable to read configuration. Using default logger.", ex);
                return _defaultFactory;
            }

            if (_setting != null && !typeof(ILoggerFactoryAdapter).IsAssignableFrom(_setting.FactoryAdapterType))
            {
                ILoggerFactoryAdapter _defaultFactory = BuildDefaultLoggerFactoryAdapter();
                ILog _log = _defaultFactory.GetLogger(typeof(LogManager));
                _log.Warn("Type " + _setting.FactoryAdapterType.FullName + " does not implement ILoggerFactoryAdapter. Using default logger");
                return _defaultFactory;
            }

            ILoggerFactoryAdapter _instance = null;

            if (_setting != null)
            {
                if (_setting.Properties.Count > 0)
                {
                    try
                    {
                        object[] args = { _setting.Properties };

                        _instance = (ILoggerFactoryAdapter)Activator.CreateInstance(_setting.FactoryAdapterType, args);
                    }
                    catch (Exception ex)
                    {
                        ILoggerFactoryAdapter _defaultFactory = BuildDefaultLoggerFactoryAdapter();
                        ILog _log = _defaultFactory.GetLogger(typeof(LogManager));
                        _log.Warn("Unable to create instance of type " + _setting.FactoryAdapterType.FullName + ". Using default logger.", ex);
                        return _defaultFactory;
                    }
                }
                else
                {
                    try
                    {
                        _instance = (ILoggerFactoryAdapter)Activator.CreateInstance(_setting.FactoryAdapterType);
                    }
                    catch (Exception ex)
                    {
                        ILoggerFactoryAdapter defaultFactory = BuildDefaultLoggerFactoryAdapter();
                        ILog log = defaultFactory.GetLogger(typeof(LogManager));
                        log.Warn("Unable to create instance of type " + _setting.FactoryAdapterType.FullName + ". Using default logger.", ex);
                        return defaultFactory;
                    }
                }
            }
            else
            {
                ILoggerFactoryAdapter defaultFactory = BuildDefaultLoggerFactoryAdapter();
                ILog log = defaultFactory.GetLogger(typeof(LogManager));
                log.Warn("Unable to read configuration IBatisNet/logging. Using default logger.");
                return defaultFactory;
            }

            return _instance;
        }


        /// <summary>
        /// Builds the default logger factory adapter.
        /// </summary>
        /// <returns></returns>
        private static ILoggerFactoryAdapter BuildDefaultLoggerFactoryAdapter()
        {
            return new NoOpLoggerFactory();
        }
    }
}
