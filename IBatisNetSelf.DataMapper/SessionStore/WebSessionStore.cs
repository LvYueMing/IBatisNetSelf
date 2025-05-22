using IBatisNetSelf.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// 提供了 <see cref="ISessionStore"/> 接口的实现，
    /// 基于 <c>HttpContext</c>，适用于 Web 项目。
    /// 此实现会从当前 HTTP 请求中获取会话对象。
    /// </summary>
    public class WebSessionStore : AbstractSessionStore
    {
        // HttpContext 访问器，用于获取当前请求上下文
        private readonly static HttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        /// <summary>
        /// 构造函数，初始化 WebSessionStore 实例
        /// </summary>
        /// <param name="sqlMapperId">SQL 映射器的 ID，用于标识当前 SqlMapSession 实例</param>
        public WebSessionStore(string sqlMapperId) : base(sqlMapperId)
        {
        }

        /// <summary>
        /// 获取当前请求上下文中的 SqlMap 会话对象（LocalSession）
        /// </summary>
        public override ISqlMapSession? LocalSession
        {
            get
            {
                // 获取当前 HTTP 请求上下文
                HttpContext _currentContext = ObtainSessionContext();
                // 从 HttpContext.Items 中读取当前请求的会话对象
                return _currentContext.Items[sessionName] as SqlMapSession;
            }
        }

        /// <summary>
        /// 存储指定的 SqlMap 会话对象到当前 HTTP 请求上下文中
        /// </summary>
        /// <param name="session">要保存的会话对象</param>
        public override void Store(ISqlMapSession session)
        {
            // 获取当前请求上下文
            HttpContext _currentContext = ObtainSessionContext();
            // 将会话对象存储到 HttpContext.Items 中
            _currentContext.Items[sessionName] = session;
        }

        /// <summary>
        /// 清除当前请求上下文中的 SqlMap 会话对象
        /// </summary>
        public override void Dispose()
        {
            // 获取当前请求上下文
            HttpContext _currentContext = ObtainSessionContext();
            // 从 Items 中移除会话对象
            _currentContext.Items.Remove(sessionName);
        }

        /// <summary>
        /// 获取当前请求的 HttpContext 上下文
        /// </summary>
        /// <returns>当前的 HttpContext</returns>
        private static HttpContext ObtainSessionContext()
        {
            // 获取当前请求上下文
            HttpContext _currentContext = httpContextAccessor.HttpContext;

            // 如果 HttpContext 为 null，抛出异常
            if (_currentContext == null)
            {
                throw new IBatisNetSelfException("WebSessionStore: Could not obtain reference to HttpContext");
            }
            return _currentContext;
        }
    }
}
