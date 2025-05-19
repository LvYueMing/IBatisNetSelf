using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A factory that can create objects 
    /// </summary>
    public interface IObjectFactory
    {
        /// <summary>
        /// Create a new <see cref="IFactory"/> instance for a given type
        /// </summary>
        /// <param name="typeToCreate">The type instance to build</param>
        /// <param name="types">The types of the constructor arguments</param>
        /// <returns>Returns a new see cref="IFactory"/> instance</returns>
        IFactory CreateFactory(Type typeToCreate, Type[] types);
    }
}
