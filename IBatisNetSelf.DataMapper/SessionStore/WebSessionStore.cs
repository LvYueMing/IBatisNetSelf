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
    /// Provides an implementation of <see cref="ISessionStore"/>
    /// which relies on <c>HttpContext</c>. Suitable for web projects.
    /// This implementation will get the current session from the current request.
    /// </summary>
    public class WebSessionStore : AbstractSessionStore
    {
        private readonly static HttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSessionStore"/> class.
        /// </summary>
        /// <param name="sqlMapperId">The SQL mapper id.</param>
        public WebSessionStore(string sqlMapperId) : base(sqlMapperId)
        {
        }

        /// <summary>
        /// Get the local session
        /// </summary>
        public override ISqlMapSession? LocalSession
        {
            get
            {
                HttpContext _currentContext = ObtainSessionContext();
                return _currentContext.Items[sessionName] as SqlMapSession;
            }
        }

        /// <summary>
        /// Store the specified session.
        /// </summary>
        /// <param name="session">The session to store</param>
        public override void Store(ISqlMapSession session)
        {
            HttpContext _currentContext = ObtainSessionContext();
            _currentContext.Items[sessionName] = session;
        }

        /// <summary>
        /// Remove the local session.
        /// </summary>
        public override void Dispose()
        {
            HttpContext _currentContext = ObtainSessionContext();
            _currentContext.Items.Remove(sessionName);
        }


        private static HttpContext ObtainSessionContext()
        {
            HttpContext _currentContext = httpContextAccessor.HttpContext;

            if (_currentContext == null)
            {
                throw new IBatisNetSelfException("WebSessionStore: Could not obtain reference to HttpContext");
            }
            return _currentContext;
        }
    }
}
