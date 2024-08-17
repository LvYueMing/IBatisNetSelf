using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging.Impl
{
    /// <summary>
    /// Sends log messages to <see cref="Console.Out" />.
    /// </summary>
    public class ConsoleOutLogger : AbstractLogger
    {
        private bool showDateTime = false;
        private bool showLogName = false;
        private string logName = string.Empty;
        private LogLevel currentLogLevel = LogLevel.All;
        private string dateTimeFormat = string.Empty;
        private bool hasDateTimeFormat = false;

        /// <summary>
        /// Creates and initializes a logger that writes messages to <see cref="Console.Out" />.
        /// </summary>
        /// <param name="logName">The name, usually type name of the calling class, of the logger.</param>
        /// <param name="logLevel">The current logging threshold. Messages recieved that are beneath this threshold will not be logged.</param>
        /// <param name="showDateTime">Include the current time in the log message.</param>
        /// <param name="showLogName">Include the instance name in the log message.</param>
        /// <param name="dateTimeFormat">The date and time format to use in the log message.</param>
        public ConsoleOutLogger(string logName, LogLevel logLevel
                                 , bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            this.logName = logName;
            this.currentLogLevel = logLevel;
            this.showDateTime = showDateTime;
            this.showLogName = showLogName;
            this.dateTimeFormat = dateTimeFormat;

            if (this.dateTimeFormat != null && this.dateTimeFormat.Length > 0)
            {
                this.hasDateTimeFormat = true;
            }
        }

        /// <summary>
        /// Do the actual logging by constructing the log message using a <see cref="StringBuilder" /> then
        /// sending the output to <see cref="Console.Out" />.
        /// </summary>
        /// <param name="aLevel">The <see cref="LogLevel" /> of the message.</param>
        /// <param name="aMessage">The log message.</param>
        /// <param name="e">An optional <see cref="Exception" /> associated with the message.</param>
        protected override void Write(LogLevel aLevel, object aMessage, Exception ex)
        {
            // Use a StringBuilder for better performance
            StringBuilder _sb = new StringBuilder();
            // Append date-time if so configured
            if (this.showDateTime)
            {
                if (this.hasDateTimeFormat)
                {
                    _sb.Append(DateTime.Now.ToString(this.dateTimeFormat, CultureInfo.InvariantCulture));
                }
                else
                {
                    _sb.Append(DateTime.Now);
                }

                _sb.Append(" ");
            }
            // Append a readable representation of the log level
            _sb.Append(string.Format("[{0}]", aLevel.ToString().ToUpper()).PadRight(8));

            // Append the name of the log instance if so configured
            if (this.showLogName)
            {
                _sb.Append(this.logName).Append(" - ");
            }

            // Append the message
            _sb.Append(aMessage.ToString());

            // Append stack trace if not null
            if (ex != null)
            {
                _sb.Append(Environment.NewLine).Append(ex.ToString());
            }

            // Print to the appropriate destination
            Console.Out.WriteLine(_sb.ToString());
        }

        /// <summary>
        /// Determines if the given log level is currently enabled.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        protected override bool IsLevelEnabled(LogLevel level)
        {
            int iLevel = (int)level;
            int iCurrentLogLevel = (int)currentLogLevel;

            // return iLevel.CompareTo(iCurrentLogLevel); better ???
            return (iLevel >= iCurrentLogLevel);
        }
    }
}
