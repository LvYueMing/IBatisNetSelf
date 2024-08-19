using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// Allows values to be retrieved from the underlying IDataReader.
    /// TypeHandlerCallback implementations use this interface to
    /// get values that they can subsequently manipulate before
    /// having them returned.  																																																														   * or index with these methods.
    /// </summary>
    /// <remarks>
    /// There is no need to implement this.  The implementation
    /// will be passed into the TypeHandlerCallback automatically.
    /// </remarks>
    public interface IResultGetter
    {

        /// <summary>
        /// Returns the underlying IDataReader
        /// </summary>
        IDataReader DataReader { get; }

        /// <summary>
        /// Get the parameter value
        /// </summary>
        object Value { get; }
    }
}
