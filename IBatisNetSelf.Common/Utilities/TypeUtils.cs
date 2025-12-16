using IBatisNetSelf.Common.Utilities.TypesResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// 用于解析类型名以及实例化原始类型的工具类。
    /// </summary>
    public sealed class TypeUtils
    {
        #region Fields
        // 内部使用的类型解析器，带缓存功能，提升性能。
        private static readonly ITypeResolver typeResolverCache = new CachedTypeResolver(new TypeResolver());

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
        /// 将提供的类型名称解析为一个实际的 Type 实例。
        /// </summary>
        /// <param name="typeName">类型名称，可能是部分或完整的程序集限定名。</param>
        /// <returns>解析得到的 Type 实例。</returns>
        /// <exception cref="TypeLoadException">如果类型无法被解析则抛出异常。</exception>
        public static Type ResolveType(string typeName)
        {
            // 先尝试使用 TypeRegistry（预加载的系统类型）进行类型解析。
            Type? _type = TypeRegistry.ResolveType(typeName);
            if (_type == null)
            {
                _type = typeResolverCache.Resolve(typeName);
            }
            return _type;
        }

        /// <summary>
        /// 创建一个指定 TypeCode 类型的默认实例（用于原始类型）。
        /// </summary>
        /// <param name="typeCode">原始类型的 TypeCode。</param>
        /// <returns>该类型的默认值实例，如果不是原始类型则返回 null。</returns>
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
