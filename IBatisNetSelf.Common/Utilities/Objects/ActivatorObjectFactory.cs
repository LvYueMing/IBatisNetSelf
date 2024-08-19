using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A <see cref="IObjectFactory"/> implementation that can create objects 
    /// via Activator.CreateInstance
    /// </summary>
    public class ActivatorObjectFactory : IObjectFactory
    {

        #region IObjectFactory members

        /// <summary>
        /// Create a new see <see cref="IObjectFactory"/> instance for a given type
        /// </summary>
        /// <param name="typeToCreate">The type instance to build</param>
        /// <param name="types">The types of the constructor arguments</param>
        /// <returns>Returns a new <see cref="IObjectFactory"/> instance.</returns>
        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            return new ActivatorFactory(typeToCreate);
        }

        #endregion
    }
}
