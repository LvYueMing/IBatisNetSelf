using IBatisNetSelf.Common;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.Configuration.Cache;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.SessionStore;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// Summary description for SqlMap.
    /// </summary>
    public class SqlMapper : ISqlMapper
    {
        #region Fields
        // An identifiant 
        private string id = string.Empty;

        //(MappedStatement Name, MappedStatement)
        private HybridDictionary mappedStatements = new HybridDictionary();
        //(ResultMap name, ResultMap)
        private HybridDictionary resultMaps = new HybridDictionary();
        //(ParameterMap name, ParameterMap)
        private HybridDictionary parameterMaps = new HybridDictionary();
        // DataSource
        private IDataSource dataSource = null;
        //(CacheModel name, cache))
        private HybridDictionary cacheMaps = new HybridDictionary();
        private TypeHandlerFactory typeHandlerFactory = null;
        //private DBHelperParameterCache dbHelperParameterCache = null;

        private bool cacheModelsEnabled = false;


        /// <summary>
        /// Container session unique for each thread. 
        /// </summary>
        private ISessionStore sessionStore = null;
        private IObjectFactory objectFactory = null;
        private AccessorFactory accessorFactory = null;
        private DataExchangeFactory dataExchangeFactory = null;
        #endregion

        #region Properties

        /// <summary>
        /// Name used to identify the the <see cref="SqlMapper"/>
        /// </summary>
        public string Id => id;

        /// <summary>
        /// Allow to set a custom session store like the <see cref="HybridWebThreadSessionStore"/>
        /// </summary>
        /// <remarks>Set it after the configuration and before use of the <see cref="SqlMapper"/></remarks>
        /// <example>
        /// sqlMapper.SessionStore = new HybridWebThreadSessionStore( sqlMapper.Id );
        /// </example>
        public ISessionStore SessionStore  
        {
            set { this.sessionStore = value; }
        }

        /// <summary>
        ///  Returns the DalSession instance 
        ///  currently being used by the SqlMap.
        /// </summary>
        public ISqlMapSession LocalSession =>  this.sessionStore.LocalSession;

        /// <summary>
        /// Gets a value indicating whether this instance is session started.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is session started; otherwise, <c>false</c>.
        /// </value>
        public bool IsSessionStarted => (sessionStore.LocalSession != null);

        /// <summary>
        /// Gets the DB helper parameter cache.
        /// </summary>
        /// <value>The DB helper parameter cache.</value>
    //    public DBHelperParameterCache DBHelperParameterCache => this.dbHelperParameterCache; 

        /// <summary>
        /// Factory for DataExchange objects
        /// </summary>
        public DataExchangeFactory DataExchangeFactory =>  this.dataExchangeFactory; 

        /// <summary>
        /// The TypeHandlerFactory
        /// </summary>
        public TypeHandlerFactory TypeHandlerFactory => this.typeHandlerFactory; 

        /// <summary>
        /// The meta factory for object factory
        /// </summary>
        public IObjectFactory ObjectFactory => this.objectFactory; 

        /// <summary>
        /// The factory which build <see cref="IAccessor"/>
        /// </summary>
        public AccessorFactory AccessorFactory => this.accessorFactory;


        /// <summary>
        /// A flag that determines whether cache models were enabled 
        /// when this SqlMap was built.
        /// </summary>
        public bool IsCacheModelsEnabled
        {
            set { this.cacheModelsEnabled = value; }
            get { return this.cacheModelsEnabled; }
        }

        #endregion


        #region Constructor (s) / Destructor


        /// <summary>
        /// Initializes a new instance of the <see cref="SqlMapper"/> class.
        /// </summary>
        /// <param name="aObjectFactory">The object factory.</param>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public SqlMapper(IObjectFactory aObjectFactory, AccessorFactory aAccessorFactory)
        {
            this.typeHandlerFactory = new TypeHandlerFactory();
            //this.dbHelperParameterCache = new DBHelperParameterCache();
            this.objectFactory = aObjectFactory;
            this.accessorFactory = aAccessorFactory;

            this.dataExchangeFactory = new DataExchangeFactory(this.typeHandlerFactory, objectFactory, aAccessorFactory);

            this.id = HashCodeProvider.GetIdentityHashCode(this).ToString();
            this.sessionStore = SessionStoreFactory.GetSessionStore(this.id);
        }


        #endregion


        #region Methods

        /// <summary>
        /// Creates a new SqlMapSession that will be used to query the data source.
        /// </summary>
        /// <returns>A new session</returns>
        public ISqlMapSession CreateSqlMapSession()
        {
            ISqlMapSession _session = new SqlMapSession(this);
            _session.CreateConnection();

            return _session;
        }


        /// <summary>
        /// Creates the SQL map session.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A new session</returns>
        public ISqlMapSession CreateSqlMapSession(string connectionString)
        {
            ISqlMapSession _session = new SqlMapSession(this);
            _session.CreateConnection(connectionString);

            return _session;
        }

        #region Get/Add ParemeterMap, ResultMap, MappedStatement, TypeAlias, DataSource, CacheModel

        /// <summary>
        /// Gets a MappedStatement by name
        /// </summary>
        /// <param name="id"> The id of the statement</param>
        /// <returns> The MappedStatement</returns>
        public IMappedStatement GetMappedStatement(string id)
        {
            if (this.mappedStatements.Contains(id) == false)
            {
                throw new DataMapperException("This SQL map does not contain a MappedStatement named " + id);
            }
            return (IMappedStatement)this.mappedStatements[id];
        }

        /// <summary>
        /// Adds a (named) MappedStatement.
        /// </summary>
        /// <param name="key"> The key name</param>
        /// <param name="mappedStatement">The statement to add</param>
        public void AddMappedStatement(string key, IMappedStatement mappedStatement)
        {
            if (this.mappedStatements.Contains(key) == true)
            {
                throw new DataMapperException("This SQL map already contains a MappedStatement named " + mappedStatement.Id);
            }
            this.mappedStatements.Add(key, mappedStatement);
        }

        /// <summary>
        /// The MappedStatements collection
        /// </summary>
        public HybridDictionary MappedStatements
        {
            get { return mappedStatements; }
        }

        /// <summary>
        /// Get a ParameterMap by name
        /// </summary>
        /// <param name="name">The name of the ParameterMap</param>
        /// <returns>The ParameterMap</returns>
        public ParameterMap GetParameterMap(string name)
        {
            if (!parameterMaps.Contains(name))
            {
                throw new DataMapperException("This SQL map does not contain an ParameterMap named " + name + ".  ");
            }
            return (ParameterMap)parameterMaps[name];
        }

        /// <summary>
        /// Adds a (named) ParameterMap.
        /// </summary>
        /// <param name="parameterMap">the ParameterMap to add</param>
        public void AddParameterMap(ParameterMap parameterMap)
        {
            if (this.parameterMaps.Contains(parameterMap.Id) == true)
            {
                throw new DataMapperException("This SQL map already contains an ParameterMap named " + parameterMap.Id);
            }
            this.parameterMaps.Add(parameterMap.Id, parameterMap);
        }

        /// <summary>
        /// Gets a ResultMap by name
        /// </summary>
        /// <param name="name">The name of the result map</param>
        /// <returns>The ResultMap</returns>
        public IResultMap GetResultMap(string name)
        {
            if (resultMaps.Contains(name) == false)
            {
                throw new DataMapperException("This SQL map does not contain an ResultMap named " + name);
            }
            return (ResultMap)resultMaps[name];
        }

        /// <summary>
        /// Adds a (named) ResultMap
        /// </summary>
        /// <param name="resultMap">The ResultMap to add</param>
        public void AddResultMap(IResultMap resultMap)
        {
            if (resultMaps.Contains(resultMap.Id) == true)
            {
                throw new DataMapperException("This SQL map already contains an ResultMap named " + resultMap.Id);
            }
            resultMaps.Add(resultMap.Id, resultMap);
        }

        /// <summary>
        /// The ParameterMap collection
        /// </summary>
        public HybridDictionary ParameterMaps => this.parameterMaps;


        /// <summary>
        /// The ResultMap collection
        /// </summary>
        public HybridDictionary ResultMaps => this.resultMaps;


        /// <summary>
        /// The DataSource
        /// </summary>
        public IDataSource DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        /// <summary>
        /// Flushes all cached objects that belong to this SqlMap
        /// </summary>
        public void FlushCaches()
        {
            IDictionaryEnumerator enumerator = cacheMaps.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ((CacheModel)enumerator.Value).Flush();
            }
        }

        /// <summary>
        /// Adds a (named) cache.
        /// </summary>
        /// <param name="cache">The cache to add</param>
        public void AddCache(CacheModel cache)
        {
            if (cacheMaps.Contains(cache.Id))
            {
                throw new DataMapperException("This SQL map already contains an Cache named " + cache.Id);
            }
            cacheMaps.Add(cache.Id, cache);
        }

        /// <summary>
        /// Gets a cache by name
        /// </summary>
        /// <param name="name">The name of the cache to get</param>
        /// <returns>The cache object</returns>
        public CacheModel GetCache(string name)
        {
            if (!cacheMaps.Contains(name))
            {
                throw new DataMapperException("This SQL map does not contain an Cache named " + name);
            }
            return (CacheModel)cacheMaps[name];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public string GetDataCacheStats()
        //{
        //    StringBuilder buffer = new StringBuilder();
        //    buffer.Append(Environment.NewLine);
        //    buffer.Append("Cache Data Statistics");
        //    buffer.Append(Environment.NewLine);
        //    buffer.Append("=====================");
        //    buffer.Append(Environment.NewLine);

        //    IDictionaryEnumerator enumerator = _mappedStatements.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        IMappedStatement mappedStatement = (IMappedStatement)enumerator.Value;

        //        buffer.Append(mappedStatement.Id);
        //        buffer.Append(": ");

        //        if (mappedStatement is CachingStatement)
        //        {
        //            double hitRatio = ((CachingStatement)mappedStatement).GetDataCacheHitRatio();
        //            if (hitRatio != -1)
        //            {
        //                buffer.Append(Math.Round(hitRatio * 100));
        //                buffer.Append("%");
        //            }
        //            else
        //            {
        //                // this statement has a cache but it hasn't been accessed yet
        //                // buffer.Append("Cache has not been accessed."); ???
        //                buffer.Append("No Cache.");
        //            }
        //        }
        //        else
        //        {
        //            buffer.Append("No Cache.");
        //        }

        //        buffer.Append(Environment.NewLine);
        //    }

        //    return buffer.ToString();
        //}

        #endregion

        #region Manage Connection, Transaction

        /// <summary>
        /// Open a connection
        /// </summary>
        /// <returns></returns>
        public ISqlMapSession OpenConnection()
        {
            if (this.sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
            }
            ISqlMapSession _session = CreateSqlMapSession();
            this.sessionStore.Store(_session);
            return _session;
        }

        /// <summary>
        /// Open a connection, on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public ISqlMapSession OpenConnection(string connectionString)
        {
            if (this.sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke OpenConnection(). A connection is already started. Call CloseConnection first.");
            }
            ISqlMapSession session = CreateSqlMapSession(connectionString);
            this.sessionStore.Store(session);
            return session;
        }

        /// <summary>
        /// Open a connection
        /// </summary>
        public void CloseConnection()
        {
            if (this.sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CloseConnection(). No connection was started. Call OpenConnection() first.");
            }
            try
            {
                ISqlMapSession session = sessionStore.LocalSession;
                session.CloseConnection();
            }
            catch (Exception ex)
            {
                throw new DataMapperException("SqlMapper could not CloseConnection(). Cause :" + ex.Message, ex);
            }
            finally
            {
                sessionStore.Dispose();
            }
        }


        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        public ISqlMapSession BeginTransaction()
        {
            if (sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = CreateSqlMapSession();
            sessionStore.Store(session);
            session.BeginTransaction();
            return session;
        }

        /// <summary>
        /// Open a connection and begin a transaction on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public ISqlMapSession BeginTransaction(string connectionString)
        {
            if (sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = CreateSqlMapSession(connectionString);
            sessionStore.Store(session);
            session.BeginTransaction(connectionString);
            return session;
        }

        /// <summary>
        /// Begins a database transaction on the currect session
        /// </summary>
        /// <param name="openConnection">Open a connection.</param>
        public ISqlMapSession BeginTransaction(bool openConnection)
        {
            ISqlMapSession session = null;

            if (openConnection)
            {
                session = this.BeginTransaction();
            }
            else
            {
                session = sessionStore.LocalSession;
                if (session == null)
                {
                    throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
                }
                session.BeginTransaction(openConnection);
            }

            return session;
        }

        /// <summary>
        /// Begins a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level under which the transaction should run.
        /// </param>
        public ISqlMapSession BeginTransaction(IsolationLevel isolationLevel)
        {
            if (sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = CreateSqlMapSession();
            sessionStore.Store(session);
            session.BeginTransaction(isolationLevel);
            return session;
        }

        /// <summary>
        /// Open a connection and begin a transaction on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="isolationLevel">The transaction isolation level for this connection.</param>
        public ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel)
        {
            if (sessionStore.LocalSession != null)
            {
                throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A Transaction is already started. Call CommitTransaction() or RollbackTransaction first.");
            }
            ISqlMapSession session = CreateSqlMapSession(connectionString);
            sessionStore.Store(session);
            session.BeginTransaction(connectionString, isolationLevel);
            return session;
        }

        /// <summary>
        /// Start a database transaction on the current session
        /// with the specified isolation level.
        /// </summary>
        /// <param name="openNewConnection">Open a connection.</param>
        /// <param name="isolationLevel">
        /// The isolation level under which the transaction should run.
        /// </param>
        public ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel)
        {
            ISqlMapSession session = null;

            if (openNewConnection)
            {
                session = this.BeginTransaction(isolationLevel);
            }
            else
            {
                session = sessionStore.LocalSession;
                if (session == null)
                {
                    throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
                }
                session.BeginTransaction(openNewConnection, isolationLevel);
            }
            return session;
        }

        /// <summary>
        /// Begins a transaction on the current connection
        /// with the specified IsolationLevel value.
        /// </summary>
        /// <param name="isolationLevel">The transaction isolation level for this connection.</param>
        /// <param name="connectionString">The connection string</param>
        /// <param name="openNewConnection">Open a connection.</param>
        public ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel)
        {
            ISqlMapSession session = null;

            if (openNewConnection)
            {
                session = this.BeginTransaction(connectionString, isolationLevel);
            }
            else
            {
                session = sessionStore.LocalSession;
                if (session == null)
                {
                    throw new DataMapperException("SqlMap could not invoke BeginTransaction(). A session must be Open. Call OpenConnection() first.");
                }
                session.BeginTransaction(connectionString, openNewConnection, isolationLevel);
            }
            return session;
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <remarks>
        /// Will close the connection.
        /// </remarks>
        public void CommitTransaction()
        {
            if (sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                ISqlMapSession session = sessionStore.LocalSession;
                session.CommitTransaction();
            }
            finally
            {
                sessionStore.Dispose();
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <param name="closeConnection">Close the connection</param>
        public void CommitTransaction(bool closeConnection)
        {
            if (sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke CommitTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                ISqlMapSession session = sessionStore.LocalSession;
                session.CommitTransaction(closeConnection);
            }
            finally
            {
                if (closeConnection)
                {
                    sessionStore.Dispose();
                }
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <remarks>
        /// Will close the connection.
        /// </remarks>
        public void RollBackTransaction()
        {
            if (sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                ISqlMapSession session = sessionStore.LocalSession;
                session.RollBackTransaction();
            }
            finally
            {
                sessionStore.Dispose();
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <param name="closeConnection">Close the connection</param>
        public void RollBackTransaction(bool closeConnection)
        {
            if (sessionStore.LocalSession == null)
            {
                throw new DataMapperException("SqlMap could not invoke RollBackTransaction(). No Transaction was started. Call BeginTransaction() first.");
            }
            try
            {
                ISqlMapSession session = sessionStore.LocalSession;
                session.RollBackTransaction(closeConnection);
            }
            finally
            {
                if (closeConnection)
                {
                    sessionStore.Dispose();
                }
            }
        }

        #endregion

        #region QueryForObject

        /// <summary>
        /// Executes a Sql SELECT statement that returns that returns data 
        /// to populate a single object instance.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns> The single result object populated with the result set data.</returns>
        public object QueryForObject(string statementName, object parameterObject)
        {
            bool isSessionLocal = false;
            ISqlMapSession _session = sessionStore.LocalSession;
            object result;

            if (_session == null)
            {
                _session = CreateSqlMapSession();
                isSessionLocal = true;
            }

            try
            {
                IMappedStatement statement = GetMappedStatement(statementName);
                result = statement.ExecuteQueryForObject(_session, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isSessionLocal)
                {
                    _session.CloseConnection();
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a Sql SELECT statement that returns a single object of the type of the
        /// resultObject parameter.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An object of the type to be returned.</param>
        /// <returns>The single result object populated with the result set data.</returns>
        public object QueryForObject(string statementName, object parameterObject, object resultObject)
        {
            bool _isSessionLocal = false;
            ISqlMapSession _session = this.sessionStore.LocalSession;
            object _result = null;

            if (_session == null)
            {
                _session = CreateSqlMapSession();
                _isSessionLocal = true;
            }

            try
            {
                IMappedStatement statement = GetMappedStatement(statementName);
                _result = statement.ExecuteQueryForObject(_session, parameterObject, resultObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isSessionLocal)
                {
                    _session.CloseConnection();
                }
            }

            return _result;
        }

        #endregion

        #region QueryForMap, QueryForDictionary

        /// <summary>
        ///  Alias to QueryForMap, .NET spirit.
        ///  Feature idea by Ted Husted.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        //public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty)
        //{
        //	return QueryForMap(statementName, parameterObject, keyProperty);
        //}

        /// <summary>
        /// Alias to QueryForMap, .NET spirit.
        ///  Feature idea by Ted Husted.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        //public IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty)
        //{
        //	return QueryForMap(statementName, parameterObject, keyProperty, valueProperty);
        //}

        /// <summary>
        ///  Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        ///  in the keyProperty parameter.  The value at each key will be the entire result object.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        //public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty)
        //{
        //	return QueryForMap(statementName, parameterObject, keyProperty, null);
        //}

        /// <summary>
        /// Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        /// in the keyProperty parameter.  The value at each key will be the value of the property specified
        /// in the valueProperty parameter.  If valueProperty is null, the entire result object will be entered.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        //public IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	IDictionary map = null;

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		map = statement.ExecuteQueryForMap(session, parameterObject, keyProperty, valueProperty);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return map;
        //}

        #endregion

        #region QueryForList

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        public IList QueryForList(string statementName, object parameterObject)
        {
            bool _isSessionLocal = false;
            ISqlMapSession _session = this.sessionStore.LocalSession;
            IList _list;

            if (_session == null)
            {
                _session = CreateSqlMapSession();
                _isSessionLocal = true;
            }

            try
            {
                IMappedStatement _statement = GetMappedStatement(statementName);
                _list = _statement.ExecuteQueryForList(_session, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isSessionLocal)
                {
                    _session.CloseConnection();
                }
            }

            return _list;
        }

        /// <summary>
        /// Executes the SQL and retuns all rows selected.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        //public IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	IList list;

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		list = statement.ExecuteQueryForList(session, parameterObject, skipResults, maxResults);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return list;
        //}


        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a number of result objects.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An Ilist object used to hold the objects.</param>
        /// <returns>A List of result objects.</returns>
        //public void QueryForList(string statementName, object parameterObject, IList resultObject)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;

        //	if (resultObject == null)
        //	{
        //		throw new DataMapperException("resultObject parameter must be instantiated before being passed to SqlMapper.QueryForList");
        //	}

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		statement.ExecuteQueryForList(session, parameterObject, resultObject);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}
        //}

        #endregion

        #region QueryForPaginatedList
        /// <summary>
        /// Executes the SQL and retuns a subset of the results in a dynamic PaginatedList that can be used to
        /// automatically scroll through results from a database table.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL</param>
        /// <param name="pageSize">The maximum number of objects to store in each page</param>
        /// <returns>A PaginatedList of beans containing the rows</returns>
        //[Obsolete("This method will be remove in future version.", false)]
        //public PaginatedList QueryForPaginatedList(String statementName, object parameterObject, int pageSize)
        //{
        //	IMappedStatement statement = GetMappedStatement(statementName);
        //	return new PaginatedList(statement, parameterObject, pageSize);
        //}
        #endregion

        #region QueryWithRowDelegate

        /// <summary>
        /// Runs a query for list with a custom object that gets a chance to deal 
        /// with each row as it is processed.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate"></param>
        /// <returns>A List of result objects.</returns>
        //public IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	IList list = null;

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		list = statement.ExecuteQueryForRowDelegate(session, parameterObject, rowDelegate);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return list;
        //}


        /// <summary>
        /// Runs a query with a custom object that gets a chance to deal 
        /// with each row as it is processed.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <param name="rowDelegate"></param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        //public IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	IDictionary map = null;

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		map = statement.ExecuteQueryForMapWithRowDelegate(session, parameterObject, keyProperty, valueProperty, rowDelegate);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return map;
        //}

        #endregion

        #region Query Insert, Update, Delete

        /// <summary>
        /// Executes a Sql INSERT statement.
        /// Insert is a bit different from other update methods, as it
        /// provides facilities for returning the primary key of the
        /// newly inserted row (rather than the effected rows).  This
        /// functionality is of course optional.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the INSERT values.
        /// </summary>
        /// <param name="statementName">The name of the statement to execute.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns> The primary key of the newly inserted row.  
        /// This might be automatically generated by the RDBMS, 
        /// or selected from a sequence table or other source.
        /// </returns>
        //public object Insert(string statementName, object parameterObject)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	object generatedKey = null;

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		generatedKey = statement.ExecuteInsert(session, parameterObject);
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return generatedKey;
        //}

        /// <summary>
        /// Executes a Sql UPDATE statement.
        /// Update can also be used for any other update statement type,
        /// such as inserts and deletes.  Update returns the number of
        /// rows effected.
        /// <p/>
        /// The parameter object is generally used to supply the input
        /// data for the UPDATE values as well as the WHERE clause parameter(s).
        /// </summary>
        /// <param name="statementName">The name of the statement to execute.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>The number of rows effected.</returns>
        //public int Update(string statementName, object parameterObject)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	int rows = 0; // the number of rows affected

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		rows = statement.ExecuteUpdate(session, parameterObject);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return rows;
        //}

        /// <summary>
        ///  Executes a Sql DELETE statement.
        ///  Delete returns the number of rows effected.
        /// </summary>
        /// <param name="statementName">The name of the statement to execute.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>The number of rows effected.</returns>
        //public int Delete(string statementName, object parameterObject)
        //{
        //	bool isSessionLocal = false;
        //	ISqlMapSession session = _sessionStore.LocalSession;
        //	int rows = 0; // the number of rows affected

        //	if (session == null)
        //	{
        //		session = CreateSqlMapSession();
        //		isSessionLocal = true;
        //	}

        //	try
        //	{
        //		IMappedStatement statement = GetMappedStatement(statementName);
        //		rows = statement.ExecuteUpdate(session, parameterObject);
        //	}
        //	catch
        //	{
        //		throw;
        //	}
        //	finally
        //	{
        //		if (isSessionLocal)
        //		{
        //			session.CloseConnection();
        //		}
        //	}

        //	return rows;
        //}

        #endregion

        #region QueryForDataSet

        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate
        /// a DataSet.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataSet</returns>
        public DataSet QueryForDataSet(string statementName, object parameterObject)
        {
            bool _isSessionLocal = false;
            ISqlMapSession _session = this.sessionStore.LocalSession;
            DataSet _ds;

            if (_session == null)
            {
                _session = CreateSqlMapSession();
                _isSessionLocal = true;
            }

            try
            {
                IMappedStatement _statement = GetMappedStatement(statementName);
                _ds = _statement.ExecuteQueryForDataSet(_session, parameterObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isSessionLocal)
                {
                    _session.CloseConnection();
                }
            }

            return _ds;
        }

        #endregion


        #endregion
    }
}
