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
    /// Build a session container for a Windows or Web context.
    /// When running in the context of a web application the session object is 
    /// stored in HttpContext items and has 'per request' lifetime.
    /// When running in the context of a windows application the session object is stored via CallContext.
    /// 为Windows或Web上下文构建会话容器。
    /// 当在web应用程序的上下文中运行时，会话对象是存储在HttpContext项中，并具有“每个请求”的生存期。
    /// 在windows应用程序的上下文中运行时，会话对象是通过CallContext存储。
    /// </summary>
    public sealed class SessionStoreFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly static HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        /// <summary>
        /// Gets the session store.
        /// </summary>
        /// <param name="sqlMapperId">The SQL mapper id.</param>
        /// <returns></returns>
        public static ISessionStore GetSessionStore(string sqlMapperId)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug($"New CallContextSessionStore");
                }
                return new CallContextSessionStore(sqlMapperId);
            }
            else
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug($"New WebSessionStore");
                }
                return new WebSessionStore(sqlMapperId);
            }
            return null;
        }

    }
}
