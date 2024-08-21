using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy
{
    /// <summary>
    /// <see cref="IArgumentStrategy"/> implementation when a 'resultMapping' attribute exists
    /// on a <see cref="ArgumentProperty"/>.
    /// </summary>
    public sealed class ResultMapStrategy : BaseStrategy, IArgumentStrategy
    {
        #region IArgumentStrategy Members

        /// <summary>
        /// Gets the value of an argument constructor.
        /// </summary>
        /// <param name="request">The current <see cref="RequestScope"/>.</param>
        /// <param name="mapping">The <see cref="ResultProperty"/> with the argument infos.</param>
        /// <param name="reader">The current <see cref="IDataReader"/>.</param>
        /// <param name="keys">The keys</param>
        /// <returns>The paremeter value.</returns>
        public object GetValue(RequestScope request, ResultProperty mapping,
                               ref IDataReader reader, object keys)
        {
            object[] parameters = null;
            bool isParameterFound = false;

            IResultMap resultMapping = mapping.NestedResultMap.ResolveSubMap(reader);

            if (resultMapping.Parameters.Count > 0)
            {
                parameters = new object[resultMapping.Parameters.Count];
                // Fill parameters array
                for (int index = 0; index < resultMapping.Parameters.Count; index++)
                {
                    ResultProperty property = resultMapping.Parameters[index];
                    parameters[index] = property.ArgumentStrategy.GetValue(request, property, ref reader, null);
                    request.IsRowDataFound = request.IsRowDataFound || (parameters[index] != null);
                    isParameterFound = isParameterFound || (parameters[index] != null);
                }
            }

            object obj = null;
            // If I have a constructor tag and all argumments values are null, the obj is null
            if (resultMapping.Parameters.Count > 0 && isParameterFound == false)
            {
                obj = null;
            }
            else
            {
                obj = resultMapping.CreateInstanceOfResult(parameters);
                if (FillObjectWithReaderAndResultMap(request, reader, resultMapping, ref obj) == false)
                {
                    obj = null;
                }
            }

            return obj;
        }

        #endregion
    }
}
