using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for PreparedStatementFactory.
    /// </summary>
    public class PreparedStatementFactory
    {

        #region Fields

        private PreparedStatement preparedStatement = null;

        private string parameterPrefix = string.Empty;
        private IStatement statement = null;
        private ISqlMapSession session = null;
        private string commandText = string.Empty;
        private RequestScope request = null;
        // (property, DbParameter)
        private HybridDictionary propertyDbParameterMap = new HybridDictionary();

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="statement"></param>
        /// <param name="commandText"></param>
        /// <param name="request"></param>
        public PreparedStatementFactory(ISqlMapSession session, RequestScope request, IStatement statement, string commandText)
        {
            this.session = session;
            this.request = request;
            this.statement = statement;
            this.commandText = commandText;
        }


        /// <summary>
        /// Create a list of IDataParameter for the statement and build the sql string.
        /// </summary>
        public PreparedStatement Prepare()
        {
            this.preparedStatement = new PreparedStatement();
            this.parameterPrefix = session.DataSource.DbProvider.ParameterPrefix;

            this.preparedStatement.PreparedSql = this.commandText;

            if (this.statement.CommandType == CommandType.Text)
            {
                if (this.request.ParameterMap != null)
                {
                    //Create IDataParameters for command text statement.
                    //this.preparedStatement.DbParameters
                    this.CreateParametersForTextCommand();
                    //Parse sql command text.
                    //处理sql中的参数占位符
                    this.EvaluateParameterMap();
                }
            }
            else if (this.statement.CommandType == CommandType.StoredProcedure) // StoredProcedure
            {
                if (this.request.ParameterMap == null) // No parameterMap --> error
                {
                    throw new DataMapperException("A procedure statement tag must have a parameterMap attribute, which is not the case for the procedure '" + statement.Id + ".");
                }
                else //use the parameterMap
                {
                    if (this.session.DataSource.DbProvider.UseDeriveParameters)
                    {
                        this.DiscoverParameter(session);
                    }
                    else
                    {
                        //Create IDataParameters for procedure statement.
                        this.CreateParametersForProcedureCommand();
                    }
                }

                #region Fix for Odbc
                // Although executing a parameterized stored procedure using the ODBC .NET Provider 
                // is slightly different from executing the same procedure using the SQL or 
                // the OLE DB Provider, there is one important difference 
                // -- the stored procedure must be called using the ODBC CALL syntax rather than 
                // the name of the stored procedure. 
                // For additional information on this CALL syntax, 
                // see the page entitled "Procedure Calls" in the ODBC Programmer's Reference 
                // in the MSDN Library. 
                //http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q309486

                if (this.session.DataSource.DbProvider.IsObdc == true)
                {
                    StringBuilder commandTextBuilder = new StringBuilder("{ call ");
                    commandTextBuilder.Append(commandText);

                    if (preparedStatement.DbParameters.Length > 0)
                    {
                        commandTextBuilder.Append(" (");
                        int supIndex = preparedStatement.DbParameters.Length - 1;
                        for (int i = 0; i < supIndex; i++)
                        {
                            commandTextBuilder.Append("?,");
                        }
                        commandTextBuilder.Append("?) }");
                    }
                    preparedStatement.PreparedSql = commandTextBuilder.ToString();
                }

                #endregion
            }

            if (logger.IsDebugEnabled)
            {
                logger.Debug("Statement Id: [" + statement.Id + "] Prepared SQL: [" + preparedStatement.PreparedSql + "]");
            }

            return this.preparedStatement;
        }


        #region Private methods

        /// <summary>
        /// For store procedure, auto discover IDataParameters for stored procedures at run-time.
        /// </summary>
        /// <param name="aSession">The current session.</param>
        private void DiscoverParameter(ISqlMapSession aSession)
        {
            // pull the parameters for this stored procedure from the parameter cache 
            // (or discover them & populate the cache)
            IDataParameter[] _commandParameters = aSession.SqlMapper.DBHelperParameterCache.GetSpParameterSet(aSession, this.commandText);

            this.preparedStatement.DbParameters = new IDbDataParameter[_commandParameters.Length];

            int _start = aSession.DataSource.DbProvider.ParameterPrefix.Length;
            for (int i = 0; i < _commandParameters.Length; i++)
            {
                IDbDataParameter _dataParameter = (IDbDataParameter)_commandParameters[i];

                if (aSession.DataSource.DbProvider.UseParameterPrefixInParameter == false)
                {
                    if (_dataParameter.ParameterName.StartsWith(aSession.DataSource.DbProvider.ParameterPrefix))
                    {
                        _dataParameter.ParameterName = _dataParameter.ParameterName.Substring(_start);
                    }
                }
                this.preparedStatement.DbParametersName.Add(_dataParameter.ParameterName);
                this.preparedStatement.DbParameters[i] = _dataParameter;
            }

            // Re-sort DbParameters to match order used in the parameterMap
            IDbDataParameter[] _sortedDbParameters = new IDbDataParameter[_commandParameters.Length];
            for (int i = 0; i < this.statement.ParameterMap.Properties.Count; i++)
            {
                _sortedDbParameters[i] = Search(aSession, this.preparedStatement.DbParameters, this.statement.ParameterMap.Properties[i], i);
            }
            this.preparedStatement.DbParameters = _sortedDbParameters;
        }

        private IDbDataParameter Search(ISqlMapSession session, IDbDataParameter[] parameters, ParameterProperty property, int index)
        {
            if (property.ColumnName.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameterName = parameters[i].ParameterName;
                    if (session.DataSource.DbProvider.UseParameterPrefixInParameter)
                    {
                        if (parameterName.StartsWith(session.DataSource.DbProvider.ParameterPrefix))
                        {
                            int prefixLength = session.DataSource.DbProvider.ParameterPrefix.Length;
                            parameterName = parameterName.Substring(prefixLength);
                        }
                    }
                    if (property.ColumnName.Equals(parameterName))
                    {
                        return parameters[i];
                    }
                }
                throw new IndexOutOfRangeException("The parameter '" + property.ColumnName + "' does not exist in the stored procedure '" + statement.Id + "'. Check your parameterMap.");
            }
            else
            {
                return parameters[index];
            }

        }


        /// <summary>
        /// Create IDataParameters for command text statement.
        /// </summary>
        private void CreateParametersForTextCommand()
        {
            string _sqlParamName = string.Empty;
            string _dbTypePropertyName = this.session.DataSource.DbProvider.ParameterDbTypeProperty;
            Type _enumDbType = this.session.DataSource.DbProvider.ParameterDbType;
            ParameterPropertyCollection _list = null;

            if (session.DataSource.DbProvider.UsePositionalParameters) //obdc/oledb
            {
                _list = this.request.ParameterMap.Properties;
            }
            else
            {
                _list = this.request.ParameterMap.PropertiesList;
            }

            this.preparedStatement.DbParameters = new IDbDataParameter[_list.Count];

            for (int i = 0; i < _list.Count; i++)
            {
                ParameterProperty _property = _list[i];

                if (session.DataSource.DbProvider.UseParameterPrefixInParameter)
                {
                    // From Ryan Yao: JIRA-27, used "param" + i++ for sqlParamName
                    _sqlParamName = this.parameterPrefix + "param" + i;
                }
                else
                {
                    _sqlParamName = "param" + i;
                }

                IDbDataParameter _dataParameter = this.session.CreateDataParameter();

                // Manage dbType attribute if any
                if (_property.DbType != null && _property.DbType.Length > 0)
                {
                    // Exemple : Enum.parse(System.Data.SqlDbType, 'VarChar')
                    object dbType = Enum.Parse(_enumDbType, _property.DbType, true);

                    // Exemple : ObjectHelper.SetProperty(sqlparameter, 'SqlDbType', SqlDbType.Int);
                    ObjectProbe.SetMemberValue(_dataParameter, _dbTypePropertyName, dbType,
                        request.DataExchangeFactory.ObjectFactory,
                        request.DataExchangeFactory.AccessorFactory);
                }

                // Set IDbDataParameter
                // JIRA-49 Fixes (size, precision, and scale)
                if (session.DataSource.DbProvider.SetDbParameterSize)
                {
                    if (_property.Size != -1)
                    {
                        _dataParameter.Size = _property.Size;
                    }
                }

                if (session.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    _dataParameter.Precision = _property.Precision;
                }

                if (session.DataSource.DbProvider.SetDbParameterScale)
                {
                    _dataParameter.Scale = _property.Scale;
                }

                // Set as direction parameter
                _dataParameter.Direction = _property.Direction;

                _dataParameter.ParameterName = _sqlParamName;

                this.preparedStatement.DbParametersName.Add(_property.PropertyName);
                this.preparedStatement.DbParameters[i] = _dataParameter;

                if (session.DataSource.DbProvider.UsePositionalParameters == false)
                {
                    this.propertyDbParameterMap.Add(_property, _dataParameter);
                }
            }
        }


        /// <summary>
        /// Create IDataParameters for procedure statement.
        /// </summary>
        private void CreateParametersForProcedureCommand()
        {
            string _procedureParamName = string.Empty;
            string _dbTypePropertyName = this.session.DataSource.DbProvider.ParameterDbTypeProperty;
            Type _enumDbType = this.session.DataSource.DbProvider.ParameterDbType;
            ParameterPropertyCollection _list = null;

            if (session.DataSource.DbProvider.UsePositionalParameters) //obdc/oledb
            {
                _list = this.request.ParameterMap.Properties;
            }
            else
            {
                _list = this.request.ParameterMap.PropertiesList;
            }

            this.preparedStatement.DbParameters = new IDbDataParameter[_list.Count];

            // ParemeterMap are required for procedure and we tested existance in Prepare() method
            // so we don't have to test existence here.
            // A ParameterMap used in CreateParametersForProcedureText must
            // have property and column attributes set.
            // The column attribute is the name of a procedure parameter.
            for (int i = 0; i < _list.Count; i++)
            {
                ParameterProperty _property = _list[i];

                if (this.session.DataSource.DbProvider.UseParameterPrefixInParameter)
                {
                    _procedureParamName = this.parameterPrefix + _property.ColumnName;
                }
                else //obdc/oledb
                {
                    _procedureParamName = _property.ColumnName;
                }

                //和查询创建参数不一样（CreateParametersForTextCommand）？不明白具体区别
                IDbDataParameter _dataParameter = this.session.CreateCommand(this.statement.CommandType).CreateParameter();

                // Manage dbType attribute if any
                if (_property.DbType != null && _property.DbType.Length > 0)
                {
                    // Exemple : Enum.parse(System.Data.SqlDbType, 'VarChar')
                    object _dbType = Enum.Parse(_enumDbType, _property.DbType, true);

                    // Exemple : ObjectHelper.SetProperty(sqlparameter, 'SqlDbType', SqlDbType.Int);
                    //设置_dataParameter的属性_dbTypePropertyName的值为_dbType
                    ObjectProbe.SetMemberValue(_dataParameter, _dbTypePropertyName, _dbType,
                        request.DataExchangeFactory.ObjectFactory,
                        request.DataExchangeFactory.AccessorFactory);
                }

                // Set IDbDataParameter
                // JIRA-49 Fixes (size, precision, and scale)
                if (this.session.DataSource.DbProvider.SetDbParameterSize)
                {
                    if (_property.Size != -1)
                    {
                        _dataParameter.Size = _property.Size;
                    }
                }

                if (this.session.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    _dataParameter.Precision = _property.Precision;
                }

                if (this.session.DataSource.DbProvider.SetDbParameterScale)
                {
                    _dataParameter.Scale = _property.Scale;
                }

                // Set as direction parameter
                _dataParameter.Direction = _property.Direction;

                _dataParameter.ParameterName = _procedureParamName;

                this.preparedStatement.DbParametersName.Add(_property.PropertyName);
                this.preparedStatement.DbParameters[i] = _dataParameter;

                if (this.session.DataSource.DbProvider.UsePositionalParameters == false)
                {
                    this.propertyDbParameterMap.Add(_property, _dataParameter);
                }
            }

        }


        /// <summary>
        /// Parse sql command text.
        /// </summary>
        private void EvaluateParameterMap()
        {
            string _delimiter = "?";
            string _token = null;
            int _index = 0;
            //sql中有效参数占位符定义
            string _sqlParamName = string.Empty;
            StringTokenizer _parser = new StringTokenizer(commandText, _delimiter, true);
            StringBuilder _newCommandTextBuffer = new StringBuilder();

            IEnumerator _enumerator = _parser.GetEnumerator();

            while (_enumerator.MoveNext())
            {
                _token = (string)_enumerator.Current;

                if (_delimiter.Equals(_token)) // ?
                {
                    ParameterProperty _property = request.ParameterMap.Properties[_index];
                    IDataParameter _dataParameter = null;

                    if (session.DataSource.DbProvider.UsePositionalParameters)
                    {
                        // TODO Refactor?
                        if (this.parameterPrefix.Equals(":"))
                        {
                            // ODP.NET uses positional parameters by default
                            // but uses ":0" or ":1" instead of "?"
                            _sqlParamName = ":" + _index;
                        }
                        else
                        {
                            // OLEDB/OBDC doesn't support named parameters !!!
                            _sqlParamName = "?";
                        }
                    }
                    else
                    {
                        _dataParameter = (IDataParameter)this.propertyDbParameterMap[_property];

                        // 5 May 2004
                        // Need to check UseParameterPrefixInParameter here 
                        // since CreateParametersForStatementText now does
                        // a check for UseParameterPrefixInParameter before 
                        // creating the parameter name!
                        if (session.DataSource.DbProvider.UseParameterPrefixInParameter)
                        {
                            // Fix ByteFX.Data.MySqlClient.MySqlParameter
                            // who strip prefix in Parameter Name ?!
                            if (session.DataSource.DbProvider.Name.IndexOf("ByteFx") >= 0)
                            {
                                _sqlParamName = this.parameterPrefix + _dataParameter.ParameterName;
                            }
                            else
                            {
                                _sqlParamName = _dataParameter.ParameterName;
                            }
                        }
                        else
                        {
                            _sqlParamName = this.parameterPrefix + _dataParameter.ParameterName;
                        }
                    }

                    _newCommandTextBuffer.Append(" ");
                    _newCommandTextBuffer.Append(_sqlParamName);

                    _sqlParamName = string.Empty;
                    _index++;
                }
                else
                {
                    _newCommandTextBuffer.Append(_token);
                }
            }

            this.preparedStatement.PreparedSql = _newCommandTextBuffer.ToString();
        }


        #endregion
    }
}
