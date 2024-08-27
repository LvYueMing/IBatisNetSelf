using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.DataExchange
{
    /// <summary>
    /// DataExchange implementation for "primitive" objects.
    /// </summary>
    /// <remarks>
    /// The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32,
    /// UInt32, Int64, UInt64, Char, Double, and Single + string, Guid, Decimal, DateTime
    /// </remarks>
    public sealed class PrimitiveDataExchange : BaseDataExchange
    {
        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="dataExchangeFactory"></param>
        public PrimitiveDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
        {
        }

        #region IDataExchange Members

        /// <summary>
        /// Gets the data to be set into a IDataParameter.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="parameterObject"></param>
        public override object GetData(ParameterProperty mapping, object parameterObject)
        {
            if (mapping.IsComplexMemberName)
            {
                return ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, this.DataExchangeFactory.AccessorFactory);
            }
            else
            {
                return parameterObject;
            }
        }

        /// <summary>
        /// Sets the value to the result property.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
        {
            target = dataBaseValue;
        }

        /// <summary>
        /// Sets the value to the parameter property.
        /// </summary>
        /// <remarks>Use to set value on output parameter</remarks>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        public override void SetData(ref object target, ParameterProperty mapping, object dataBaseValue)
        {
            target = dataBaseValue;
        }

        #endregion
    }
}
