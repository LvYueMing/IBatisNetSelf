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
    /// no 'resultClass' attribute is specified.
    /// </summary>
    public sealed class ObjectStrategy : IResultStrategy
    {
        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when no resultClass or resultMap attribute are specified.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aReader">The reader.</param>
        /// <param name="aResultObject">The result object.</param>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {
            object _outObject = aResultObject; 

            if (aReader.FieldCount == 1)
            {
                ResultProperty _property = new ResultProperty();
                _property.PropertyName = "value";
                _property.ColumnIndex = 0;
                _property.TypeHandler = aRequest.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(aReader.GetFieldType(0));
                _outObject = _property.GetDataBaseValue(aReader);
            }
            else if (aReader.FieldCount > 1)
            {
                object[] _newOutObject = new object[aReader.FieldCount]; 
                int count = aReader.FieldCount;
                for (int i = 0; i < count; i++)
                {
                    ResultProperty _property = new ResultProperty();
                    _property.PropertyName = "value";
                    _property.ColumnIndex = i;
                    _property.TypeHandler = aRequest.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(aReader.GetFieldType(i));
                    _newOutObject[i] = _property.GetDataBaseValue(aReader);
                }

                _outObject = _newOutObject;
            }
            else
            {
                // do nothing if 0 fields
            }

            return _outObject;
        }

        #endregion
    }
}
