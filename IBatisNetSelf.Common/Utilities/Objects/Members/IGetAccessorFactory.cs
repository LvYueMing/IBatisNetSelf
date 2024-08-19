using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// Factory contact to build <see cref="IGetAccessor"/> for a type.
    /// </summary>
    public interface IGetAccessorFactory
    {
        /// <summary>
        /// Generate an <see cref="IGetAccessor"/> instance.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="name">Field or Property name.</param>
        /// <returns>null if the generation fail</returns>
        IGetAccessor CreateGetAccessor(Type targetType, string name);
    }
}
