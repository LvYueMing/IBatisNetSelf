using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging.Impl
{
    /// <summary>
    /// Logger that sends output to the txtFile.
    /// </summary>
    public class FileTxtLogger : AbstractLogger
    {
        private bool showDateTime = false;
        private bool showLogName = false;
        private string logName = string.Empty;
        private LogLevel currentLogLevel = LogLevel.All;
        private string dateTimeFormat = string.Empty;
        private bool hasDateTimeFormat = false;

        /// <summary>
        /// Creates a new instance of the FileTxtLogger.
        /// </summary>
        /// <param name="logName">The name for this instance (usually the fully qualified class name).</param>
        /// <param name="logLevel">
        ///	The logging threshold. Log messages created with a <see cref="LogLevel" />
        /// beneath this threshold will not be logged.
        /// </param>
        /// <param name="showDateTime">Include the current time in the log message </param>
        /// <param name="showLogName">Include the instance name in the log message</param>
        /// <param name="dateTimeFormat">The date and time format to use in the log message </param>
        public FileTxtLogger(string logName, LogLevel logLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            this.logName = logName;
            this.currentLogLevel = logLevel;
            this.showDateTime = showDateTime;
            this.showLogName = showLogName;
            this.dateTimeFormat = dateTimeFormat;

            if (this.dateTimeFormat != null && this.dateTimeFormat.Length > 0)
            {
                hasDateTimeFormat = true;
            }
        }

        /// <summary>
        /// Responsible for assembling and writing the log message to the tracing sub-system.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
        protected override void Write(LogLevel level, object message, Exception e)
        {
            // Use a StringBuilder for better performance
            StringBuilder sb = new StringBuilder();

            // Append date-time if so configured
            if (showDateTime)
            {
                if (hasDateTimeFormat)
                {
                    sb.Append(DateTime.Now.ToString(dateTimeFormat, CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(DateTime.Now);
                }

                sb.Append(" ");
            }

            // Append a readable representation of the log level
            sb.Append(string.Format("[{0}]", level.ToString().ToUpper()).PadRight(8));

            // Append the name of the log instance if so configured
            if (showLogName)
            {
                sb.Append(logName).Append(" - ");
            }

            // Append the message
            sb.Append(message.ToString());

            // Append stack trace if not null
            if (e != null)
            {
                sb.Append(Environment.NewLine).Append(e.ToString());
            }

            string _logFilePath = GetLogFilePath();

            // 写入日志，如果文件不存在则创建
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.WriteLine(sb.ToString());
            }

        }

        private string GetLogFilePath()
        {
            string logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            string logFileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt";
            return Path.Combine(logFolderPath, logFileName);
        }

        /// <summary>
        /// Is the given log level currently enabled ?
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected override bool IsLevelEnabled(LogLevel level)
        {
            int iLevel = (int)level;
            int iCurrentLogLevel = (int)currentLogLevel;

            return (iLevel >= iCurrentLogLevel);
        }
    }
}
