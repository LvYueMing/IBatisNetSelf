using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// <see cref="IResultStrategy"/> implementation when 
    /// a 'resultClass' attribute is specified.
    /// </summary>
    public sealed class ResultClassStrategy : IResultStrategy
    {
        private static IResultStrategy simpleTypeStrategy = null;
        private static IResultStrategy dictionaryStrategy = null;
        private static IResultStrategy listStrategy = null;
        private static IResultStrategy autoMapStrategy = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultClassStrategy"/> class.
        /// </summary>
        public ResultClassStrategy()
        {
            simpleTypeStrategy = new SimpleTypeStrategy();
            dictionaryStrategy = new DictionaryStrategy();
            listStrategy = new ListStrategy();
            autoMapStrategy = new AutoMapStrategy();
        }

        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aReader">The reader.</param>
        /// <param name="aResultObject">The result object.</param>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {

            // Check if the ResultClass is a 'primitive' Type
            if (aRequest.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(aRequest.CurrentResultMap.Class))
            {
                return simpleTypeStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            else if (typeof(IDictionary).IsAssignableFrom(aRequest.CurrentResultMap.Class))
            {
                return dictionaryStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            else if (typeof(IList).IsAssignableFrom(aRequest.CurrentResultMap.Class))
            {
                return listStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            else
            {
                return autoMapStrategy.Process(aRequest, ref aReader, aResultObject);
            }
        }

        #endregion
    }
}
