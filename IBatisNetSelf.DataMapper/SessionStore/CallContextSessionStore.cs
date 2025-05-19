using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// Provides an implementation of <see cref="ISessionStore"/> which relies on <c>CallContext</c>.
    /// This implementation will first get the current session from the current 
    /// thread. Do NOT use on web scenario (web applications or web services).
    /// </summary>
    public class CallContextSessionStore : AbstractSessionStore
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="CallContextSessionStore"/> class.
        /// </summary>
        /// <param name="sqlMapperId">The SQL mapper id.</param>
        public CallContextSessionStore(string sqlMapperId) : base(sqlMapperId)
        { }

        /// <summary>
        /// Get the local session
        /// </summary>
        public override ISqlMapSession LocalSession => CallContext.GetData(this.sessionName) as SqlMapSession;


        /// <summary>
        /// Store the specified session.
        /// </summary>
        /// <param name="session">The session to store</param>
        public override void Store(ISqlMapSession session)
        {
            CallContext.SetData(this.sessionName, session);
        }

        /// <summary>
        /// Remove the local session.
        /// </summary>
        public override void Dispose()
        {
            CallContext.SetData(this.sessionName, null);
        }
    }
}
