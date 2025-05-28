using IBatisNetSelf.Common.Logging.Impl;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private static readonly string LOGGING_SECTION = "IBatisNetSelf:Logging";

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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("ibatislog.json", optional: true, reloadOnChange: true)
                .Build();

            var _logConfig = new LogConfig();
            configuration.GetSection(LOGGING_SECTION).Bind(_logConfig);

            string _typeStr = _logConfig.LogFactoryAdapter?.Type ?? string.Empty;
            var args = _logConfig.LogFactoryAdapter?.Args ?? new Dictionary<string, string>();

            Type? factoryType = _typeStr.ToUpperInvariant() switch
            {
                "CONSOLE" => typeof(ConsoleOutLoggerFactory),
                "TRACE" => typeof(TraceLoggerFactory),
                "NOOP" => typeof(NoOpLoggerFactory),
                "FILE" => typeof(FileTxtLoggerFactory),
                _ => Type.GetType(_typeStr, throwOnError: false)
            };

            if (factoryType == null || !typeof(ILoggerFactoryAdapter).IsAssignableFrom(factoryType))
            {
                return BuildDefaultLoggerFactoryAdapter();
            }

            try
            {
                if (args.Count > 0)
                {
                    return (ILoggerFactoryAdapter)Activator.CreateInstance(factoryType, args)!;
                }

                return (ILoggerFactoryAdapter)Activator.CreateInstance(factoryType)!;
            }
            catch
            {
                return BuildDefaultLoggerFactoryAdapter();
            }
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
