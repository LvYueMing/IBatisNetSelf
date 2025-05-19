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
    /// <see cref="IResultStrategy"/> implementation when 
    /// a 'resultMap' attribute is specified.
    /// </summary>
    public sealed class ResultMapStrategy : BaseStrategy, IResultStrategy
    {
        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when a ResultMap is specified on the statement.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object outObject = resultObject;

            IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);

            if (outObject == null)
            {
                object[] _constructorParams = null;
                if (resultMap.ConstructorParams.Count > 0)
                {
                    _constructorParams = new object[resultMap.ConstructorParams.Count];
                    // Fill parameters array
                    for (int index = 0; index < resultMap.ConstructorParams.Count; index++)
                    {
                        ResultProperty resultProperty = resultMap.ConstructorParams[index];
                        _constructorParams[index] = resultProperty.ArgumentStrategy.GetValue(request, resultProperty, ref reader, null);
                    }
                }

                outObject = resultMap.CreateInstanceOfResult(_constructorParams);
            }

            // For each Property in the ResultMap, set the property in the object 
            for (int index = 0; index < resultMap.Properties.Count; index++)
            {
                ResultProperty property = resultMap.Properties[index];
                property.PropertyStrategy.Set(request, resultMap, property, ref outObject, reader, null);
            }

            return outObject;
        }

        #endregion
    }
}
