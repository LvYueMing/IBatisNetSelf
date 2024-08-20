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
    /// A IDataExchange implemtation for working with .NET object
    /// </summary>
    public sealed class ComplexDataExchange : BaseDataExchange
    {

        /// <summary>
        /// Cosntructor
        /// </summary>
        /// <param name="dataExchangeFactory"></param>
        public ComplexDataExchange(DataExchangeFactory dataExchangeFactory) : base(dataExchangeFactory)
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
            if (parameterObject != null)
            {
                if (this.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(parameterObject.GetType()))
                {
                    return parameterObject;
                }
                else
                {
                    return ObjectProbe.GetMemberValue(parameterObject, mapping.PropertyName, this.DataExchangeFactory.AccessorFactory);
                }
            }
            else
            {
                return null;
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
            ObjectProbe.SetMemberValue(target, mapping.PropertyName, dataBaseValue,
                this.DataExchangeFactory.ObjectFactory,
                this.DataExchangeFactory.AccessorFactory);
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
