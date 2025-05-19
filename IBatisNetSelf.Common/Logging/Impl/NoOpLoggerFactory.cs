using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging.Impl
{
    /// <summary>
    /// Factory for creating "no operation" loggers that do nothing and whose Is*Enabled properties always 
    /// return false.
    /// </summary>
    /// <remarks>
    /// This factory creates a single instance of <see cref="NoOpLogger" /> and always returns that 
    /// instance whenever an <see cref="ILog" /> instance is requested.
    /// </remarks>
    public sealed class NoOpLoggerFactory : ILoggerFactoryAdapter
    {
        private ILog nopLogger = new NoOpLogger();

        /// <summary>
        /// Constructor
        /// </summary>
        public NoOpLoggerFactory()
        {
            // empty
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NoOpLoggerFactory(Dictionary<string, string> properties)
        {
            // empty
        }

        #region ILoggerFactoryAdapter Members

        /// <summary>
        /// Get a ILog instance by type 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILog GetLogger(Type type)
        {
            return nopLogger;
        }

        /// <summary>
        /// Get a ILog instance by type name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ILog ILoggerFactoryAdapter.GetLogger(string name)
        {
            return nopLogger;

        }

        #endregion
    }
}
