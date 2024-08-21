using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy
{
    /// <summary>
    /// <see cref="IArgumentStrategy"/> implementation when no 'select' or
    /// 'resultMapping' attribute exists on a <see cref="ResultProperty"/>.
    /// </summary>
    public sealed class DefaultStrategy : IArgumentStrategy
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
            if (mapping.TypeHandler == null ||
                mapping.TypeHandler is UnknownTypeHandler) // Find the TypeHandler
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
