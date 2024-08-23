using IBatisNetSelf.DataMapper.Commands;
using IBatisNetSelf.DataMapper.Configuration.Statements;
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
    /// 
    /// </summary>
    public delegate void ExecuteEventHandler(object sender, ExecuteEventArgs e);

    /// <summary>
    /// Summary description for IMappedStatement.
    /// </summary>
    public interface IMappedStatement
    {

        #region Event

        /// <summary>
        /// Event launch on exceute query
        /// </summary>
        event ExecuteEventHandler Execute;

        #endregion

        #region Properties


        /// <summary>
        /// Name used to identify the MappedStatement amongst the others.
        /// This the name of the SQL statment by default.
        /// </summary>
        string Id { get; }


        /// <summary>
        /// The IPreparedCommand to use
        /// </summary>
        IPreparedCommand PreparedCommand { get; }



        /// <summary>
        /// The SQL statment used by this MappedStatement
        /// </summary>
        IStatement Statement { get; }


        /// <summary>
        /// The SqlMap used by this MappedStatement
        /// </summary>
        ISqlMapper SqlMap { get; }
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
        ///<exception cref="IBatisNet.DataMapper.Exceptions.DataMapperException">If a transaction is not in progress, or the database throws an exception.</exception>
        IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty);

        #endregion

        #region ExecuteUpdate

        /// <summary>
        /// Execute an update statement. Also used for delete statement.
        /// Return the number of row effected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The number of row effected.</returns>
        int ExecuteUpdate(ISqlMapSession session, object parameterObject);

        #endregion

        #region ExecuteInsert

        /// <summary>
        /// Execute an insert statement. Fill the parameter object with 
        /// the ouput parameters if any, also could return the insert generated key
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="parameterObject">The parameter object used to fill the statement.</param>
        /// <returns>Can return the insert generated key.</returns>
        object ExecuteInsert(ISqlMapSession session, object parameterObject);

        #endregion

        #region ExecuteQueryForList

        /// <summary>
        /// Executes the SQL and and fill a strongly typed collection.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">A strongly typed collection of result objects.</param>
        void ExecuteQueryForList(ISqlMapSession session, object parameterObject, IList resultObject);

        /// <summary>
        /// Executes the SQL and retuns a subset of the rows selected.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="skipResults">The number of rows to skip over.</param>
        /// <param name="maxResults">The maximum number of rows to return.</param>
        /// <returns>A List of result objects.</returns>
        IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults);

        /// <summary>
        /// Executes the SQL and retuns all rows selected. This is exactly the same as
        /// calling ExecuteQueryForList(session, parameterObject, NO_SKIPPED_RESULTS, NO_MAXIMUM_RESULTS).
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A List of result objects.</returns>
        IList ExecuteQueryForList(ISqlMapSession session, object parameterObject);

        #endregion

        #region ExecuteForObject

        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>The object</returns>
        object ExecuteQueryForObject(ISqlMapSession session, object parameterObject);

        /// <summary>
        /// Executes an SQL statement that returns a single row as an Object of the type of
        /// the resultObject passed in as a parameter.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>The object</returns>
        object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject);

        #endregion

        #region Delegate

        /// <summary>
        /// Runs a query with a custom object that gets a chance 
        /// to deal with each row as it is processed.
        /// </summary>
        /// <param name="session">The session used to execute the statement.</param>
        /// <param name="parameterObject">The object used to set the parameters in the SQL.</param>
        /// <param name="rowDelegate"></param>param>
        /// <returns></returns>
        IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate);


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
        IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate);

        #endregion

        #region ExecuteForDataSet
        /// <summary>
        /// Executes the SQL and retuns DataSet
        /// </summary>
        /// <param name="aSession">The session used to execute the statement.</param>
        /// <param name="aParameterObject">The object used to set the parameters in the SQL.</param>
        /// <returns>A DataSet of result objects.</returns>
        DataSet ExecuteQueryForDataSet(ISqlMapSession aSession, object aParameterObject);
        #endregion


    }
}
