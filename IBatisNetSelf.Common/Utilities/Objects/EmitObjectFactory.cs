using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A <see cref="IObjectFactory"/> implementation that can create objects via IL code
    /// </summary>
    public sealed class EmitObjectFactory : IObjectFactory
    {
        private IDictionary cachedFactories = new HybridDictionary();
        private FactoryBuilder factoryBuilder = null;
        private object padlock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitObjectFactory"/> class.
        /// </summary>
        public EmitObjectFactory()
        {
            this.factoryBuilder = new FactoryBuilder();
        }

        #region IObjectFactory members

        /// <summary>
        /// Create a new <see cref="IFactory"/> instance for a given type
        /// </summary>
        /// <param name="aTypeToCreate">The type instance to build</param>
        /// <param name="aTypes">The types of the constructor arguments</param>
        /// <returns>Returns a new <see cref="IFactory"/> instance.</returns>
        public IFactory CreateFactory(Type aTypeToCreate, Type[] aTypes)
        {
            string _key = GenerateKey(aTypeToCreate, aTypes);

            IFactory _factory = this.cachedFactories[_key] as IFactory;
            if (_factory == null)
            {
                lock (padlock)
                {
                    _factory = this.cachedFactories[_key] as IFactory;
                    if (_factory == null) // double-check
                    {
                        _factory = this.factoryBuilder.CreateFactory(aTypeToCreate, aTypes);
                        this.cachedFactories[_key] = _factory;
                    }
                }
            }
            return _factory;
        }

        /// <summary>
        /// Generates the key for a cache entry.
        /// </summary>
        /// <param name="aTypeToCreate">The type instance to build.</param>
        /// <param name="aArguments">The types of the constructor arguments</param>
        /// <returns>The key for a cache entry.</returns>
        private string GenerateKey(Type aTypeToCreate, object[] aArguments)
        {
            StringBuilder _cacheKey = new StringBuilder();
            _cacheKey.Append(aTypeToCreate.ToString());
            _cacheKey.Append(".");
            if ((aArguments != null) && (aArguments.Length != 0))
            {
                for (int i = 0; i < aArguments.Length; i++)
                {
                    _cacheKey.Append(".").Append(aArguments[i]);
                }
            }
            return _cacheKey.ToString();
        }
        #endregion
    }
}
