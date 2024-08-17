using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// Defines the members that logging implementations must implement.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface may optional implement a constructor that accepts 
    /// a <see cref="NameValueCollection" /> which will contain zero or more user supplied configuration properties.
    /// <para>
    /// The IBatisNetSelf.Common assembly ships with the following built-in <see cref="ILoggerFactoryAdapter" /> implementations:
    /// </para>
    ///	<list type="table">
    ///	<item><term><see cref="ConsoleOutLoggerFA" /></term><description>Writes output to Console.Out</description></item>
    ///	<item><term><see cref="TraceLoggerFA" /></term><description>Writes output to the System.Diagnostics.Trace sub-system</description></item>
    ///	<item><term><see cref="NoOpLoggerFA" /></term><description>Ignores all messages</description></item>
    ///	</list>
    /// </remarks>
    public interface ILoggerFactoryAdapter
    {
        /// <summary>
        /// Get a <see cref="ILog" /> instance by type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Get a <see cref="ILog" /> instance by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ILog GetLogger(string name);

    }
}
