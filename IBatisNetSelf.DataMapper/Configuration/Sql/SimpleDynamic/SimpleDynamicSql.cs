using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.SimpleDynamic
{
    /// <summary>
    /// Summary description for SimpleDynamicSql.
    /// </summary>
    internal sealed class SimpleDynamicSql : ISql
    {

        #region private

        private const string ELEMENT_TOKEN = "$";

        private string simpleDynamicSql = string.Empty;
        private IStatement statement = null;
        private DataExchangeFactory dataExchangeFactory = null;

        #endregion

        #region Constructor (s) / Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDynamicSql"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="statement">The statement.</param>
        internal SimpleDynamicSql(IScope scope, string sqlStatement, IStatement statement)
        {
            this.simpleDynamicSql = sqlStatement;
            this.statement = statement;

            this.dataExchangeFactory = scope.DataExchangeFactory;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public string GetSql(object parameterObject)
        {
            return ProcessDynamicElements(parameterObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public static bool IsSimpleDynamicSql(string sqlStatement)
        {
            return ((sqlStatement != null) && (sqlStatement.IndexOf(ELEMENT_TOKEN) > -1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        private string ProcessDynamicElements(object aParameterObject)
        {
            // define which character is seperating fields

            StringTokenizer _parser = new StringTokenizer(this.simpleDynamicSql, ELEMENT_TOKEN, true);

            StringBuilder _newSql = new StringBuilder();

            string _token = null;
            string _lastToken = null;

            IEnumerator _enumerator = _parser.GetEnumerator();

            while (_enumerator.MoveNext())
            {
                _token = ((string)_enumerator.Current);

                if (ELEMENT_TOKEN.Equals(_lastToken))
                {
                    if (ELEMENT_TOKEN.Equals(_token))
                    {
                        _newSql.Append(ELEMENT_TOKEN);
                        _token = null;
                    }
                    else
                    {
                        object _value = null;
                        if (aParameterObject != null)
                        {
                            if (dataExchangeFactory.TypeHandlerFactory.IsSimpleType(aParameterObject.GetType()) == true)
                            {
                                _value = aParameterObject;
                            }
                            else
                            {
                                _value = ObjectProbe.GetMemberValue(aParameterObject, _token, dataExchangeFactory.AccessorFactory);
                            }
                        }
                        if (_value != null)
                        {
                            _newSql.Append(_value.ToString());
                        }

                        _enumerator.MoveNext();
                        _token = ((string)_enumerator.Current);

                        if (!ELEMENT_TOKEN.Equals(_token))
                        {
                            throw new DataMapperException("Unterminated dynamic element in sql (" + simpleDynamicSql + ").");
                        }
                        _token = null;
                    }
                }
                else
                {
                    if (!ELEMENT_TOKEN.Equals(_token))
                    {
                        _newSql.Append(_token);
                    }
                }

                _lastToken = _token;
            }

            return _newSql.ToString();
        }


        #region ISql Members

        /// <summary>
        /// Builds a new <see cref="RequestScope"/> and the sql command text to execute.
        /// </summary>
        /// <param name="parameterObject">The parameter object (used in DynamicSql)</param>
        /// <param name="session">The current session</param>
        /// <param name="mappedStatement">The <see cref="IMappedStatement"/>.</param>
        /// <returns>A new <see cref="RequestScope"/>.</returns>
        public RequestScope GetRequestScope(IMappedStatement mappedStatement,
            object parameterObject, ISqlMapSession session)
        {
            string _sqlStatement = ProcessDynamicElements(parameterObject);

            RequestScope _request = new RequestScope(dataExchangeFactory, session, statement);

            _request.PreparedStatement = BuildPreparedStatement(session, _request, _sqlStatement);
            _request.MappedStatement = mappedStatement;

            return _request;
        }

        /// <summary>
        /// Build the PreparedStatement
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        /// <param name="sqlStatement"></param>
        private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
        {
            PreparedStatementFactory _factory = new PreparedStatementFactory(session, request, this.statement, sqlStatement);
            return _factory.Prepare();
        }

        #endregion

        #endregion

    }
}
