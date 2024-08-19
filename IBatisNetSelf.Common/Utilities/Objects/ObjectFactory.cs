using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A factory to create objects 
    /// </summary>
    public class ObjectFactory : IObjectFactory
    {
        private IObjectFactory objectFactory = null;
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="allowCodeGeneration"></param>
        public ObjectFactory(bool allowCodeGeneration)
        {
            if (allowCodeGeneration)
            {
                this.objectFactory = new EmitObjectFactory();
            }
            else
            {
                this.objectFactory = new ActivatorObjectFactory();
            }
        }

        #region IObjectFactory members

        /// <summary>
        /// Create a new factory instance for a given type
        /// </summary>
        /// <param name="typeToCreate">The type instance to build</param>
        /// <param name="types">The types of the constructor arguments</param>
        /// <returns>Returns a new instance factory</returns>
        public IFactory CreateFactory(Type typeToCreate, Type[] types)
        {
            if (logger.IsDebugEnabled)
            {
                return new FactoryLogAdapter(typeToCreate, types, this.objectFactory.CreateFactory(typeToCreate, types));
            }
            else
            {
                return this.objectFactory.CreateFactory(typeToCreate, types);
            }
        }

        #endregion
    }
}
