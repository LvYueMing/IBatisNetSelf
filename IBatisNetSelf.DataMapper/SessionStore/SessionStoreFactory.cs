using IBatisNetSelf.Common.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// 为 Windows 或 Web 上下文构建会话容器。
    /// 当在 web 应用程序上下文中运行时，会话对象存储在 HttpContext 的 Items 中，生命周期为“每个请求”。
    /// 当在 Windows 应用程序上下文中运行时，会话对象通过 CallContext 存储。
    /// </summary>
    public sealed class SessionStoreFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // 用于访问当前 HttpContext 的静态对象（ASP.NET Core 中用于替代传统 HttpContext.Current 的方式）
        private readonly static HttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 获取当前上下文对应的会话存储实现。
        /// </summary>
        /// <param name="sqlMapperId">SQL 映射器的标识符。</param>
        /// <returns>基于当前上下文的 ISessionStore 实例。</returns>
        public static ISessionStore GetSessionStore(string sqlMapperId)
        {
            // 如果当前 HttpContext 为 null，说明不是在 Web 请求上下文中（可能是桌面程序或后台任务）
            if (httpContextAccessor.HttpContext == null)
            {
                // 如果启用了调试日志，记录创建 CallContextSessionStore 的信息
                if (logger.IsDebugEnabled)
                {
                    logger.Debug($"New CallContextSessionStore");
                }
                // 返回适用于非 Web 环境的会话存储对象
                return new CallContextSessionStore(sqlMapperId);
            }
            else
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug($"New WebSessionStore");
                }
                // 返回适用于 Web 环境的会话存储对象
                return new WebSessionStore(sqlMapperId);
            }
            // 理论上不会执行到这里，但为了语法完整性保留
            return null;
        }

    }
}
