using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// Factory contact to build <see cref="ISetAccessor"/> for a type.
    /// </summary>
    public interface ISetAccessorFactory
    {
        /// <summary>
        /// Generate an <see cref="ISetAccessor"/> instance.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="name">Field or Property name.</param>
        /// <returns>null if the generation fail</returns>
        ISetAccessor CreateSetAccessor(Type targetType, string name);
    }
}
