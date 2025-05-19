using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class CallContext
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly AsyncLocal<ConcurrentDictionary<string, object>> context = new AsyncLocal<ConcurrentDictionary<string, object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(string key, object value)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug($"缓存ID为{key}的SqlMapSession对象到CallContext对象中！");
            }
            ConcurrentDictionary<string, object> _dict = context.Value ??= new ConcurrentDictionary<string, object>();
            _dict[key] = value;
        }

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="ConcurrentDictionary<string, object>"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object? GetData(string key)
        {
            ConcurrentDictionary<string, object>? _dict = context.Value;
            object? _result = (_dict != null && _dict.TryGetValue(key, out var value)) ? value : null;
            if (logger.IsDebugEnabled)
            {
                logger.Debug($"从CallContext中获取ID为{key}的SqlMapSession对象！{(_result == null ? "不存在！" : "获取成功！")}");
            }
            return _result;
        }

    }
}
