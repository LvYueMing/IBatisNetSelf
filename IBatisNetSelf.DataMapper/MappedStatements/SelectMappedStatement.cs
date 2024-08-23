using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// Summary description for SelectMappedStatement.
    /// </summary>
    public sealed class SelectMappedStatement : MappedStatement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sqlMap">An SqlMap</param>
        /// <param name="statement">An SQL statement</param>
        internal SelectMappedStatement(ISqlMapper sqlMap, IStatement statement)
            : base(sqlMap, statement)
        { }


        #region ExecuteInsert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public override object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Update statements cannot be executed as a query insert.");
        }

        #endregion

        #region ExecuteUpdate

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public override int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            throw new DataMapperException("Insert statements cannot be executed as a update query.");
        }

        #endregion
    }
}
