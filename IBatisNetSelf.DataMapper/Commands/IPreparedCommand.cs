using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Commands
{
    /// <summary>
    /// Summary description for IPreparedCommand.
    /// </summary>
    public interface IPreparedCommand
    {
        /// <summary>
        /// Create an IDbCommand for the SqlMapSession and the current SQL Statement
        /// and fill IDbCommand IDataParameter's with the parameterObject.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session">The SqlMapSession</param>
        /// <param name="statement">The IStatement</param>
        /// <param name="parameterObject">
        /// The parameter object that will fill the sql parameter
        /// </param>
        /// <returns>An IDbCommand with all the IDataParameter filled.</returns>
        void Create(RequestScope request, ISqlMapSession session, IStatement statement, object parameterObject);
    }
}
