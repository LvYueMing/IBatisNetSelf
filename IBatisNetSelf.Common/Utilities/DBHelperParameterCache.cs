using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// DBHelperParameterCache provides functions to leverage a 
    /// static cache of procedure parameters, and the
    /// ability to discover parameters for stored procedures at run-time.
    /// </summary>
    public sealed class DBHelperParameterCache
    {
        private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Initializes a new instance of the <see cref="DBHelperParameterCache"/> class.
        /// </summary>
        public DBHelperParameterCache() { }

        #region private methods

        /// <summary>
        /// Resolve at run time the appropriate set of Parameters for a stored procedure
        /// </summary>
        /// <param name="session">An IDalSession object</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">whether or not to include their return value parameter</param>
        /// <returns></returns>
        private IDataParameter[] DiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            return InternalDiscoverSpParameterSet(
                session,
                spName,
                includeReturnValueParameter);
        }


        /// <summary>
        /// Discover at run time the appropriate set of Parameters for a stored procedure
        /// </summary>
        /// <param name="session">An IDalSession object</param>
        /// <param name="spName">Name of the stored procedure.</param>
        /// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
        /// <returns>The stored procedure parameters.</returns>
        private IDataParameter[] InternalDiscoverSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            // SqlCommandBuilder.DeriveParameters(<command>) does not support transactions. 
            // If the command is within a transaction, you will get the following error: 
            // sqlCommandBuilder Execute requires the command to have a transaction object 
            // when the connection assigned to the command is in a pending local transaction?
            // even when the command object does in fact have a transaction object. 
            using (IDbCommand cmd = session.CreateCommand(CommandType.StoredProcedure))
            {
                cmd.CommandText = spName;

                // The session connection object is always created but the connection is not alwys open
                // so we try to open it in case.
                session.OpenConnection();

                DeriveParameters(session.DataSource.DbProvider, cmd);

                if (cmd.Parameters.Count > 0)
                {
                    IDataParameter _firstParameter = (IDataParameter)cmd.Parameters[0];
                    if (_firstParameter.Direction == ParameterDirection.ReturnValue)
                    {
                        if (!includeReturnValueParameter)
                        {
                            cmd.Parameters.RemoveAt(0);
                        }
                    }
                }


                IDataParameter[] discoveredParameters = new IDataParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(discoveredParameters, 0);
                return discoveredParameters;
            }
        }

        private void DeriveParameters(IDbProvider provider, IDbCommand command)
        {
            Type _commandBuilderType;

            // Find the CommandBuilder
            if (provider == null)
                throw new ArgumentNullException("provider");
            if ((provider.CommandBuilderClass == null) || (provider.CommandBuilderClass.Length < 1))
                throw new Exception($"CommandBuilderClass not defined for provider \"{provider.Name}\".");
            _commandBuilderType = provider.CommandBuilderType;

            // Invoke the static DeriveParameter method on the CommandBuilder class
            // NOTE: OracleCommandBuilder has no DeriveParameter method
            try
            {
                _commandBuilderType.InvokeMember("DeriveParameters",
                                                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static, null, null,
                                                new object[] { command });
            }
            catch (Exception ex)
            {
                throw new IBatisNetSelfException("Could not retrieve parameters for the store procedure named " + command.CommandText, ex);
            }
        }

        /// <summary>
        /// Deep copy of cached IDataParameter array.
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] _clonedParameters = new IDataParameter[originalParameters.Length];

            int length = originalParameters.Length;
            for (int i = 0, j = length; i < j; i++)
            {
                _clonedParameters[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return _clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for an IDbConnection</param>
        /// <param name="commandText">the stored procedure name or SQL command</param>
        /// <param name="commandParameters">an array of IDataParameters to be cached</param>
        public void CacheParameterSet(string connectionString, string commandText, params IDataParameter[] commandParameters)
        {
            string _hashKey = connectionString + ":" + commandText;

            this.paramCache[_hashKey] = commandParameters;
        }


        /// <summary>
        /// Clear the parameter cache.
        /// </summary>
        public void Clear()
        {
            this.paramCache.Clear();
        }

        /// <summary>
        /// retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">a valid connection string for an IDbConnection</param>
        /// <param name="commandText">the stored procedure name or SQL command</param>
        /// <returns>an array of IDataParameters</returns>
        public IDataParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            string _hashKey = connectionString + ":" + commandText;

            IDataParameter[] cachedParameters = (IDataParameter[])this.paramCache[_hashKey];

            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="session">a valid session</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <returns>an array of IDataParameters</returns>
        public IDataParameter[] GetSpParameterSet(IDalSession session, string spName)
        {
            return GetSpParameterSet(session, spName, false);
        }

        /// <summary>
        /// Retrieves the set of IDataParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="session">a valid session</param>
        /// <param name="spName">the name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>an array of IDataParameters</returns>
        public IDataParameter[] GetSpParameterSet(IDalSession session, string spName, bool includeReturnValueParameter)
        {
            string _hashKey = session.DataSource.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            IDataParameter[] _cachedParameters;

            _cachedParameters = (IDataParameter[])this.paramCache[_hashKey];

            if (_cachedParameters == null)
            {
                this.paramCache[_hashKey] = DiscoverSpParameterSet(session, spName, includeReturnValueParameter);
                _cachedParameters = (IDataParameter[])paramCache[_hashKey];
            }

            return CloneParameters(_cachedParameters);
        }

        #endregion Parameter Discovery Functions
    }
}
