using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.TypesResolver
{
    /// <summary> 
    /// 提供对别名注册表的访问 <see cref="System.Type"/>s.
    /// </summary>
    /// <remarks>
    /// <p>
    /// 通过使用别名而不是完全限定类型名来简化配置
    /// </p>
    /// <p>
    /// Comes 'pre-loaded' with a number of convenience alias for the more common types; 
    /// an example would be the '<c>int</c>' alias for the <see cref="System.Int32"/> type.
    /// </p>
    /// </remarks>
    internal class TypeRegistry
    {
        #region Constants

        /// <summary>
        /// The alias around the 'list' type.
        /// </summary>
        public const string ArrayListAlias1 = "arraylist";
        /// <summary>
        /// Another alias around the 'list' type.
        /// </summary>
        public const string ArrayListAlias2 = "list";

        /// <summary>
        /// Another alias around the 'bool' type.
        /// </summary>
        public const string BoolAlias = "bool";
        /// <summary>
        /// The alias around the 'bool' type.
        /// </summary>
        public const string BooleanAlias = "boolean";

        /// <summary>
        /// The alias around the 'byte' type.
        /// </summary>
        public const string ByteAlias = "byte";

        /// <summary>
        /// The alias around the 'char' type.
        /// </summary>
        public const string CharAlias = "char";

        /// <summary>
        /// The alias around the 'DateTime' type.
        /// </summary>
        public const string DateAlias1 = "datetime";
        /// <summary>
        /// Another alias around the 'DateTime' type.
        /// </summary>
        public const string DateAlias2 = "date";

        /// <summary>
        /// The alias around the 'decimal' type.
        /// </summary>
        public const string DecimalAlias = "decimal";

        /// <summary>
        /// The alias around the 'double' type.
        /// </summary>
        public const string DoubleAlias = "double";


        /// <summary>
        /// The alias around the 'float' type.
        /// </summary>
        public const string FloatAlias = "float";
        /// <summary>
        /// Another alias around the 'float' type.
        /// </summary>
        public const string SingleAlias = "single";

        /// <summary>
        /// The alias around the 'guid' type.
        /// </summary>
        public const string GuidAlias = "guid";

        /// <summary>
        /// The alias around the 'Hashtable' type.
        /// </summary>
        public const string HashtableAlias1 = "hashtable";
        /// <summary>
        /// Another alias around the 'Hashtable' type.
        /// </summary>
        public const string HashtableAlias2 = "map";
        /// <summary>
        /// Another alias around the 'Hashtable' type.
        /// </summary>
        public const string HashtableAlias3 = "hashmap";

        /// <summary>
        /// The alias around the 'short' type.
        /// </summary>
        public const string Int16Alias1 = "int16";
        /// <summary>
        /// Another alias around the 'short' type.
        /// </summary>
        public const string Int16Alias2 = "short";


        /// <summary>
        /// The alias around the 'int' type.
        /// </summary>
        public const string Int32Alias1 = "int32";
        /// <summary>
        /// Another alias around the 'int' type.
        /// </summary>
        public const string Int32Alias2 = "int";
        /// <summary>
        /// Another alias around the 'int' type.
        /// </summary>
        public const string Int32Alias3 = "integer";

        /// <summary>
        /// The alias around the 'long' type.
        /// </summary>
        public const string Int64Alias1 = "int64";
        /// <summary>
        /// Another alias around the 'long' type.
        /// </summary>
        public const string Int64Alias2 = "long";

        /// <summary>
        /// The alias around the 'unsigned short' type.
        /// </summary>
        public const string UInt16Alias1 = "uint16";
        /// <summary>
        /// Another alias around the 'unsigned short' type.
        /// </summary>
        public const string UInt16Alias2 = "ushort";

        /// <summary>
        /// The alias around the 'unsigned int' type.
        /// </summary>
        public const string UInt32Alias1 = "uint32";
        /// <summary>
        /// Another alias around the 'unsigned int' type.
        /// </summary>
        public const string UInt32Alias2 = "uint";

        /// <summary>
        /// The alias around the 'unsigned long' type.
        /// </summary>
        public const string UInt64Alias1 = "uint64";
        /// <summary>
        /// Another alias around the 'unsigned long' type.
        /// </summary>
        public const string UInt64Alias2 = "ulong";

        /// <summary>
        /// The alias around the 'SByte' type.
        /// </summary>
        public const string SByteAlias = "sbyte";

        /// <summary>
        /// The alias around the 'string' type.
        /// </summary>
        public const string StringAlias = "string";

        /// <summary>
        /// The alias around the 'TimeSpan' type.
        /// </summary>
        public const string TimeSpanAlias = "timespan";

        #endregion

        #region Fields

        private static IDictionary preLoadedTypes = new Hashtable();

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Creates a new instance of the <see cref="TypeRegistry"/> class.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This is a utility class, and as such has no publicly visible
        /// constructors.
        /// </p>
        /// </remarks>
        private TypeRegistry() { }

        /// <summary>
        /// Initialises the static properties of the TypeAliasResolver class.
        /// </summary>
        static TypeRegistry()
        {
            // Initialize a dictionary with some fully qualifiaed name 
            preLoadedTypes[ArrayListAlias1] = typeof(ArrayList);
            preLoadedTypes[ArrayListAlias2] = typeof(ArrayList);

            preLoadedTypes[BoolAlias] = typeof(bool);
            preLoadedTypes[BooleanAlias] = typeof(bool);

            preLoadedTypes[ByteAlias] = typeof(byte);

            preLoadedTypes[CharAlias] = typeof(char);

            preLoadedTypes[DateAlias1] = typeof(DateTime);
            preLoadedTypes[DateAlias2] = typeof(DateTime);

            preLoadedTypes[DecimalAlias] = typeof(decimal);

            preLoadedTypes[DoubleAlias] = typeof(double);

            preLoadedTypes[FloatAlias] = typeof(float);
            preLoadedTypes[SingleAlias] = typeof(float);

            preLoadedTypes[GuidAlias] = typeof(Guid);

            preLoadedTypes[HashtableAlias1] = typeof(Hashtable);
            preLoadedTypes[HashtableAlias2] = typeof(Hashtable);
            preLoadedTypes[HashtableAlias3] = typeof(Hashtable);

            preLoadedTypes[Int16Alias1] = typeof(short);
            preLoadedTypes[Int16Alias2] = typeof(short);

            preLoadedTypes[Int32Alias1] = typeof(int);
            preLoadedTypes[Int32Alias2] = typeof(int);
            preLoadedTypes[Int32Alias3] = typeof(int);

            preLoadedTypes[Int64Alias1] = typeof(long);
            preLoadedTypes[Int64Alias2] = typeof(long);

            preLoadedTypes[UInt16Alias1] = typeof(ushort);
            preLoadedTypes[UInt16Alias2] = typeof(ushort);

            preLoadedTypes[UInt32Alias1] = typeof(uint);
            preLoadedTypes[UInt32Alias2] = typeof(uint);

            preLoadedTypes[UInt64Alias1] = typeof(ulong);
            preLoadedTypes[UInt64Alias2] = typeof(ulong);

            preLoadedTypes[SByteAlias] = typeof(sbyte);

            preLoadedTypes[StringAlias] = typeof(string);

            preLoadedTypes[TimeSpanAlias] = typeof(string);

        }
        #endregion

        #region Methods

        /// <summary> 
        /// Resolves the supplied <paramref name="alias"/> to a <see cref="System.Type"/>. 
        /// </summary> 
        /// <param name="alias">
        /// The alias to resolve.
        /// </param>
        /// <returns>
        /// The <see cref="System.Type"/> the supplied <paramref name="alias"/> was
        /// associated with, or <see lang="null"/> if no <see cref="System.Type"/> 
        /// was previously registered for the supplied <paramref name="alias"/>.
        /// </returns>
        /// <remarks>The alis name will be convert in lower character before the resolution.</remarks>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="alias"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static Type? ResolveType(string alias)
        {
            return preLoadedTypes[alias.ToLower()] as Type;
        }

        #endregion
    }
}
