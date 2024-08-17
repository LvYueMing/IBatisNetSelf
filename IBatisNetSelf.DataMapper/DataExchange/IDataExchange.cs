using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.DataExchange
{
    /// <summary>
    /// Interface for exchanging data between a parameter map/result map and the related objects
    /// </summary>
    public interface IDataExchange
    {
        /// <summary>
        /// Gets the data to be set into a IDataParameter.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="parameterObject"></param>
        object GetData(ParameterProperty mapping, object parameterObject);

        /// <summary>
        /// Sets the value to the result property.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        void SetData(ref object target, ResultProperty mapping, object dataBaseValue);

        /// <summary>
        /// Sets the value to the parameter property.
        /// </summary>
        /// <remarks>Use to set value on output parameter</remarks>
        /// <param name="mapping"></param>
        /// <param name="target"></param>
        /// <param name="dataBaseValue"></param>
        void SetData(ref object target, ParameterProperty mapping, object dataBaseValue);
    }
}
