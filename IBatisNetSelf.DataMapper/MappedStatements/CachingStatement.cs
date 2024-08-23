using IBatisNetSelf.DataMapper.Commands;
using IBatisNetSelf.DataMapper.Configuration.Cache;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// Summary description for CachingStatement.
    /// </summary>
    public sealed class CachingStatement : IMappedStatement
    {
        private MappedStatement mappedStatement = null;

        /// <summary>
        /// Event launch on exceute query
        /// </summary>
        public event ExecuteEventHandler Execute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aMappedStatement"></param>
        public CachingStatement(MappedStatement aMappedStatement)
        {
            this.mappedStatement = aMappedStatement;
        }

        #region IMappedStatement Members


        /// <summary>
        /// The IPreparedCommand to use
        /// </summary>
        public IPreparedCommand PreparedCommand
        {
            get { return mappedStatement.PreparedCommand; }
        }

        /// <summary>
        /// Name used to identify the MappedStatement amongst the others.
        /// This the name of the SQL statment by default.
        /// </summary>
        public string Id
        {
            get { return mappedStatement.Id; }
        }

        /// <summary>
        /// The SQL statment used by this MappedStatement
        /// </summary>
        public IStatement Statement
        {
            get { return mappedStatement.Statement; }
        }

        /// <summary>
        /// The SqlMap used by this MappedStatement
        /// </summary>
        public ISqlMapper SqlMap
        {
            get { return mappedStatement.SqlMap; }
        }

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
        ///<exception cref="IBatisNet.DataMapper.Exceptions.DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            IDictionary map = new Hashtable();
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);

            mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);

            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForMap");
            if (keyProperty != null)
            {
                cacheKey.Update(keyProperty);
            }
            if (valueProperty != null)
            {
                cacheKey.Update(valueProperty);
            }

            map = this.Statement.CacheModel[cacheKey] as IDictionary;
            if (map == null)
            {
                map = mappedStatement.RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, null);
                this.Statement.CacheModel[cacheKey] = map;
            }

            return map;
        }


        /// <summary>
        /// Execute an update statement. Also used for delete statement.
        /// Return the number of row effected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The number of row effected.</returns>
        public int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            return mappedStatement.ExecuteUpdate(session, parameterObject);
        }

        /// <summary>
        /// Execute an insert statement. Fill the parameter object with 
        /// the ouput parameters if any, also could return the insert generated key
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="parameterObject">The parameter object used to fill the statement.</param>
        /// <returns>Can return the insert generated key.</returns>
        public object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            return mappedStatement.ExecuteInsert(session, parameterObject);
        }

        #region ExecuteQueryForList

        /// <summary>
        /// Executes the SQL and and fill a strongly typed collection.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">A strongly typed collection of result objects.</param>
        public void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject)
        {
            mappedStatement.ExecuteQueryForList(session, parameterObject, resultObject);
        }

        /// <summary>
        /// Executes the SQL and retuns a subset of the rows selected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        public IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList list = null;
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);

            mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);

            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForList");
            cacheKey.Update(skipResults);
            cacheKey.Update(maxResults);

            list = this.Statement.CacheModel[cacheKey] as IList;
            if (list == null)
            {
                list = mappedStatement.RunQueryForList(request, session, parameterObject, skipResults, maxResults);
                this.Statement.CacheModel[cacheKey] = list;
            }

            return list;
        }
        /// <summary>
        /// Executes the SQL and retuns all rows selected. This is exactly the same as
        /// calling ExecuteQueryForList(session, parameterObject, NO_SKIPPED_RESULTS, NO_MAXIMUM_RESULTS).
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        public IList ExecuteQueryForList(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForList(session, parameterObject, MappedStatement.NO_SKIPPED_RESULTS, MappedStatement.NO_MAXIMUM_RESULTS);
        }
        #endregion


        #region ExecuteQueryForObject

        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The object</returns>
        public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            return this.ExecuteQueryForObject(session, parameterObject, null);
        }

        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object of the type of
        /// the resultObject passed in as a parameter.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>The object</returns>
        public object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            object obj = null;
            RequestScope request = this.Statement.Sql.GetRequestScope(this, parameterObject, session);

            mappedStatement.PreparedCommand.Create(request, session, this.Statement, parameterObject);

            CacheKey cacheKey = this.GetCacheKey(request);
            cacheKey.Update("ExecuteQueryForObject");

            obj = this.Statement.CacheModel[cacheKey];
            // check if this query has alreay been run 
            if (obj == CacheModel.NULL_OBJECT)
            {
                // convert the marker object back into a null value 
                obj = null;
            }
            else if (obj == null)
            {
                obj = mappedStatement.RunQueryForObject(request, session, parameterObject, resultObject);
                this.Statement.CacheModel[cacheKey] = obj;
            }

            return obj;
        }
        #endregion


        /// <summary>
        /// Runs a query with a custom object that gets a chance 
        /// to deal with each row as it is processed.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate"></param>
        public IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            return mappedStatement.ExecuteQueryForRowDelegate(session, parameterObject, rowDelegate);
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
        /// <exception cref="IBatisNet.DataMapper.Exceptions.DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        public IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            return mappedStatement.ExecuteQueryForMapWithRowDelegate(session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }



        /// <summary>
        /// Executes the SQL and retuns fill a DataSet
        /// </summary>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataSet objects.</returns>
        public DataSet ExecuteQueryForDataSet(ISqlMapSession aSession, object aParameterObject)
        {
            return mappedStatement.ExecuteQueryForDataSet(aSession, aParameterObject);
        }
        #endregion

        /// <summary>
        /// Gets a percentage of successful cache hits achieved
        /// </summary>
        /// <returns>The percentage of hits (0-1), or -1 if cache is disabled.</returns>
        public double GetDataCacheHitRatio()
        {
            if (this.mappedStatement.Statement.CacheModel != null)
            {
                return this.mappedStatement.Statement.CacheModel.HitRatio;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <returns>the cache key</returns>
        private CacheKey GetCacheKey(RequestScope aRequest)
        {
            CacheKey _cacheKey = new CacheKey();
            int _count = aRequest.IDbCommand.Parameters.Count;
            for (int i = 0; i < _count; i++)
            {
                IDataParameter _dataParameter = (IDataParameter)aRequest.IDbCommand.Parameters[i];
                if (_dataParameter.Value != null)
                {
                    _cacheKey.Update(_dataParameter.Value);
                }
            }

            _cacheKey.Update(this.mappedStatement.Id);
            _cacheKey.Update(this.mappedStatement.SqlMap.DataSource.ConnectionString);
            _cacheKey.Update(aRequest.IDbCommand.CommandText);

            CacheModel _cacheModel = this.mappedStatement.Statement.CacheModel;
            if (!_cacheModel.IsReadOnly && !_cacheModel.IsSerializable)
            {
                _cacheKey.Update(aRequest);
            }
            return _cacheKey;
        }
    }
}
