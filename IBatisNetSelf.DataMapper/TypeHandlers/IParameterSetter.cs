using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// Allows parameters to be set on the underlying prepared IDbCommand.
    /// TypeHandlerCallback implementations use this interface to
    /// process values before they are set on the IDbCommand.
    /// </summary>
    /// <remarks>
    /// There is no need to implement this.  The implementation
    /// will be passed into the TypeHandlerCallback automatically.
    /// </remarks>
    public interface IParameterSetter
    {

        /// <summary>
        /// Returns the underlying IDataParameter
        /// </summary>
        IDataParameter DataParameter { get; }

        /// <summary>
        /// Get the parameter value
        /// </summary>
        object Value { set; }
    }
}
