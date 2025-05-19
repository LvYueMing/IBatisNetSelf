using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Scope
{
    /// <summary>
    /// Hold data during the process of a mapped statement.
    /// 在Mapped Statement的处理过程中持有数据。这里的“持有数据”指的是在执行一个SQL映射语句（即Mapped Statement）的过程中，保持或维持某些状态或数据
    /// </summary>
    public class RequestScope : IScope
    {
        #region Fields

        private ErrorContext errorContext = null;

        private IStatement statement = null;
        private ParameterMap parameterMap = null;
        private PreparedStatement preparedStatement = null;
        private IDbCommand command = null;
        private Queue selects = new Queue();
        bool rowDataFound = false;
        private static long nextId = 0;
        private long id = 0;
        private DataExchangeFactory dataExchangeFactory = null;
        private ISqlMapSession session = null;
        private IMappedStatement mappedStatement = null;
        private int currentResultMapIndex = -1;
        // Used by N+1 Select solution
        // Holds [IResultMap, IDictionary] couple where the IDictionary holds [key, result object]
        private IDictionary uniqueKeys = null;

        #endregion

        #region Properties

        /// <summary>
        ///  The current <see cref="IMappedStatement"/>.
        /// </summary>
        public IMappedStatement MappedStatement
        {
            set { mappedStatement = value; }
            get { return mappedStatement; }
        }

        /// <summary>
        /// Gets the current <see cref="IStatement"/>.
        /// </summary>
        /// <value>The statement.</value>
        public IStatement Statement
        {
            get { return statement; }
        }

        /// <summary>
        ///  The current <see cref="ISqlMapSession"/>.
        /// </summary>
        public ISqlMapSession Session
        {
            get { return session; }
        }

        /// <summary>
        ///  The <see cref="IDbCommand"/> to execute
        /// </summary>
        public IDbCommand IDbCommand
        {
            set { command = value; }
            get { return command; }
        }

        /// <summary>
        ///  Indicate if the statement have find data
        /// </summary>
        public bool IsRowDataFound
        {
            set { rowDataFound = value; }
            get { return rowDataFound; }
        }

        /// <summary>
        /// The 'select' result property to process after having process the main properties.
        /// </summary>
        public Queue QueueSelect
        {
            get { return selects; }
            set { selects = value; }
        }

        /// <summary>
        /// The current <see cref="IResultMap"/> used by this request.
        /// </summary>
        public IResultMap CurrentResultMap
        {
            get { return statement.ResultsMap[currentResultMapIndex]; }
        }

        /// <summary>
        /// Moves to the next result map.
        /// </summary>
        /// <returns></returns>
        public bool MoveNextResultMap()
        {
            if (currentResultMapIndex < statement.ResultsMap.Count - 1)
            {
                currentResultMapIndex++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// The <see cref="ParameterMap"/> used by this request.
        /// </summary>
        public ParameterMap ParameterMap
        {
            set { parameterMap = value; }
            get { return parameterMap; }
        }

        /// <summary>
        /// The <see cref="PreparedStatement"/> used by this request.
        /// </summary>
        public PreparedStatement PreparedStatement
        {
            get { return preparedStatement; }
            set { preparedStatement = value; }
        }


        #endregion

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScope"/> class.
        /// </summary>
        /// <param name="aDataExchangeFactory">The data exchange factory.</param>
        /// <param name="aSession">The session.</param>
        /// <param name="aStatement">The statement</param>
        public RequestScope(DataExchangeFactory aDataExchangeFactory, ISqlMapSession aSession, IStatement aStatement)
        {
            this.errorContext = new ErrorContext();

            this.statement = aStatement;
            this.parameterMap = aStatement.ParameterMap;
            this.session = aSession;
            this.dataExchangeFactory = aDataExchangeFactory;
            this.id = GetNextId();
        }
        #endregion

        #region Method

        /// <summary>
        /// Gets the unique keys.
        /// </summary>
        /// <param name="map">The ResultMap.</param>
        /// <returns>
        /// Returns [key, result object] which holds the result objects that have  
        /// already been build during this request with this <see cref="IResultMap"/>
        /// </returns>
        public IDictionary GetUniqueKeys(IResultMap map)
        {
            if (uniqueKeys == null)
            {
                return null;
            }
            return (IDictionary)uniqueKeys[map];
        }

        /// <summary>
        /// Sets the unique keys.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="keys">The keys.</param>
        public void SetUniqueKeys(IResultMap map, IDictionary keys)
        {
            if (uniqueKeys == null)
            {
                uniqueKeys = new Hashtable();
            }
            uniqueKeys.Add(map, keys);
        }

        /// <summary>
        /// Check if the specify object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) { return true; }
            if (!(obj is RequestScope)) { return false; }

            RequestScope scope = (RequestScope)obj;

            if (id != scope.id) return false;

            return true;
        }

        /// <summary>
        /// Get the HashCode for this RequestScope
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)(id ^ (id >> 32));
        }

        /// <summary>
        /// Method to get a unique ID
        /// </summary>
        /// <returns>The new ID</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static long GetNextId()
        {
            return nextId++;
        }
        #endregion

        #region IScope Members

        /// <summary>
        /// A factory for DataExchange objects
        /// </summary>
        public DataExchangeFactory DataExchangeFactory
        {
            get { return dataExchangeFactory; }
        }

        /// <summary>
        ///  Get the request's error context
        /// </summary>
        public ErrorContext ErrorContext
        {
            get { return errorContext; }
        }
        #endregion
    }
}
