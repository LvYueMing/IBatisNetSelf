using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBatisNetSelf.DataMapper.Exceptions;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// Summary description for SqlMapSession.
    /// </summary>
    [Serializable]
    public class SqlMapSession : ISqlMapSession
    {
        #region Fields
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ISqlMapper sqlMapper = null;
        private IDataSource dataSource = null;

        #endregion

        #region Fields

        private bool isTransactionOpen = false;
        /// <summary>
        /// Changes the vote to commit (true) or to abort (false) in transsaction
        /// </summary>
        private bool consistent = false;

        /// <summary>
        /// Holds value of connection
        /// </summary>
        private IDbConnection connection = null;

        /// <summary>
        /// Holds value of transaction
        /// </summary>
        private IDbTransaction transaction = null;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aSqlMapper"></param>
        public SqlMapSession(ISqlMapper aSqlMapper)
        {
            this.dataSource = aSqlMapper.DataSource;
            this.sqlMapper = aSqlMapper;
        }
        #endregion


        #region ISqlMapSession Members
        /// <summary>
        /// Gets the SQL mapper.
        /// </summary>
        /// <value>The SQL mapper.</value>
        public ISqlMapper SqlMapper
        {
            get { return this.sqlMapper; }
        }

        /// <summary>
        /// Create the connection
        /// </summary>
        public void CreateConnection()
        {
            this.CreateConnection(this.dataSource.ConnectionString);
        }

        /// <summary>
        /// Create the connection
        /// </summary>
        public void CreateConnection(string connectionString)
        {
            this.connection = this.dataSource.DbProvider.CreateConnection();
            this.connection.ConnectionString = connectionString;
        }

        #endregion

        #region IDalSession Members


        #region Properties
        /// <summary>
        /// The data source use by the session.
        /// </summary>
        /// <value></value>
        public IDataSource DataSource
        {
            get { return this.dataSource; }
        }


        /// <summary>
        /// The Connection use by the session.
        /// </summary>
        /// <value></value>
        public IDbConnection Connection
        {
            get { return connection; }
        }


        /// <summary>
        /// The Transaction use by the session.
        /// </summary>
        /// <value></value>
        public IDbTransaction Transaction
        {
            get { return transaction; }
        }

        /// <summary>
        /// Indicates if a transaction is open  on
        /// the session.
        /// </summary>
        public bool IsTransactionStart
        {
            get { return this.isTransactionOpen; }
        }

        /// <summary>
        /// Changes the vote for transaction to commit (true) or to abort (false).
        /// </summary>
        private bool Consistent
        {
            set { consistent = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Complete (commit) a transaction
        /// </summary>
        /// <remarks>
        /// Use in 'using' syntax.
        /// </remarks>
        public void Complete()
        {
            this.Consistent = true;
        }

        /// <summary>
        /// Open the connection
        /// </summary>
        public void OpenConnection()
        {
            this.OpenConnection(this.dataSource.ConnectionString);
        }

        /// <summary>
        /// Open a connection, on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public void OpenConnection(string connectionString)
        {
            if (this.connection == null)
            {
                this.CreateConnection(connectionString);
                try
                {
                    this.connection.Open();
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", connection.GetHashCode().ToString(), this.dataSource.DbProvider.Description));
                    }
                }
                catch (Exception ex)
                {
                    throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this.dataSource.DbProvider.Description), ex);
                }
            }
            else if (connection.State != ConnectionState.Open)
            {
                try
                {
                    this.connection.Open();
                    if (logger.IsDebugEnabled)
                    {
                        logger.Debug(string.Format("Open Connection \"{0}\" to \"{1}\".", connection.GetHashCode().ToString(), this.dataSource.DbProvider.Description));
                    }
                }
                catch (Exception ex)
                {
                    throw new DataMapperException(string.Format("Unable to open connection to \"{0}\".", this.dataSource.DbProvider.Description), ex);
                }
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void CloseConnection()
        {
            if ((this.connection != null) && (this.connection.State != ConnectionState.Closed))
            {
                this.connection.Close();
                if (logger.IsDebugEnabled)
                {

                    logger.Debug(string.Format("Close Connection \"{0}\" to \"{1}\".", this.connection.GetHashCode().ToString(), this.dataSource.DbProvider.Description));
                }
                this.connection.Dispose();
            }
            this.connection = null;
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.BeginTransaction(this.dataSource.ConnectionString);
        }

        /// <summary>
        /// Open a connection and begin a transaction on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public void BeginTransaction(string connectionString)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                this.OpenConnection(connectionString);
            }
            this.transaction = connection.BeginTransaction();
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Begin Transaction.");
            }
            this.isTransactionOpen = true;
        }

        /// <summary>
        /// Begins a database transaction
        /// </summary>
        /// <param name="openConnection">Open a connection.</param>
        public void BeginTransaction(bool openConnection)
        {
            if (openConnection)
            {
                this.BeginTransaction();
            }
            else
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    this.OpenConnection();
                }
                transaction = connection.BeginTransaction();
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Begin Transaction.");
                }
                this.isTransactionOpen = true;
            }
        }

        /// <summary>
        /// Begins a database transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level under which the transaction should run.
        /// </param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            this.BeginTransaction(this.dataSource.ConnectionString, isolationLevel);
        }

        /// <summary>
        /// Open a connection and begin a transaction on the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="isolationLevel">The transaction isolation level for this connection.</param>
        public void BeginTransaction(string connectionString, IsolationLevel isolationLevel)
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                this.OpenConnection(connectionString);
            }
            transaction = connection.BeginTransaction(isolationLevel);
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Begin Transaction.");
            }
            this.isTransactionOpen = true;
        }

        /// <summary>
        /// Begins a transaction on the current connection
        /// with the specified IsolationLevel value.
        /// </summary>
        /// <param name="isolationLevel">The transaction isolation level for this connection.</param>
        /// <param name="openConnection">Open a connection.</param>
        public void BeginTransaction(bool openConnection, IsolationLevel isolationLevel)
        {
            this.BeginTransaction(this.dataSource.ConnectionString, openConnection, isolationLevel);
        }

        /// <summary>
        /// Begins a transaction on the current connection
        /// with the specified IsolationLevel value.
        /// </summary>
        /// <param name="isolationLevel">The transaction isolation level for this connection.</param>
        /// <param name="connectionString">The connection string</param>
        /// <param name="openConnection">Open a connection.</param>
        public void BeginTransaction(string connectionString, bool openConnection, IsolationLevel isolationLevel)
        {
            if (openConnection)
            {
                this.BeginTransaction(connectionString, isolationLevel);
            }
            else
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    throw new DataMapperException("SqlMapSession could not invoke StartTransaction(). A Connection must be started. Call OpenConnection() first.");
                }
                transaction = connection.BeginTransaction(isolationLevel);
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Begin Transaction.");
                }
                this.isTransactionOpen = true;
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <remarks>
        /// Will close the connection.
        /// </remarks>
        public void CommitTransaction()
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Commit Transaction.");
            }
            transaction.Commit();
            transaction.Dispose();
            transaction = null;
            this.isTransactionOpen = false;

            if (connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        /// <param name="closeConnection">Close the connection</param>
        public void CommitTransaction(bool closeConnection)
        {
            if (closeConnection)
            {
                this.CommitTransaction();
            }
            else
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Commit Transaction.");
                }
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
                this.isTransactionOpen = false;
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
            if (logger.IsDebugEnabled)
            {
                logger.Debug("RollBack Transaction.");
            }
            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
            this.isTransactionOpen = false;
            if (connection.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        /// <param name="closeConnection">Close the connection</param>
        public void RollBackTransaction(bool closeConnection)
        {
            if (closeConnection)
            {
                this.RollBackTransaction();
            }
            else
            {
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("RollBack Transaction.");
                }
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
                this.isTransactionOpen = false;
            }
        }

        /// <summary>
        /// Create a command object
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(CommandType commandType)
        {
            IDbCommand _command = this.dataSource.DbProvider.CreateCommand();

            _command.CommandType = commandType;
            _command.Connection = this.connection;

            // Assign transaction
            if (transaction != null)
            {
                try
                {
                    _command.Transaction = transaction;
                }
                catch
                { }
            }
            // Assign connection timeout
            if (connection != null)
            {
                try // MySql provider doesn't suppport it !
                {
                    _command.CommandTimeout = connection.ConnectionTimeout;
                }
                catch (NotSupportedException e)
                {
                    if (logger.IsInfoEnabled)
                    {
                        logger.Info(e.Message);
                    }
                }
            }

            //			if (_logger.IsDebugEnabled)
            //			{
            //				command = IDbCommandProxy.NewInstance(command);
            //			}

            return _command;
        }

        /// <summary>
        /// Create an IDataParameter
        /// </summary>
        /// <returns>An IDataParameter.</returns>
        public IDbDataParameter CreateDataParameter()
        {
            return this.dataSource.DbProvider.CreateDataParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataAdapter CreateDataAdapter()
        {
            return this.dataSource.DbProvider.CreateDataAdapter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aCommand"></param>
        /// <returns></returns>
        public IDbDataAdapter CreateDataAdapter(IDbCommand aCommand)
        {
            IDbDataAdapter _dataAdapter = null;

            _dataAdapter = this.dataSource.DbProvider.CreateDataAdapter();
            _dataAdapter.SelectCommand = aCommand;

            return _dataAdapter;
        }
        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releasing, or resetting resources.
        /// </summary>
        public void Dispose()
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Dispose SqlMapSession");
            }
            if (this.isTransactionOpen == false)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    sqlMapper.CloseConnection();
                }
            }
            else
            {
                if (consistent)
                {
                    sqlMapper.CommitTransaction();
                    this.isTransactionOpen = false;
                }
                else
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        sqlMapper.RollBackTransaction();
                        this.isTransactionOpen = false;
                    }
                }
            }
        }

        #endregion
    }
}
