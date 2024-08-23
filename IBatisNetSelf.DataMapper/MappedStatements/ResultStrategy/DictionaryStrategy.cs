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
    /// the type of the result object is <see cref="IDictionary"/>.
    /// </summary>
    public sealed class DictionaryStrategy : IResultStrategy
    {
        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// when a 'resultClass' attribute is specified on the statement and
        /// the 'resultClass' attribute is a <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aReader">The reader.</param>
        /// <param name="aResultObject">The result object.</param>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {
            object _outObject = aResultObject;
            AutoResultMap _resultMap = aRequest.CurrentResultMap as AutoResultMap;

            if (_outObject == null)
            {
                _outObject = _resultMap.CreateInstanceOfResultClass();
            }

            int count = aReader.FieldCount;
            IDictionary _dictionary = (IDictionary)_outObject;
            for (int i = 0; i < count; i++)
            {
                ResultProperty _property = new ResultProperty();
                _property.PropertyName = "value";
                _property.ColumnIndex = i;
                _property.TypeHandler = aRequest.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(aReader.GetFieldType(i));
                _dictionary.Add(aReader.GetName(i), _property.GetDataBaseValue(aReader));
            }

            return _outObject;
        }

        #endregion
    }
}
