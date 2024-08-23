using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Static
{
    /// <summary>
    /// Summary description for ProcedureSql.
    /// </summary>
    public sealed class ProcedureSql : ISql
    {
        #region Fields

        private IStatement statement = null;
        private PreparedStatement preparedStatement = null;
        private string sqlStatement = string.Empty;
        private object synRoot = new Object();
        private DataExchangeFactory dataExchangeFactory = null;

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aStatement">The statement.</param>
        /// <param name="aSqlStatement"></param>
        /// <param name="aConfigScope"></param>
        public ProcedureSql(IScope aConfigScope, string aSqlStatement, IStatement aStatement)
        {
            this.sqlStatement = aSqlStatement;
            this.statement = aStatement;

            this.dataExchangeFactory = aConfigScope.DataExchangeFactory;
        }
        #endregion

        #region ISql Members

        /// <summary>
        /// Builds a new <see cref="RequestScope"/> and the sql command text to execute.
        /// </summary>
        /// <param name="aParameterObject">The parameter object (used in DynamicSql)</param>
        /// <param name="aSession">The current session</param>
        /// <param name="aMappedStatement">The <see cref="IMappedStatement"/>.</param>
        /// <returns>A new <see cref="RequestScope"/>.</returns>
        public RequestScope GetRequestScope(IMappedStatement aMappedStatement,
            object aParameterObject, ISqlMapSession aSession)
        {
            RequestScope _request = new RequestScope(this.dataExchangeFactory, aSession, this.statement);

            _request.PreparedStatement = BuildPreparedStatement(aSession, _request, this.sqlStatement);
            _request.MappedStatement = aMappedStatement;

            return _request;
        }

        /// <summary>
        /// Build the PreparedStatement
        /// </summary>
        /// <param name="aSession"></param>
        /// <param name="aCommandText"></param>
        /// <param name="aRequest"></param>
        public PreparedStatement BuildPreparedStatement(ISqlMapSession aSession, RequestScope aRequest, string aCommandText)
        {
            if (this.preparedStatement == null)
            {
                lock (this.synRoot)
                {
                    if (this.preparedStatement == null)
                    {
                        PreparedStatementFactory _factory = new PreparedStatementFactory(aSession, aRequest, this.statement, aCommandText);
                        this.preparedStatement = _factory.Prepare();
                    }
                }
            }
            return this.preparedStatement;
        }

        #endregion
    }
}
