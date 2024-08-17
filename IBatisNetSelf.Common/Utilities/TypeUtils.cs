using IBatisNetSelf.Common.Utilities.TypesResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    public sealed class TypeUtils
    {
        #region Fields

        private static readonly ITypeResolver internalTypeResolver = new CachedTypeResolver(new TypeResolver());

        #endregion

        #region Constructor(s) / Destructor

        /// <summary>
        /// Creates a new instance of the <see cref="IBatisNet.Common.Utilities.TypeUtils"/> class.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This is a utility class, and as such exposes no public constructors.
        /// </p>
        /// </remarks>
        private TypeUtils()
        {
        }

        #endregion

        /// <summary>
        /// Resolves the supplied type name into a <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="typeName">
        /// The (possibly partially assembly qualified) name of a
        /// <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the type cannot be resolved.
        /// </exception>
        public static Type ResolveType(string typeName)
        {
            //resolve alias
            Type? _type = TypeRegistry.ResolveType(typeName);
            if (_type == null)
            {
                _type = internalTypeResolver.Resolve(typeName);
            }
            return _type;
        }

        /// <summary>
        /// Instantiate a 'Primitive' Type.
        /// </summary>
        /// <param name="typeCode">a typeCode.</param>
        /// <returns>An object.</returns>
        public static object? InstantiatePrimitiveType(TypeCode typeCode)
        {
            object resultObject = null;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    resultObject = new Boolean();
                    break;
                case TypeCode.Byte:
                    resultObject = new Byte();
                    break;
                case TypeCode.Char:
                    resultObject = new Char();
                    break;
                case TypeCode.DateTime:
                    resultObject = new DateTime();
                    break;
                case TypeCode.Decimal:
                    resultObject = new Decimal();
                    break;
                case TypeCode.Double:
                    resultObject = new Double();
                    break;
                case TypeCode.Int16:
                    resultObject = new Int16();
                    break;
                case TypeCode.Int32:
                    resultObject = new Int32();
                    break;
                case TypeCode.Int64:
                    resultObject = new Int64();
                    break;
                case TypeCode.SByte:
                    resultObject = new SByte();
                    break;
                case TypeCode.Single:
                    resultObject = new Single();
                    break;
                case TypeCode.String:
                    resultObject = "";
                    break;
                case TypeCode.UInt16:
                    resultObject = new UInt16();
                    break;
                case TypeCode.UInt32:
                    resultObject = new UInt32();
                    break;
                case TypeCode.UInt64:
                    resultObject = new UInt64();
                    break;
            }
            return resultObject;
        }
    }
}
