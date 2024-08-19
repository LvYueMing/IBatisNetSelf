using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="IAccessor"/> interface defines a field/property contract.
    /// </summary>
    public interface IAccessor
    {
        /// <summary>
        /// Gets the member name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of this member (field or property).
        /// </summary>
        Type MemberType { get; }
    }
}
