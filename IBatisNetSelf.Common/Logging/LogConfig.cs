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
    /// 用于应用程序的配置文件（如 ibatislog.json）中配置日志子系统。
    /// </summary>
    /// <remarks>
    /// <example>
    /// 一个示例配置片段，使用内置的控制台日志记录器（Console Logger）将 IBatisNet 消息输出到控制台：
    /// {
    ///   "IBatisNetSelf": {
    ///     "Logging": {
    ///       "LogFactoryAdapter": {
    ///         "type": "console",
    ///         "args": {
    ///           "showLogName": "true",              // 是否显示日志名称
    ///           "showDataTime": "true",             // 是否显示时间戳
    ///           "level": "Info",                    // 日志级别(All、Debug、Info、Warn、Error、Fatal、Off)
    ///           "dateTimeFormat": "yyyy/MM/dd HH:mm:ss:fff"  // 时间格式
    ///         }
    ///       }
    ///     }
    ///   }
    /// }
    /// <para>
    /// 以下别名可用于 logFactoryAdapter 的 type 属性：
    /// </para>
    /// <list type="table">
    /// <item><term>console</term><description>别名，实际类型为 IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFactory, IBatisNetSelf.Common</description></item>
    /// <item><term>trace</term><description>别名，实际类型为 IBatisNetSelf.Common.Logging.Impl.TraceLoggerFactory, IBatisNetSelf.Common</description></item>
    /// <item><term>noop</term><description>别名，实际类型为 IBatisNetSelf.Common.Logging.Impl.NoOpLoggerFactory, IBatisNetSelf.Common</description></item>
    /// <item><term>file</term><description>别名，实际类型为 IBatisNetSelf.Common.Logging.Impl.FileTxtLoggerFactory, IBatisNetSelf.Common</description></item>
    /// </list>
    /// </remarks>


    // 日志配置类，表示整个 Logging 节点
    public class LogConfig
    {
        // 表示日志工厂适配器配置，即 "LogFactoryAdapter" 部分
        public LogFactoryAdapterConfig LogFactoryAdapter { get; set; } = new();
    }


    // 表示 LogFactoryAdapter 节点的配置项
    public class LogFactoryAdapterConfig
    {
        // 表示适配器类型，例如 "IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNetSelf.Common"
        public string Type { get; set; } = string.Empty;

        // 表示传给适配器构造函数的参数键值对，如 showLogName、level 等
        public Dictionary<string, string> Args { get; set; } = new();
    }


}
