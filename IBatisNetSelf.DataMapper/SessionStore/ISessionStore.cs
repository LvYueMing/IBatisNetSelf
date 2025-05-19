using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.SessionStore
{
    /// <summary>
    /// Provides the contract for implementors who want to store session.
    /// </summary>
    public interface ISessionStore
    {
        /// <summary>
        /// Get the local session
        /// </summary>
        ISqlMapSession LocalSession { get; }

        /// <summary>
        /// Store the specified session.
        /// </summary>
        /// <param name="session">The session to store</param>
        void Store(ISqlMapSession session);

        /// <summary>
        /// Remove the local session from the storage.
        /// </summary>
        void Dispose();
    }
}
