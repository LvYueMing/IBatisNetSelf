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
    /// 提供一种在调用和异步上下文中传递上下文数据的方式。
    /// 用于在线程或异步调用中保存/获取数据（如 SqlMapSession）。
    /// </summary>
    public static class CallContext
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // 使用 AsyncLocal 保证在异步/线程环境中有独立的数据上下文
        // 每个异步流都有自己的 ConcurrentDictionary 用于存储键值对
        private static readonly AsyncLocal<ConcurrentDictionary<string, object>> context = new AsyncLocal<ConcurrentDictionary<string, object>>();

        /// <summary>
        /// 储存指定名称的数据对象，并关联到当前调用上下文中。
        /// </summary>
        /// <param name="key">用于标识数据项的名称。</param>
        /// <param name="value">要存储的数据对象。</param>
        public static void SetData(string key, object value)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug($"缓存ID为{key}的SqlMapSession对象到CallContext对象中！");
            }
            // 如果当前上下文没有字典，就新建一个
            ConcurrentDictionary<string, object> _dict = context.Value ??= new ConcurrentDictionary<string, object>();
            _dict[key] = value;
        }

        /// <summary>
        /// 从 <see cref="ConcurrentDictionary<string, object>"/> 中获取指定名称的数据对象。
        /// </summary>
        /// <param name="key">要获取的数据项的名称。</param>
        /// <returns>关联到指定名称的对象，如果不存在则返回 null。</returns>
        public static object? GetData(string key)
        {
            // 获取当前上下文中的字典对象（可能为 null）
            ConcurrentDictionary<string, object>? _dict = context.Value;
            // 如果存在且包含指定键，则获取值，否则返回 null
            object? _result = (_dict != null && _dict.TryGetValue(key, out var value)) ? value : null;
            if (logger.IsDebugEnabled)
            {
                logger.Debug($"从CallContext中获取ID为{key}的SqlMapSession对象！{(_result == null ? "不存在！" : "获取成功！")}");
            }
            return _result;
        }

    }
}
