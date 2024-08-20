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
    /// Summary description for BaseDataExchange.
    /// </summary>
    public abstract class BaseDataExchange : IDataExchange
    {
        private DataExchangeFactory dataExchangeFactory = null;

        /// <summary>
        /// Getter for the factory that created this object
        /// </summary>
        public DataExchangeFactory DataExchangeFactory => dataExchangeFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExchangeFactory"></param>
        public BaseDataExchange(DataExchangeFactory dataExchangeFactory)
        {
            this.dataExchangeFactory = dataExchangeFactory;
        }

        #region IDataExchange Members

        /// <summary>
        /// Gets the data to be set into a IDataParameter.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="parameterObject"></param>
        public abstract object GetData(ParameterProperty mapping, object parameterObject);

        /// <summary>
        /// Sets the value to the result property.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        public abstract void SetData(ref object target, ResultProperty mapping, object dataBaseValue);

        /// <summary>
        /// Sets the value to the parameter property.
        /// </summary>
        /// <remarks>Use to set value on output parameter</remarks>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        public abstract void SetData(ref object target, ParameterProperty mapping, object dataBaseValue);

        #endregion
    }
}
