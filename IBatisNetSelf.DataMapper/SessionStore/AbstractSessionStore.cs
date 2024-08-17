using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractSessionStore : MarshalByRefObject, ISessionStore
    {
        const string KEY = "_IBATIS_LOCAL_SQLMAP_SESSION_";
        /// <summary>
        /// session name
        /// </summary>	    
        protected string sessionName = string.Empty;


        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractSessionStore"/> class.
        /// </summary>
        /// <param name="sqlMapperId">The SQL mapper id.</param>
        public AbstractSessionStore(string sqlMapperId)
        {
            this.sessionName = KEY + sqlMapperId;
        }

        /// <summary>
        /// Get the local session
        /// </summary>
        public abstract ISqlMapSession LocalSession
        {
            get;
        }


        /// <summary>
        /// Store the specified session.
        /// </summary>
        /// <param name="session">The session to store</param>
        public abstract void Store(ISqlMapSession session);

        /// <summary>
        /// Remove the local session from the storage.
        /// </summary>
        public abstract void Dispose();
    }
}
