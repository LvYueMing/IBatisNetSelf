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
    /// Summary description for StaticSql.
    /// </summary>
    public sealed class StaticSql : ISql
    {

        #region Fields

        private IStatement statement = null;
        private PreparedStatement preparedStatement = null;
        private DataExchangeFactory dataExchangeFactory = null;

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="scope"></param>
        public StaticSql(IScope scope, IStatement statement)
        {
            this.statement = statement;

            this.dataExchangeFactory = scope.DataExchangeFactory;
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

            _request.PreparedStatement = this.preparedStatement;
            _request.MappedStatement = aMappedStatement;

            return _request;
        }

        /// <summary>
        /// Build the PreparedStatement
        /// </summary>
        /// <param name="aSession"></param>
        /// <param name="aSqlStatement"></param>
        public void BuildPreparedStatement(ISqlMapSession aSession, string aSqlStatement)
        {
            RequestScope _request = new RequestScope(this.dataExchangeFactory, aSession, this.statement);

            PreparedStatementFactory _factory = new PreparedStatementFactory(aSession, _request, this.statement, aSqlStatement);
            this.preparedStatement = _factory.Prepare();
        }

        #endregion
    }
}
