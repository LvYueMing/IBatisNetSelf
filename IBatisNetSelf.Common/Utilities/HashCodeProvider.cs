using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// Summary description for HashCodeProvider.
    /// </summary>
    public sealed class HashCodeProvider
    {
        /// <summary>
        /// Supplies a hash code for an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A hash code</returns>
        /// <remarks>
        /// </remarks>
        public static int GetIdentityHashCode(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
