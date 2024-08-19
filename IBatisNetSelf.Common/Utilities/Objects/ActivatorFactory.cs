using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// Create objects via Activator.CreateInstance
    /// </summary>
    public sealed class ActivatorFactory : IFactory
    {
        private Type typeToCreate = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aTypeToCreate"></param>
        public ActivatorFactory(Type aTypeToCreate)
        {
            this.typeToCreate = aTypeToCreate;
        }

        #region IFactory members

        /// <summary>
        /// Create a new instance with the specified parameters
        /// </summary>
        /// <param name="parameters">
        /// An array of values that matches the number, order and type 
        /// of the parameters for this constructor. 
        /// </param>
        /// <remarks>
        /// If you call a constructor with no parameters, pass null. 
        /// Anyway, what you pass will be ignore.
        /// </remarks>
        /// <returns>A new instance</returns>
        public object CreateInstance(object[] parameters)
        {
            return Activator.CreateInstance(this.typeToCreate, parameters);
        }

        #endregion
    }
}
