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
    /// a 'resultClass' attribute is specified and
    /// the type of the result object is <see cref="IList"/>.
    /// </summary>
    public sealed class ListStrategy : IResultStrategy
    {
        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when a ResultClass is specified on the statement and
        /// the ResultClass is <see cref="IList"/>.
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

            int count = reader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                ResultProperty property = new ResultProperty();
                property.PropertyName = "value";
                property.ColumnIndex = i;
                property.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(reader.GetFieldType(i));
                ((IList)outObject).Add(property.GetDataBaseValue(reader));
            }

            return outObject;
        }

        #endregion
    }
}
