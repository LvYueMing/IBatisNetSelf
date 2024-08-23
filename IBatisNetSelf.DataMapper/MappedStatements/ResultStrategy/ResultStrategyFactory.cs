using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// Factory to get <see cref="IResultStrategy"/> implementation.
    /// </summary>
    public sealed class ResultStrategyFactory
    {
        private static IResultStrategy resultClassStrategy = null;
        private static IResultStrategy mapStrategy = null;
        private static IResultStrategy objectStrategy = null;

        /// <summary>
        /// Initializes the <see cref="ResultStrategyFactory"/> class.
        /// </summary>
        static ResultStrategyFactory()
        {
            mapStrategy = new MapStrategy();
            resultClassStrategy = new ResultClassStrategy();
            objectStrategy = new ObjectStrategy();
        }

        /// <summary>
        /// Finds the <see cref="IResultStrategy"/>.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>The <see cref="IResultStrategy"/></returns>
        public static IResultStrategy Get(IStatement statement)
        {
            // If there's an IResultMap, use it
            if (statement.ResultsMap.Count > 0)
            {
                if (statement.ResultsMap[0] is ResultMap)
                {
                    return mapStrategy;
                }
                else // it is an AutoResultMap
                {
                    return resultClassStrategy;
                }
            }
            else
            {
                return objectStrategy;
            }
        }
    }
}
