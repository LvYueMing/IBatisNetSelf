using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// A ParameterSetter implementation
    /// </summary>
    public sealed class ParameterSetterImpl : IParameterSetter
    {
        #region Fields

        private IDataParameter _dataParameter = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="dataParameter"></param>
        public ParameterSetterImpl(IDataParameter dataParameter)
        {
            _dataParameter = dataParameter;
        }
        #endregion

        #region IParameterSetter members

        /// <summary>
        /// Returns the underlying DataParameter
        /// </summary>
        public IDataParameter DataParameter
        {
            get { return _dataParameter; }
        }

        /// <summary>
        /// Set the parameter value
        /// </summary>
        public object Value
        {
            set { _dataParameter.Value = value; }
        }

        #endregion
    }
}
