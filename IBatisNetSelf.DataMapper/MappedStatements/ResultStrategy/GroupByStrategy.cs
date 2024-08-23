using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
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
    /// a 'groupBy' attribute is specified on the resultMap tag.
    /// </summary>
    /// <remarks>N+1 Select solution</remarks>
    public sealed class GroupByStrategy : BaseStrategy, IResultStrategy
    {

        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>The result object</returns>
        public object Process(RequestScope request, ref IDataReader reader, object resultObject)
        {
            object outObject = resultObject;

            IResultMap resultMap = request.CurrentResultMap.ResolveSubMap(reader);

            string uniqueKey = GetUniqueKey(resultMap, request, reader);
            // Gets the [key, result object] already build
            IDictionary buildObjects = request.GetUniqueKeys(resultMap);

            if (buildObjects != null && buildObjects.Contains(uniqueKey))
            {
                // Unique key is already known, so get the existing result object and process additional results.
                outObject = buildObjects[uniqueKey];
                // process resulMapping attribute wich point to a groupBy attribute
                for (int index = 0; index < resultMap.Properties.Count; index++)
                {
                    ResultProperty resultProperty = resultMap.Properties[index];
                    if (resultProperty.PropertyStrategy is PropertStrategy.GroupByStrategy)
                    {
                        resultProperty.PropertyStrategy.Set(request, resultMap, resultProperty, ref outObject, reader, null);
                    }
                }
                outObject = SKIP;
            }
            else if (uniqueKey == null || buildObjects == null || !buildObjects.Contains(uniqueKey))
            {
                // Unique key is NOT known, so create a new result object and process additional results.

                // Fix IBATISNET-241
                if (outObject == null)
                {
                    // temp ?, we don't support constructor tag with groupBy attribute
                    outObject = resultMap.CreateInstanceOfResult(null);
                }

                for (int index = 0; index < resultMap.Properties.Count; index++)
                {
                    ResultProperty resultProperty = resultMap.Properties[index];
                    resultProperty.PropertyStrategy.Set(request, resultMap, resultProperty, ref outObject, reader, null);
                }

                if (buildObjects == null)
                {
                    buildObjects = new Hashtable();
                    request.SetUniqueKeys(resultMap, buildObjects);
                }
                buildObjects[uniqueKey] = outObject;
            }

            return outObject;
        }

        #endregion
    }
}
