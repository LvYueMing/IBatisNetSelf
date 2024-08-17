using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.TypesResolver
{
    /// <summary>
    /// Resolves a <see cref="System.Type"/> by name.
    /// </summary>
    /// <remarks>
    /// <p>
    /// The rationale behind the creation of this class is to centralise the
    /// resolution of type names to <see cref="System.Type"/> instances beyond that
    /// offered by the plain vanilla System.Type.GetType method call.
    /// </p>
    /// </remarks>
    internal class TypeResolver : ITypeResolver
    {
        private const string NULLABLE_TYPE = "System.Nullable";

        #region ITypeResolver Members
        /// <summary>
        /// Resolves the supplied <paramref name="typeName"/> to a
        /// <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The unresolved name of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public virtual Type Resolve(string typeName)
        {
            return ResolveType(typeName.Replace(" ", string.Empty));
        }

        #endregion

        private Type ResolveType(string typeName)
        {
            // sanity check
            if (typeName == null || typeName.Trim().Length == 0)
            {
                throw BuildTypeLoadException("\"\"或null");
            }

            TypeAssemblyInfo _typeInfo = new TypeAssemblyInfo(typeName);
            Type? _type = null;
            try
            {
                _type = (_typeInfo.IsAssemblyQualified) ? LoadTypeDirectlyFromAssembly(_typeInfo) : LoadTypeByIteratingOverAllLoadedAssemblies(_typeInfo);
            }
            catch (Exception ex)
            {
                throw BuildTypeLoadException(typeName, ex);
            }
            if (_type == null)
            {
                throw BuildTypeLoadException(typeName);
            }
            return _type;

        }

        /// <summary>
        /// Uses <see cref="System.Reflection.Assembly.Load(string)"/>
        /// to load an <see cref="System.Reflection.Assembly"/> and then the attendant
        /// <see cref="System.Type"/> referred to by the <paramref name="typeInfo"/> parameter.
        /// </summary>
        /// <param name="typeInfo">
        /// The assembly and type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        private static Type? LoadTypeDirectlyFromAssembly(TypeAssemblyInfo typeInfo)
        {
            Type? _type = null;
            // assembly qualified... load the assembly, then the Type
            Assembly? _assembly = Assembly.Load(typeInfo.AssemblyName);

            if (_assembly != null)
            {
                _type = _assembly.GetType(typeInfo.TypeName, true, true);
            }
            return _type;
        }

        /// <summary>
        /// Check all assembly
        /// to load the attendant <see cref="System.Type"/> referred to by 
        /// the <paramref name="typeInfo"/> parameter.
        /// </summary>
        /// <param name="typeInfo">
        /// The type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        private static Type? LoadTypeByIteratingOverAllLoadedAssemblies(TypeAssemblyInfo typeInfo)
        {
            Type? _type = null;
            Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in _assemblies)
            {
                _type = assembly.GetType(typeInfo.TypeName, false, false);
                if (_type != null)
                {
                    break;
                }
            }
            return _type;
        }



        #region Exception
        private static TypeLoadException BuildTypeLoadException(string typeName)
        {
            return new TypeLoadException($"Could not load type from string value '{typeName}'.");
        }

        private static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
        {
            return new TypeLoadException($"Could not load type from string value '{typeName}'.", ex);
        }
        #endregion
    }
}
