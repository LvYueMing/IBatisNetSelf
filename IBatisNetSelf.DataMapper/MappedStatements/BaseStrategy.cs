using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// BaseStrategy.
    /// </summary>
    public abstract class BaseStrategy
    {
        /// <summary>
        /// Used by N+1 Select solution
        /// </summary>
        public static object SKIP = new object();

        private const string KEY_SEPARATOR = "\002";


        /// <summary>
        /// Calculte a unique key which identify the resukt object build by this <see cref="IResultMap"/>
        /// </summary>
        /// <param name="resultMap"></param>
        /// <param name="request"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected string GetUniqueKey(IResultMap resultMap, RequestScope request, IDataReader reader)
        {
            if (resultMap.GroupByProperties.Count > 0)
            {
                StringBuilder keyBuffer = new StringBuilder();

                for (int i = 0; i < resultMap.GroupByProperties.Count; i++)
                {
                    ResultProperty resultProperty = resultMap.GroupByProperties[i];
                    keyBuffer.Append(resultProperty.GetDataBaseValue(reader));
                    keyBuffer.Append('-');
                }

                if (keyBuffer.Length < 1)
                {
                    // we should never go here
                    return null;
                }
                else
                {
                    // separator value not likely to appear in a database
                    keyBuffer.Append(KEY_SEPARATOR);
                    return keyBuffer.ToString();
                }
            }
            else
            {
                // we should never go here
                return null;
            }
        }

        /// <summary>
        /// Fills the object with reader and result map.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="resultObject">The result object.</param>
        /// <returns>Indicates if we have found a row.</returns>
        protected bool FillObjectWithReaderAndResultMap(RequestScope request, IDataReader reader,
                                                        IResultMap resultMap, ref object resultObject)
        {
            bool dataFound = false;

            if (resultMap.Properties.Count > 0)
            {
                // For each Property in the ResultMap, set the property in the object 
                for (int index = 0; index < resultMap.Properties.Count; index++)
                {
                    request.IsRowDataFound = false;
                    ResultProperty property = resultMap.Properties[index];
                    property.PropertyStrategy.Set(request, resultMap, property, ref resultObject, reader, null);
                    dataFound = dataFound || request.IsRowDataFound;
                }

                request.IsRowDataFound = dataFound;
                return dataFound;
            }
            else
            {
                return true;
            }
        }

    }
}
