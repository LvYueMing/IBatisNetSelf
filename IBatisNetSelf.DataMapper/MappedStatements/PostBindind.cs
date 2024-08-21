using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// All datas for retrieve 'select' <see cref="ResultProperty"/>
    /// </summary>
    /// <remarks>
    /// As ADO.NET allows one open <see cref="IDataReader"/> per connection at once, we keep
    /// all the datas to open the next <see cref="IDataReader"/> after having closed the current. 
    /// </remarks>
    public sealed class PostBindind
    {
        /// <summary>
        /// Enumeration of the ExecuteQuery method.
        /// </summary>
        public enum ExecuteMethod : int
        {
            /// <summary>
            /// Execute Query For Object
            /// </summary>
            ExecuteQueryForObject = 1,
            /// <summary>
            /// Execute Query For IList
            /// </summary>
            ExecuteQueryForIList,
            /// <summary>
            /// Execute Query For Generic IList
            /// </summary>
            ExecuteQueryForGenericIList,
            /// <summary>
            /// Execute Query For Array List
            /// </summary>
            ExecuteQueryForArrayList,
            /// <summary>
            /// Execute Query For Strong Typed IList
            /// </summary>
            ExecuteQueryForStrongTypedIList
        }

        #region Fields
        private IMappedStatement statement = null;
        private ResultProperty property = null;
        private object target = null;
        private object keys = null;
        private ExecuteMethod method = ExecuteMethod.ExecuteQueryForIList;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the statement.
        /// </summary>
        /// <value>The statement.</value>
        public IMappedStatement Statement
        {
            set { statement = value; }
            get { return statement; }
        }


        /// <summary>
        /// Gets or sets the result property.
        /// </summary>
        /// <value>The result property.</value>
        public ResultProperty ResultProperty
        {
            set { property = value; }
            get { return property; }
        }


        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target
        {
            set { target = value; }
            get { return target; }
        }



        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public object Keys
        {
            set { keys = value; }
            get { return keys; }
        }


        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public ExecuteMethod Method
        {
            set { method = value; }
            get { return method; }
        }
        #endregion
    }
}
