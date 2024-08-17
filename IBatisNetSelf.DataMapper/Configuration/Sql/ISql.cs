using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql
{
    /// <summary>
    /// Summary description for ISql.
    /// </summary>
    public interface ISql
    {

        #region Methods
        /// <summary>
        /// Builds a new <see cref="RequestScope"/> and the <see cref="IDbCommand"/> text to execute.
        /// </summary>
        /// <param name="parameterObject">
        /// The parameter object (used by DynamicSql/SimpleDynamicSql).
        /// Use to complete the sql statement.
        /// </param>
        /// <param name="session">The current session</param>
        /// <param name="mappedStatement">The <see cref="IMappedStatement"/>.</param>
        /// <returns>A new <see cref="RequestScope"/>.</returns>
        RequestScope GetRequestScope(IMappedStatement mappedStatement, object parameterObject, ISqlMapSession session);
        #endregion

    }
}
