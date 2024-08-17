using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging.Impl
{
    /// <summary>
    /// Silently ignores all log messages.
    /// </summary>
    public sealed class NoOpLogger : ILog
    {
        #region Members of ILog

        /// <summary>
        /// Always returns <see langword="false" />.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Always returns <see langword="false" />.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return false; }

        }

        /// <summary>
        /// Always returns <see langword="false" />.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Always returns <see langword="false" />.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Always returns <see langword="false" />.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        public void Debug(object message)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Debug(object message, Exception e)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        public void Error(object message)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Error(object message, Exception e)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(object message)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Fatal(object message, Exception e)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        public void Info(object message)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Info(object message, Exception e)
        {
            // NOP - no operation
        }

        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        public void Warn(object message)
        {
            // NOP - no operation
        }


        /// <summary>
        /// Ignores message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        public void Warn(object message, Exception e)
        {
            // NOP - no operation
        }

        #endregion
    }
}
