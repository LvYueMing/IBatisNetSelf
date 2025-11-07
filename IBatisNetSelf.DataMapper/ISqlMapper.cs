using IBatisNetSelf.Common;
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
    /// Contract for an <see cref="ISqlMapper"/>
    /// </summary>
    public interface ISqlMapper
    {
        #region properties
        /// <summary>
        /// Name used to identify the the <see cref="SqlMapper"/>
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        IDataSource DataSource { get; set; }

        /// <summary>
        /// The ParameterMap collection
        /// </summary>
        HybridDictionary ParameterMaps { get; }

        /// <summary>
        /// The ResultMap collection
        /// </summary>
        HybridDictionary ResultMaps { get; }

        /// <summary>
        /// The MappedStatements collection
        /// </summary>
        HybridDictionary MappedStatements { get; }

        /// <summary>
        /// Allow to set a custom session store like the <see cref="HybridWebThreadSessionStore"/>
        /// </summary>
        /// <remarks>Set it after the configuration and before use of the <see cref="SqlMapper"/></remarks>
        /// <example>
        /// sqlMapper.SessionStore = new HybridWebThreadSessionStore( sqlMapper.Id );
        /// </example>
        ISessionStore SessionStore { set; }

        /// <summary>
        /// Gets a value indicating whether this instance is session started.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is session started; otherwise, <c>false</c>.
        /// </value>
        bool IsSessionStarted { get; }

        /// <summary>
        ///  Returns the DalSession instance 
        ///  currently being used by the SqlMap.
        /// </summary>
        ISqlMapSession LocalSession { get; }

        /// <summary>
        /// Gets the DB helper parameter cache.
        /// </summary>
        /// <value>The DB helper parameter cache.</value>
        DBHelperParameterCache DBHelperParameterCache { get; }


        /// <summary>
        /// Factory for DataExchange objects
        /// </summary>
        DataExchangeFactory DataExchangeFactory { get; }

        /// <summary>
        /// The TypeHandlerFactory
        /// </summary>
        TypeHandlerFactory TypeHandlerFactory { get; }

        /// <summary>
        /// The meta factory for object factory
        /// </summary>
        IObjectFactory ObjectFactory { get; }

        /// <summary>
        /// The factory which build <see cref="IAccessor"/>
        /// </summary>
        AccessorFactory AccessorFactory { get; }


        /// <summary>
        /// A flag that determines whether cache models were enabled 
        /// when this SqlMap was built.
        /// </summary>
        bool IsCacheModelsEnabled { get; set; }

        #endregion


        #region methods
        /// <summary>
        /// Get a ParameterMap by name
        /// </summary>
        /// <param name="name">The name of the ParameterMap</param>
        /// <returns>The ParameterMap</returns>
        ParameterMap GetParameterMap(string name);

        /// <summary>
        /// Adds a (named) ParameterMap.
        /// </summary>
        /// <param name="parameterMap">the ParameterMap to add</param>
        void AddParameterMap(ParameterMap parameterMap);

        /// <summary>
        /// Gets a ResultMap by name
        /// </summary>
        /// <param name="name">The name of the result map</param>
        /// <returns>The ResultMap</returns>
        IResultMap GetResultMap(string name);

        /// <summary>
        /// Adds a (named) ResultMap
        /// </summary>
        /// <param name="resultMap">The ResultMap to add</param>
        void AddResultMap(IResultMap resultMap);


        /// <summary>
        /// Gets a MappedStatement by name
        /// </summary>
        /// <param name="id"> The id of the statement</param>
        /// <returns> The MappedStatement</returns>
        IMappedStatement GetMappedStatement(string id);


        /// <summary>
        /// Adds a (named) MappedStatement.
        /// </summary>
        /// <param name="key"> The key name</param>
        /// <param name="mappedStatement">The statement to add</param>
        void AddMappedStatement(string key, IMappedStatement mappedStatement);

        /// <summary>
        /// Gets a cache by name
        /// </summary>
        /// <param name="name">The name of the cache to get</param>
        /// <returns>The cache object</returns>
             CacheModel GetCache(string name);

        /// <summary>
        /// Adds a (named) cache.
        /// </summary>
        /// <param name="cache">The cache to add</param>
            void AddCache(CacheModel cache);

        /// <summary>
        /// Flushes all cached objects that belong to this SqlMap
        /// </summary>
           void FlushCaches();

        /// <summary>
        /// Gets the data cache stats.
        /// </summary>
        /// <returns></returns>
        //     string GetDataCacheStats();



        /// <summary>
        /// Creates a new SqlMapSession that will be used to query the data source.
        /// </summary>
        /// <returns>A new session</returns>
        ISqlMapSession CreateSqlMapSession();

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <returns></returns>
        ISqlMapSession OpenConnection();

        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        ISqlMapSession OpenConnection(string connectionString);


        /// <summary>
        /// Closes the connection.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        ISqlMapSession BeginTransaction();

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="openConnection">if set to <c>true</c> [open connection].</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(bool openConnection);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(string connectionString);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="openNewConnection">if set to <c>true</c> [open new connection].</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(bool openNewConnection, IsolationLevel isolationLevel);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="openNewConnection">if set to <c>true</c> [open new connection].</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(string connectionString, bool openNewConnection, IsolationLevel isolationLevel);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        ISqlMapSession BeginTransaction(string connectionString, IsolationLevel isolationLevel);

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <param name="closeConnection">if set to <c>true</c> [close connection].</param>
        void CommitTransaction(bool closeConnection);

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rolls the back transaction.
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// Rolls the back transaction.
        /// </summary>
        /// <param name="closeConnection">if set to <c>true</c> [close connection].</param>
        void RollBackTransaction(bool closeConnection);

        #endregion


        #region execute sql methods

        /// <summary>
        /// Alias to QueryForMap, .NET spirit.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <param name="valueProperty">The property of the result object to be used as the value (or null)</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        ///<exception cref="DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        //   IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty, string valueProperty);

        /// <summary>
        ///  Alias to QueryForMap, .NET spirit.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
        //   IDictionary QueryForDictionary(string statementName, object parameterObject, string keyProperty);

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
        //   void QueryForList(string statementName, object parameterObject, IList resultObject);

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
        IList QueryForList(string statementName, object parameterObject);

        IList QueryForList<T>(string statementName, object parameterObject);

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
     //   IList QueryForList(string statementName, object parameterObject, int skipResults, int maxResults);


        /// <summary>
        ///  Executes the SQL and retuns all rows selected in a map that is keyed on the property named
        ///  in the keyProperty parameter.  The value at each key will be the entire result object.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="keyProperty">The property of the result object to be used as the key.</param>
        /// <returns>A IDictionary (Hashtable) of object containing the rows keyed by keyProperty.</returns>
     //   IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty);

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
     //   IDictionary QueryForMap(string statementName, object parameterObject, string keyProperty, string valueProperty);


        /// <summary>
        /// Executes a Sql SELECT statement that returns a single object of the type of the
        /// resultObject parameter.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">An object of the type to be returned.</param>
        /// <returns>The single result object populated with the result set data.</returns>
        object QueryForObject(string statementName, object parameterObject, object resultObject);

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
        object QueryForObject(string statementName, object parameterObject);

        /// <summary>
        /// 执行一个 SQL SELECT 查询语句，用于返回数据并填充一个对象实例（泛型版）。
        /// 参数对象通常用于提供 SELECT 语句中 WHERE 子句的输入参数。
        /// </summary>
        /// <typeparam name="T">期望返回的结果类型</typeparam>
        /// <param name="statementName">要执行的 SQL 语句名称。</param>
        /// <param name="parameterObject">用于设置 SQL 参数的对象。</param>
        /// <returns>一个被结果集数据填充的单一对象（T 类型）。</returns>
        public T QueryForObject<T>(string statementName, object parameterObject);


        /// <summary>
        /// 执行一个 Sql SELECT 语句，返回一个指定类型的对象，类型由 resultObject 参数指定（泛型版）。
        /// </summary>
        /// <typeparam name="T">期望返回的结果类型</typeparam>
        /// <param name="statementName">要执行的 SQL 映射语句名称。</param>
        /// <param name="parameterObject">用于填充 SQL 参数的对象。</param>
        /// <param name="resultObject">作为返回类型模板的对象。</param>
        /// <returns>一个填充了结果数据的对象（T 类型）。</returns>
        public T QueryForObject<T>(string statementName, object parameterObject, T resultObject);



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
        IList QueryWithRowDelegate(string statementName, object parameterObject, RowDelegate rowDelegate);


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
        IDictionary QueryForMapWithRowDelegate(string statementName, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);


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
        DataSet QueryForDataSet(string statementName, object parameterObject);


        /// <summary>
        /// Executes a Sql SELECT statement that returns data to populate a DataTable.
        /// <p/>
        ///  The parameter object is generally used to supply the input
        /// data for the WHERE clause parameter(s) of the SELECT statement.
        /// </summary>
        /// <param name="statementName">The name of the sql statement to execute.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataTable</returns>
        DataTable QueryForDataTable(string statementName, object parameterObject);

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
        //    object Insert(string statementName, object parameterObject);

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
        //  int Update(string statementName, object parameterObject);

        /// <summary>
        ///  Executes a Sql DELETE statement.
        ///  Delete returns the number of rows effected.
        /// </summary>
        /// <param name="statementName">The name of the statement to execute.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <returns>The number of rows effected.</returns>
        //    int Delete(string statementName, object parameterObject);

        #endregion

    }
}
