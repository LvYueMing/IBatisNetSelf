using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A <see cref="IObjectFactory"/> implementation that for abstract type
    /// </summary>
    public class AbstractFactory : IFactory
    {
        private Type typeToCreate = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFactory"/> class.
        /// </summary>
        /// <param name="typeToCreate">The type to create.</param>
        public AbstractFactory(Type typeToCreate)
        {
            this.typeToCreate = typeToCreate;
        }

        #region IFactory Members

        /// <summary>
        /// Create a new instance with the specified parameters
        /// </summary>
        /// <param name="parameters">An array of values that matches the number, order and type
        /// of the parameters for this constructor.</param>
        /// <returns>A new instance</returns>
        /// <remarks>
        /// If you call a constructor with no parameters, pass null.
        /// Anyway, what you pass will be ignore.
        /// </remarks>
        public object CreateInstance(object[] parameters)
        {
            throw new ProbeException(
                $"Unable to optimize create instance. Cause : Could not find public constructor on the abstract type \"{typeToCreate.Name}\".");
        }

        #endregion
    }
}
