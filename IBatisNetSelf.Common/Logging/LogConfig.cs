using IBatisNetSelf.Common.Logging.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// Used in an application's configuration file (appsettings.json) to configure the logging subsystem.
    /// </summary>
    /// <remarks>
    /// <example>
    /// An example configuration section that writes IBatisNet messages to the Console using the built-in Console Logger.
    ///{
    ///  "IBatisNetSelf": {
    ///    "Logging": {
    ///      "LogFactoryAdapter": {
    ///        "type": "IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNetSelf.Common",
    ///        "args": {
    ///          "showLogName": "true",
    ///          "showDataTime": "true",
    ///          "level": "Info",  // 覆盖 ALL，只保留最后一个值
    ///          "dateTimeFormat": "yyyy/MM/dd HH:mm:ss:fff"
    ///        }
    ///      }
    ///    }
    ///  }
    ///}
    //<para>
    /// The following aliases are recognized for the type attribute of logFactoryAdapter: 
    /// </para>
    /// <list type="table">
    /// <item><term>CONSOLE</term><description>Alias for IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNetSelf.Common</description></item>
    /// <item><term>TRACE</term><description>Alias for IBatisNetSelf.Common.Logging.Impl.TraceLoggerFA, IBatisNetSelf.Common</description></item>
    /// <item><term>NOOP</term><description>Alias IBatisNetSelf.Common.Logging.Impl.NoOpLoggerFA, IBatisNetSelf.Common</description></item>
    /// </list>
    /// </remarks>
    /// 

    public class LogConfig
    {
        public LogFactoryAdapterConfig LogFactoryAdapter { get; set; } = new();
    }

    public class LogFactoryAdapterConfig
    {
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, string> Args { get; set; } = new();
    }

}
