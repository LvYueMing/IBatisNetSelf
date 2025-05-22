using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// 提供一个基于 <c>CallContext</c> 的 <see cref="ISessionStore"/> 实现。
    /// 此实现会从当前线程获取会话对象。⚠ 不适用于 Web 场景（如 Web 应用程序或 Web 服务）。
    /// </summary>
    public class CallContextSessionStore : AbstractSessionStore
    {
        /// <summary>
        /// 使用指定的 SQL 映射器 ID 初始化 <see cref="CallContextSessionStore"/> 的新实例。
        /// </summary>
        /// <param name="sqlMapperId">SQL 映射器的唯一标识符（用于生成唯一的会话键）。</param>
        public CallContextSessionStore(string sqlMapperId) : base(sqlMapperId)
        { }

        /// <summary>
        /// 获取当前线程的本地会话。
        /// </summary>
        public override ISqlMapSession LocalSession => CallContext.GetData(this.sessionName) as SqlMapSession;


        /// <summary>
        /// 存储指定的会话对象到当前线程上下文。
        /// </summary>
        /// <param name="session">要存储的 SQL 会话对象。</param>
        public override void Store(ISqlMapSession session)
        {
            CallContext.SetData(this.sessionName, session);
        }

        /// <summary>
        /// 移除当前线程的本地会话。
        /// </summary>
        public override void Dispose()
        {
            CallContext.SetData(this.sessionName, null);
        }
    }
}
