using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="ISetAccessor"/> interface defines a field/property set accessor.
    /// provides <c>Reflection.Emit</c>-generated <see cref="ISet"/> 
    /// implementations for drastically improved performance over default late-bind 
    /// invoke.
    /// </summary>
    public interface ISetAccessor : IAccessor, ISet
    {
    }
}
