using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy;
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
    /// <see cref="IResultStrategy"/> implementation when 
    /// a 'resultClass' attribute is specified and
    /// the type of the result object is primitive.
    /// </summary>
    public sealed class SimpleTypeStrategy : IResultStrategy
    {
        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when a ResultClass is specified on the statement and
        /// the ResultClass is a SimpleType.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object outObject = resultObject;
            AutoResultMap resultMap = request.CurrentResultMap as AutoResultMap;

            if (outObject == null)
            {
                outObject = resultMap.CreateInstanceOfResultClass();
            }

            if (!resultMap.IsInitalized)
            {
                lock (resultMap)
                {
                    if (!resultMap.IsInitalized)
                    {
                        // Create a ResultProperty
                        ResultProperty property = new ResultProperty();
                        property.PropertyName = "value";
                        property.ColumnIndex = 0;
                        property.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(outObject.GetType());
                        property.PropertyStrategy = PropertyStrategyFactory.Get(property);

                        resultMap.Properties.Add(property);
                        resultMap.DataExchange = request.DataExchangeFactory.GetDataExchangeForClass(typeof(int));// set the PrimitiveDataExchange
                        resultMap.IsInitalized = true;
                    }
                }
            }

            resultMap.Properties[0].PropertyStrategy.Set(request, resultMap, resultMap.Properties[0], ref outObject, reader, null);

            return outObject;
        }

        #endregion
    }
}
