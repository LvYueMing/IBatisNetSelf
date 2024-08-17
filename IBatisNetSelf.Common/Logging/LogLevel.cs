using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// The 7 logging levels used by Log are (in order): 
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 
        /// </summary>
        All = 0,
        /// <summary>
        /// 
        /// </summary>
        Debug = 1,
        /// <summary>
        /// 
        /// </summary>
        Info = 2,
        /// <summary>
        /// 
        /// </summary>
        Warn = 3,
        /// <summary>
        /// 
        /// </summary>
        Error = 4,
        /// <summary>
        ///
        /// </summary>
        Fatal = 5,
        /// <summary>
        /// Do not log anything.
        /// </summary>
        Off = 6,
    }
}
