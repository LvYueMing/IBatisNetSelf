using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// <see cref="IPropertyStrategy"/> implementation when no 'select' or
    /// 'resultMapping' attribute exists on a <see cref="ResultProperty"/>.
    /// </summary>
    public sealed class DefaultStrategy : IPropertyStrategy
    {
        #region IPropertyStrategy members

        ///<summary>
        /// Sets value of the specified <see cref="ResultProperty"/> on the target object
        /// when the 'select' and 'resultMap' attributes 
        /// on the <see cref="ResultProperty"/> are empties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The ResultProperty.</param>
        /// <param name="target">The target.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="keys">The keys</param>
        public void Set(RequestScope request, IResultMap resultMap,
            ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            object obj = Get(request, resultMap, mapping, ref target, reader);
            resultMap.SetValueOfProperty(ref target, mapping, obj);
        }

        /// <summary>
        /// Gets the value of the specified <see cref="ResultProperty"/> that must be set on the target object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="target">The target object</param>
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            if (mapping.TypeHandler == null || mapping.TypeHandler is UnknownTypeHandler) // Find the TypeHandler
            {
                lock (mapping)
                {
                    if (mapping.TypeHandler == null || mapping.TypeHandler is UnknownTypeHandler)
                    {
                        int columnIndex = 0;
                        if (mapping.ColumnIndex == ResultProperty.UNKNOWN_COLUMN_INDEX)
                        {
                            columnIndex = reader.GetOrdinal(mapping.ColumnName);
                        }
                        else
                        {
                            columnIndex = mapping.ColumnIndex;
                        }
                        Type systemType = ((IDataRecord)reader).GetFieldType(columnIndex);

                        mapping.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(systemType);
                    }
                }
            }

            object dataBaseValue = mapping.GetDataBaseValue(reader);
            request.IsRowDataFound = request.IsRowDataFound || (dataBaseValue != null);
            return dataBaseValue;
        }

        #endregion
    }
}
