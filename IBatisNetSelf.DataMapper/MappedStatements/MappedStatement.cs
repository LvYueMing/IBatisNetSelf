using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy;
using IBatisNetSelf.DataMapper.Commands;
using IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// Summary description for MappedStatement.
    /// </summary>
    public class MappedStatement : IMappedStatement
    {
        /// <summary>
        /// Event launch on exceute query
        /// </summary>
        public event ExecuteEventHandler Execute;

        #region Fields

        // Magic number used to set the the maximum number of rows returned to 'all'. 
        internal const int NO_MAXIMUM_RESULTS = -1;
        // Magic number used to set the the number of rows skipped to 'none'. 
        internal const int NO_SKIPPED_RESULTS = -1;

        private IStatement statement = null;
        private ISqlMapper sqlMap = null;
        private IPreparedCommand preparedCommand = null;
        private IResultStrategy resultStrategy = null;
        #endregion

        #region Properties


        /// <summary>
        /// The IPreparedCommand to use
        /// </summary>
        public IPreparedCommand PreparedCommand => this.preparedCommand;

        /// <summary>
        /// Name used to identify the MappedStatement amongst the others.
        /// This the name of the SQL statement by default.
        /// </summary>
        public string Id => this.Id;

        /// <summary>
        /// The SQL statment used by this MappedStatement
        /// </summary>
        public IStatement Statement => this.statement;

        /// <summary>
        /// The SqlMap used by this MappedStatement
        /// </summary>
        public ISqlMapper SqlMap => this.sqlMap;

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlMap">An SqlMap</param>
        /// <param name="statement">An SQL statement</param>
        internal MappedStatement(ISqlMapper sqlMap, IStatement statement)
        {
            this.sqlMap = sqlMap;
            this.statement = statement;
            //创建默认的DefaultPreparedCommand:IPreparedCommand
            this.preparedCommand = PreparedCommandFactory.GetPreparedCommand(false);
            this.resultStrategy = ResultStrategyFactory.Get(this.statement);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Retrieve the output parameter and map them on the result object.
        /// This routine is only use is you specified a ParameterMap and some output attribute
        /// or if you use a store procedure with output parameter...
        /// </summary>
        /// <param name="aRequest"></param>
        /// <param name="aSession">The current session.</param>
        /// <param name="aResult">The result object.</param>
        /// <param name="aCommand">The command sql.</param>
        private void RetrieveOutputParameters(RequestScope aRequest, ISqlMapSession aSession, IDbCommand aCommand, object aResult)
        {
            if (aRequest.ParameterMap != null)
            {
                int _count = aRequest.ParameterMap.PropertiesList.Count;
                for (int i = 0; i < _count; i++)
                {
                    ParameterProperty _mapping = aRequest.ParameterMap.GetProperty(i);
                    if (_mapping.Direction == ParameterDirection.Output ||
                        _mapping.Direction == ParameterDirection.InputOutput)
                    {
                        string _parameterName = string.Empty;
                        if (aSession.DataSource.DbProvider.UseParameterPrefixInParameter == false)
                        {
                            _parameterName = _mapping.ColumnName;
                        }
                        else
                        {
                            _parameterName = aSession.DataSource.DbProvider.ParameterPrefix +
                                _mapping.ColumnName;
                        }

                        if (_mapping.TypeHandler == null) // Find the TypeHandler
                        {
                            lock (_mapping)
                            {
                                if (_mapping.TypeHandler == null)
                                {
                                    Type _propertyType = ObjectProbe.GetMemberTypeForGetter(aResult, _mapping.PropertyName);

                                    _mapping.TypeHandler = aRequest.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(_propertyType);
                                }
                            }
                        }

                        // Fix IBATISNET-239
                        //"Normalize" System.DBNull parameters
                        IDataParameter _dataParameter = (IDataParameter)aCommand.Parameters[_parameterName];
                        object _dbValue = _dataParameter.Value;

                        object _value = null;

                        bool _isNull = (_dbValue == DBNull.Value);
                        if (_isNull)
                        {
                            if (_mapping.HasNullValue)
                            {
                                _value = _mapping.TypeHandler.ValueOf(_mapping.GetAccessor.MemberType, _mapping.NullValue);
                            }
                            else
                            {
                                _value = _mapping.TypeHandler.NullValue;
                            }
                        }
                        else
                        {
                            _value = _mapping.TypeHandler.GetDataBaseValue(_dataParameter.Value, aResult.GetType());
                        }

                        aRequest.IsRowDataFound = aRequest.IsRowDataFound || (_value != null);

                        aRequest.ParameterMap.SetOutputParameter(ref aResult, _mapping, _value);
                    }
                }
            }
        }


        #region ExecuteForObject

        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The object</returns>
        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            return ExecuteQueryForObject(session, parameterObject, null);
        }


        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object of the type of
        /// the resultObject passed in as a parameter.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>The object</returns>
        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            object _obj = null;
            RequestScope _request = statement.Sql.GetRequestScope(this, parameterObject, session);

            this.preparedCommand.Create(_request, session, this.Statement, parameterObject);

            _obj = RunQueryForObject(_request, session, parameterObject, resultObject);

            return _obj;
        }


        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object of the type of
        /// the resultObject passed in as a parameter.
        /// </summary>
        /// <param name="request">The request scope.</param>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>The object</returns>
        internal object RunQueryForObject(RequestScope request, ISqlMapSession session, object parameterObject, object resultObject)
        {
            object _result = resultObject;

            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader _reader = command.ExecuteReader();
                try
                {
                    while (_reader.Read())
                    {
                        object obj = this.resultStrategy.Process(request, ref _reader, resultObject);
                        if (obj != BaseStrategy.SKIP)
                        {
                            _result = obj;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _reader.Close();
                    _reader.Dispose();
                }

                ExecutePostSelect(request);

                #region remark
                // If you are using the OleDb data provider (as you are), you need to close the
                // DataReader before output parameters are visible.
                #endregion

                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            RaiseExecuteEvent();

            return _result;
        }

        #endregion


        #region ExecuteQueryForList

        /// <summary>
        /// Runs a query with a custom object that gets a chance 
        /// to deal with each row as it is processed.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate"></param>
        public virtual IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            if (rowDelegate == null)
            {
                throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
            }

            return RunQueryForList(request, session, parameterObject, null, rowDelegate);
        }

        /// <summary>
        /// Runs a query with a custom object that gets a chance 
        /// to deal with each row as it is processed.
        /// </summary>
        /// <param name="session">The session used to execute the statement</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL. </param>
        /// <param name="keyProperty">The property of the result object to be used as the key. </param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <param name="rowDelegate"></param>
        /// <returns>A hashtable of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public virtual IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            if (rowDelegate == null)
            {
                throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForMapWithRowDelegate.");
            }

            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            return RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }


        /// <summary>
        /// Executes the SQL and retuns all rows selected. This is exactly the same as
        /// calling ExecuteQueryForList(session, parameterObject, NO_SKIPPED_RESULTS, NO_MAXIMUM_RESULTS).
        /// </summary>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        public virtual IList ExecuteQueryForList(ISqlMapSession aSession, object aParameterObject)
        {
            //创建请求对象_request
            RequestScope _request = this.statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            //创建命令对象IDbCommand，_request.IDbCommand=aSession.CreateCommand(aStatement.CommandType)
            //命令对象执行语句CommandText赋值，_request.IDbCommand.CommandText = _request.PreparedStatement.PreparedSql
            //给命令对象创建参数列表（aRequest.IDbCommand.Parameters），并赋值（从aParameterObject获取参数对应的值）
            //一切准备就绪，可执行命令获取数据
            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            return RunQueryForList(_request, aSession, aParameterObject, null, null);
        }


        /// <summary>
        /// Executes the SQL and retuns a subset of the rows selected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        public virtual IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            return RunQueryForList(request, session, parameterObject, skipResults, maxResults);
        }

        /// <summary>
        /// Runs the query for list.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        internal IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList list = null;

            using (IDbCommand command = request.IDbCommand)
            {
                if (statement.ListClass == null)
                {
                    list = new ArrayList();
                }
                else
                {
                    list = statement.CreateInstanceOfListClass();
                }

                IDataReader reader = command.ExecuteReader();

                try
                {
                    // skip results
                    for (int i = 0; i < skipResults; i++)
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                    }

                    // Get Results
                    int resultsFetched = 0;
                    while ((maxResults == NO_MAXIMUM_RESULTS || resultsFetched < maxResults)
                        && reader.Read())
                    {
                        object obj = this.resultStrategy.Process(request, ref reader, null);
                        if (obj != BaseStrategy.SKIP)
                        {
                            list.Add(obj);
                        }
                        resultsFetched++;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }

                ExecutePostSelect(request);

                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            return list;
        }

        /// <summary>
        /// Executes the SQL and retuns a List of result objects.
        /// </summary>
        /// <param name="aRequest">The request scope.</param>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="aResultObject">A strongly typed collection of result objects.</param>
        /// <param name="aRowDelegate"></param>
        /// <returns>A List of result objects.</returns>
        internal IList RunQueryForList(RequestScope aRequest, ISqlMapSession aSession, object aParameterObject, IList aResultObject, RowDelegate aRowDelegate)
        {
            IList _list = aResultObject;

            using (IDbCommand _command = aRequest.IDbCommand)
            {
                if (aResultObject == null)
                {
                    if (this.statement.ListClass == null)
                    {
                        _list = new ArrayList();
                    }
                    else
                    {
                        _list = this.statement.CreateInstanceOfListClass();
                    }
                }

                IDataReader _reader = _command.ExecuteReader();

                try
                {
                    do
                    {
                        if (aRowDelegate == null)
                        {
                            while (_reader.Read())
                            {
                                object obj = this.resultStrategy.Process(aRequest, ref _reader, null);
                                if (obj != BaseStrategy.SKIP)
                                {
                                    _list.Add(obj);
                                }
                            }
                        }
                        else
                        {
                            while (_reader.Read())
                            {
                                object obj = this.resultStrategy.Process(aRequest, ref _reader, null);
                                aRowDelegate(obj, aParameterObject, _list);
                            }
                        }
                    }
                    while (_reader.NextResult());
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _reader.Close();
                    _reader.Dispose();
                }

                this.ExecutePostSelect(aRequest);
                this.RetrieveOutputParameters(aRequest, aSession, _command, aParameterObject);
            }

            return _list;
        }


        /// <summary>
        /// Executes the SQL and and fill a strongly typed collection.
        /// </summary>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="aResultObject">A strongly typed collection of result objects.</param>
        public virtual void ExecuteQueryForList(ISqlMapSession aSession, object aParameterObject, IList aResultObject)
        {
            RequestScope _request = statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            RunQueryForList(_request, aSession, aParameterObject, aResultObject, null);
        }


        #endregion


        #region ExecuteUpdate, ExecuteInsert

        /// <summary>
        /// Execute an update statement. Also used for delete statement.
        /// Return the number of row effected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The number of row effected.</returns>
        public virtual int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            int rows = 0; // the number of rows affected
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            preparedCommand.Create(request, session, this.Statement, parameterObject);

            using (IDbCommand command = request.IDbCommand)
            {
                rows = command.ExecuteNonQuery();

                //ExecutePostSelect(request);

                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            RaiseExecuteEvent();

            return rows;
        }


        /// <summary>
        /// Execute an insert statement. Fill the parameter object with 
        /// the ouput parameters if any, also could return the insert generated key
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="parameterObject">The parameter object used to fill the statement.</param>
        /// <returns>Can return the insert generated key.</returns>
        public virtual object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            object generatedKey = null;
            SelectKey selectKeyStatement = null;
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            if (statement is Insert)
            {
                selectKeyStatement = ((Insert)statement).SelectKey;
            }

            if (selectKeyStatement != null && !selectKeyStatement.isAfter)
            {
                IMappedStatement mappedStatement = sqlMap.GetMappedStatement(selectKeyStatement.Id);
                generatedKey = mappedStatement.ExecuteQueryForObject(session, parameterObject);

                ObjectProbe.SetMemberValue(parameterObject, selectKeyStatement.PropertyName, generatedKey,
                    request.DataExchangeFactory.ObjectFactory,
                    request.DataExchangeFactory.AccessorFactory);
            }

            preparedCommand.Create(request, session, this.Statement, parameterObject);
            using (IDbCommand command = request.IDbCommand)
            {
                if (statement is Insert)
                {
                    command.ExecuteNonQuery();
                }
                // Retrieve output parameter if the result class is specified
                else if (statement is Procedure && (statement.ResultClass != null) &&
                        sqlMap.TypeHandlerFactory.IsSimpleType(statement.ResultClass))
                {
                    IDataParameter returnValueParameter = command.CreateParameter();
                    returnValueParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValueParameter);

                    command.ExecuteNonQuery();
                    generatedKey = returnValueParameter.Value;

                    ITypeHandler typeHandler = sqlMap.TypeHandlerFactory.GetTypeHandler(statement.ResultClass);
                    generatedKey = typeHandler.GetDataBaseValue(generatedKey, statement.ResultClass);
                }
                else
                {
                    generatedKey = command.ExecuteScalar();
                    if ((statement.ResultClass != null) &&
                        sqlMap.TypeHandlerFactory.IsSimpleType(statement.ResultClass))
                    {
                        ITypeHandler typeHandler = sqlMap.TypeHandlerFactory.GetTypeHandler(statement.ResultClass);
                        generatedKey = typeHandler.GetDataBaseValue(generatedKey, statement.ResultClass);
                    }
                }

                if (selectKeyStatement != null && selectKeyStatement.isAfter)
                {
                    IMappedStatement mappedStatement = sqlMap.GetMappedStatement(selectKeyStatement.Id);
                    generatedKey = mappedStatement.ExecuteQueryForObject(session, parameterObject);

                    ObjectProbe.SetMemberValue(parameterObject, selectKeyStatement.PropertyName, generatedKey,
                        request.DataExchangeFactory.ObjectFactory,
                        request.DataExchangeFactory.AccessorFactory);
                }

                //ExecutePostSelect(request);

                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            RaiseExecuteEvent();

            return generatedKey;
        }

        #endregion


        #region ExecuteQueryForMap

        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the value of the property specified
        /// in the valueProperty parameter.  If valueProperty is null, the entire result object will be entered.
        /// </summary>
        /// <param name="session">The session used to execute the statement</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL. </param>
        /// <param name="keyProperty">The property of the result object to be used as the key. </param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>A hashtable of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public virtual IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            RequestScope _request = statement.Sql.GetRequestScope(this, parameterObject, session);

            this.preparedCommand.Create(_request, session, this.Statement, parameterObject);

            return RunQueryForMap(_request, session, parameterObject, keyProperty, valueProperty, null);
        }


        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the value of the property specified
        /// in the valueProperty parameter.  If valueProperty is null, the entire result object will be entered.
        /// </summary>
        /// <param name="request">The request scope.</param>
        /// <param name="session">The session used to execute the statement</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <param name="rowDelegate">A delegate called once per row in the QueryForMapWithRowDelegate method</param>
        /// <returns>A hashtable of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        internal IDictionary RunQueryForMap(RequestScope request,
            ISqlMapSession session,
            object parameterObject,
            string keyProperty,
            string valueProperty,
            DictionaryRowDelegate rowDelegate)
        {
            IDictionary map = new Hashtable();

            using (IDbCommand command = request.IDbCommand)
            {
                IDataReader reader = command.ExecuteReader();
                try
                {

                    if (rowDelegate == null)
                    {
                        while (reader.Read())
                        {
                            object obj = this.resultStrategy.Process(request, ref reader, null);
                            object key = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
                            object value = obj;
                            if (valueProperty != null)
                            {
                                value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
                            }
                            map.Add(key, value);
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            object obj = this.resultStrategy.Process(request, ref reader, null);
                            object key = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
                            object value = obj;
                            if (valueProperty != null)
                            {
                                value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
                            }
                            rowDelegate(key, value, parameterObject, map);

                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                ExecutePostSelect(request);
            }
            return map;

        }


        #endregion


        #region ExecuteQueryForDataSet


        /// <summary>
        /// 执行sql返回DataSet数据集，没有应用结果策略(resultStrategy)，使用IDbDataAdapter直接返回DataSet
        /// </summary>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataSet of result objects.</returns>
        public virtual DataSet ExecuteQueryForDataSet(ISqlMapSession aSession, object aParameterObject)
        {
            RequestScope _request = this.statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            //创建命令对象IDbCommand，_request.IDbCommand=aSession.CreateCommand(aStatement.CommandType)
            //命令对象执行语句CommandText赋值，_request.IDbCommand.CommandText = _request.PreparedStatement.PreparedSql
            //给命令对象创建参数列表（aRequest.IDbCommand.Parameters），并赋值（从aParameterObject获取参数对应的值）
            //一切准备就绪，可执行命令获取数据
            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            return RunQueryForDataSet(_request, aSession, aParameterObject);
        }


        /// <summary>
        /// 执行sql返回DataSet数据集，没有应用结果策略，使用IDbDataAdapter直接返回DataSet
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataSet of result objects.</returns>
        internal DataSet RunQueryForDataSet(RequestScope aRequest, ISqlMapSession aSession, object aParameterObject)
        {
            DataSet _ds = new DataSet();

            using (IDbCommand command = ((DbCommandDecorator)aRequest.IDbCommand).IDbCommand)
            {
                try
                {
                    IDbDataAdapter _dataAdapter = aSession.CreateDataAdapter(command);
                    _dataAdapter.Fill(_ds);
                }
                catch
                {
                    throw;
                }

            }

            return _ds;
        }
        #endregion


        /// <summary>
        /// Executes the <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="aRequest">The current <see cref="RequestScope"/>.</param>
        private void ExecutePostSelect(RequestScope aRequest)
        {
            while (aRequest.QueueSelect.Count > 0)
            {
                PostBindind _postSelect = aRequest.QueueSelect.Dequeue() as PostBindind;

                PostSelectStrategyFactory.Get(_postSelect.Method).Execute(_postSelect, aRequest);
            }
        }


        /// <summary>
        /// Raise an event ExecuteEventArgs
        /// (Used when a query is executed)
        /// </summary>
        private void RaiseExecuteEvent()
        {
            ExecuteEventArgs e = new ExecuteEventArgs();
            e.StatementName = statement.Id;
            if (Execute != null)
            {
                Execute(this, e);
            }
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns>A string that describes the MappedStatement</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("\tMappedStatement: " + this.Id);
            buffer.Append(Environment.NewLine);
            if (statement.ParameterMap != null) buffer.Append(statement.ParameterMap.Id);

            return buffer.ToString();
        }


        #endregion

    }
}
