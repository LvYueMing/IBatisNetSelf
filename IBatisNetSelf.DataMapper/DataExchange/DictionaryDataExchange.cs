using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.DataExchange
{
    /// <summary>
    /// DataExchange implementation for IDictionary objects
    /// </summary>
    public sealed class DictionaryDataExchange : BaseDataExchange
    {

        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="dataExchangeFactory"></param>
        public DictionaryDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
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
            return ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName,
                this.DataExchangeFactory.AccessorFactory);
        }

        /// <summary>
        /// Sets the value to the result property.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        public override void SetData(ref object target, ResultProperty mapping, object dataBaseValue)
        {
            ((IDictionary)target).Add(mapping.PropertyName, dataBaseValue);
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
            ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue,
                this.DataExchangeFactory.ObjectFactory,
                this.DataExchangeFactory.AccessorFactory);
        }

        #endregion
    }
}
