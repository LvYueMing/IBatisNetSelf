using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// Delegates on the <see cref="ResultMapStrategy"/>  or on the 
    /// <see cref="GroupByStrategy"/> implementation if a grouBy attribute is specify on the resultMap tag.
    /// </summary>
    public sealed class MapStrategy : IResultStrategy
    {
        private static IResultStrategy _resultMapStrategy = null;
        private static IResultStrategy _groupByStrategy = null;

        /// <summary>
        /// Initializes the <see cref="MapStrategy"/> class.
        /// </summary>
        static MapStrategy()
        {
            _resultMapStrategy = new ResultMapStrategy();
            _groupByStrategy = new GroupByStrategy();
        }

        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);

            if (resultMap.GroupByPropertyNames.Count > 0)
            {
                return _groupByStrategy.Process(request, ref reader, resultObject);
            }
            else
            {
                return _resultMapStrategy.Process(request, ref reader, resultObject);
            }
        }

        #endregion
    }
}
