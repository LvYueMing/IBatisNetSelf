using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.TypesResolver
{
    /// <summary>
    /// Resolves (instantiates) a <see cref="System.Type"/> by it's (possibly
    /// assembly qualified) name, and caches the <see cref="System.Type"/> instance against the type name.
    /// </summary>
    internal class CachedTypeResolver : ITypeResolver
    {
        #region Fields
        /// <summary>
        /// The cache, mapping type names (<see cref="System.String"/> instances) against <see cref="System.Type"/> instances.
        /// </summary>
        private IDictionary typeCache = new HybridDictionary();

        private ITypeResolver typeResolver = null;

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Creates a new instance of the <see cref="IBatisNet.Common.Utilities.TypesResolver.CachedTypeResolver"/> class.
        /// </summary>
        /// <param name="typeResolver">
        /// The <see cref="IBatisNet.Common.Utilities.TypesResolver.ITypeResolver"/> that this instance will delegate
        /// actual <see cref="System.Type"/> resolution to if a <see cref="System.Type"/>
        /// cannot be found in this instance's <see cref="System.Type"/> cache.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="typeResolver"/> is <see langword="null"/>.
        /// </exception>
        public CachedTypeResolver(ITypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
        }
        #endregion

        #region ITypeResolver Members

        /// <summary>
        /// Resolves the supplied <paramref name="aTypeName"/> to a
        /// <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="aTypeName">
        /// The (possibly partially assembly qualified) name of a
        /// <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="aTypeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public Type Resolve(string aTypeName)
        {
            if (aTypeName == null || aTypeName.Trim().Length == 0)
            {
                throw new TypeLoadException($"Could not load type from string value ' {aTypeName}'.");
            }

            Type? _type = null;
            try
            {
                _type = this.typeCache[aTypeName] as Type;
                if (_type == null)
                {
                    _type = this.typeResolver.Resolve(aTypeName);
                    this.typeCache[aTypeName] = _type;
                }
            }
            catch (Exception ex)
            {
                if (ex is TypeLoadException)
                {
                    throw;
                }
                throw new TypeLoadException($"Could not load type from string value '{aTypeName}'.", ex);
            }
            return _type;
        }


        #endregion
    }
}
