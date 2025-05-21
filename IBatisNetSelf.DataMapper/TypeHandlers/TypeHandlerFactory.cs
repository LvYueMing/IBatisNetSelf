using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.Configuration.Alias;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// 显然，这是类型处理器工厂类的标准实现
    /// </summary>
    public class TypeHandlerFactory
    {

        #region Fields

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IDictionary typeHandlerMap = new HybridDictionary();
        private ITypeHandler unknownTypeHandler = null;
        private const string DB_NULL = "_NULL_TYPE_";
        //(typeAlias name, type alias)
        private IDictionary typeAliasMaps = new HybridDictionary();
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public TypeHandlerFactory()
        {
            ITypeHandler _handler = null;

            _handler = new DBNullTypeHandler();
            this.Register(typeof(DBNull), _handler);

            _handler = new BooleanTypeHandler();
            this.Register(typeof(bool), _handler); // key= "System.Boolean"

            _handler = new ByteTypeHandler();
            this.Register(typeof(Byte), _handler);

            _handler = new CharTypeHandler();
            this.Register(typeof(Char), _handler);

            _handler = new DateTimeTypeHandler();
            this.Register(typeof(DateTime), _handler);

            _handler = new DecimalTypeHandler();
            this.Register(typeof(Decimal), _handler);

            _handler = new DoubleTypeHandler();
            this.Register(typeof(Double), _handler);

            _handler = new Int16TypeHandler();
            this.Register(typeof(Int16), _handler);

            _handler = new Int32TypeHandler();
            this.Register(typeof(Int32), _handler);

            _handler = new Int64TypeHandler();
            this.Register(typeof(Int64), _handler);

            _handler = new SingleTypeHandler();
            this.Register(typeof(Single), _handler);

            _handler = new StringTypeHandler();
            this.Register(typeof(String), _handler);

            _handler = new GuidTypeHandler();
            this.Register(typeof(Guid), _handler);

            _handler = new TimeSpanTypeHandler();
            this.Register(typeof(TimeSpan), _handler);

            _handler = new ByteArrayTypeHandler();
            this.Register(typeof(Byte[]), _handler);

            _handler = new ObjectTypeHandler();
            this.Register(typeof(object), _handler);

            _handler = new EnumTypeHandler();
            this.Register(typeof(System.Enum), _handler);

            _handler = new UInt16TypeHandler();
            this.Register(typeof(UInt16), _handler);

            _handler = new UInt32TypeHandler();
            this.Register(typeof(UInt32), _handler);

            _handler = new UInt64TypeHandler();
            this.Register(typeof(UInt64), _handler);

            _handler = new SByteTypeHandler();
            this.Register(typeof(SByte), _handler);

            this.unknownTypeHandler = new UnknownTypeHandler(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get a TypeHandler for a Type
        /// </summary>
        /// <param name="type">the Type you want a TypeHandler for</param>
        /// <returns>the handler</returns>
        public ITypeHandler GetTypeHandler(Type type)
        {
            return GetTypeHandler(type, null);
        }

        /// <summary>
        /// Get a TypeHandler for a type
        /// </summary>
        /// <param name="type">the type you want a TypeHandler for</param>
        /// <param name="dbType">the database type</param>
        /// <returns>the handler</returns>
        public ITypeHandler GetTypeHandler(Type type, string dbType)
        {
            if (type.IsEnum)
            {
                return this.GetPrivateTypeHandler(typeof(System.Enum), dbType);
            }
            else
            {
                return this.GetPrivateTypeHandler(type, dbType);
            }
        }

        /// <summary>
        ///  Get a TypeHandler for a type and a dbType type
        /// </summary>
        /// <param name="aType">the type</param>
        /// <param name="aDbType">the dbType type</param>
        /// <returns>the handler</returns>
        private ITypeHandler GetPrivateTypeHandler(Type aType, string aDbType)
        {
            IDictionary _dbTypeHandlerMap = (IDictionary)this.typeHandlerMap[aType];
            ITypeHandler _handler = null;

            if (_dbTypeHandlerMap != null)
            {
                if (aDbType == null)
                {
                    _handler = (ITypeHandler)_dbTypeHandlerMap[DB_NULL];
                }
                else
                {
                    _handler = (ITypeHandler)_dbTypeHandlerMap[aDbType];
                    if (_handler == null)
                    {
                        _handler = (ITypeHandler)_dbTypeHandlerMap[DB_NULL];
                    }
                }
                if (_handler == null)
                {
                    throw new DataMapperException(String.Format("Type handler for {0} not registered.", aType.Name));
                }
            }

            return _handler;
        }


        /// <summary>
        /// Register (add) a type handler for a type
        /// </summary>
        /// <param name="type">the type</param>
        /// <param name="handler">the handler instance</param>
        public void Register(Type type, ITypeHandler handler)
        {
            this.Register(type, null, handler);
        }

        /// <summary>
        /// 为.NET类型和数据库类型 注册（添加）一个类型处理器
        /// </summary>
        /// <param name="aType">要处理的.NET类型</param>
        /// <param name="aDbType">数据库类型（可选，如果为null则处理器将用于所有数据库类型）</param>
        /// <param name="aHandler">类型处理器实例</param>
        public void Register(Type aType, string aDbType, ITypeHandler aHandler)
        {
            // 从类型处理器映射表中获取该类型对应的数据库类型字典
            HybridDictionary _map = (HybridDictionary)this.typeHandlerMap[aType];
            // 如果该类型尚未注册过处理器
            if (_map == null)
            {
                // 数据库类型与处理器的映射字典
                _map = new HybridDictionary();
                this.typeHandlerMap.Add(aType, _map);
            }

            // 当 dbType 为 null 时，表示注册一个通用处理器（适用于所有数据库类型）
            if (aDbType == null)
            {
                // 提示用户当前操作将替换系统内置的类型处理器
                if (logger.IsInfoEnabled)
                {
                    // 获取之前注册的通用处理器（使用DB_NULL作为键）
                    ITypeHandler _oldTypeHandler = (ITypeHandler)_map[DB_NULL];

                    if (_oldTypeHandler != null)
                    {
                        // the replacement will always(?) be a CustomTypeHandler
                        CustomTypeHandler _customTypeHandler = aHandler as CustomTypeHandler;

                        string _replacement = string.Empty;

                        if (_customTypeHandler != null)
                        {
                            // report the underlying type
                            _replacement = _customTypeHandler.Callback.ToString();
                        }
                        else
                        {
                            _replacement = aHandler.ToString();
                        }

                        // 记录替换日志（原处理器 -> 新处理器）
                        logger.Info("Replacing type handler [" + _oldTypeHandler.ToString() + "] with [" + _replacement + "].");
                    }
                }

                _map[DB_NULL] = aHandler;
            }
            else
            {
                _map.Add(aDbType, aHandler);
            }
        }

        /// <summary>
        /// 【获取未知类型处理器】- 当无法确定具体类型时的兜底处理器
        /// </summary>
        /// <returns>
        /// 预定义的未知类型处理器（用于处理未明确注册的类型转换）
        /// </returns>
        /// <remarks>
        /// 设计意图：
        /// 1. 作为类型处理系统的安全网机制
        /// 2. 避免因类型未注册导致系统崩溃
        /// 3. 通常包含基础的类型转换逻辑或特殊错误处理
        /// 
        /// 典型应用场景：
        /// - 动态SQL查询返回未映射的类型
        /// - 新添加的数据类型尚未配置处理器
        /// - 反射生成的临时类型处理
        /// </remarks>
        public ITypeHandler GetUnkownTypeHandler()
        {
            return unknownTypeHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSimpleType(Type type)
        {
            bool _result = false;
            if (type != null)
            {
                ITypeHandler handler = this.GetTypeHandler(type, null);
                if (handler != null)
                {
                    _result = handler.IsSimpleType;
                }
            }
            return _result;
        }

        #endregion

        /// <summary>
        /// Gets a named TypeAlias from the list of available TypeAlias
        /// </summary>
        /// <param name="name">The name of the TypeAlias.</param>
        /// <returns>The TypeAlias.</returns>
        internal TypeAlias GetTypeAlias(string name)
        {
            if (this.typeAliasMaps.Contains(name) == true)
            {
                return (TypeAlias)this.typeAliasMaps[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the type object from the specific class name.
        /// </summary>
        /// <param name="className">The supplied class name.</param>
        /// <returns>The correpsonding type.
        /// </returns>
        internal Type GetType(string className)
        {
            Type type = null;
            TypeAlias typeAlias = this.GetTypeAlias(className) as TypeAlias;

            if (typeAlias != null)
            {
                type = typeAlias.Class;
            }
            else
            {
                type = TypeUtils.ResolveType(className);
            }

            return type;
        }

        /// <summary>
        /// Adds a named TypeAlias to the list of available TypeAlias.
        /// </summary>
        /// <param name="key">The key name.</param>
        /// <param name="typeAlias"> The TypeAlias.</param>
        internal void AddTypeAlias(string key, TypeAlias typeAlias)
        {
            if (this.typeAliasMaps.Contains(key) == true)
            {
                throw new DataMapperException(" Alias name conflict occurred.  The type alias '" + key + "' is already mapped to the value '" + typeAlias.ClassName + "'.");
            }
            this.typeAliasMaps.Add(key, typeAlias);
        }
    }
}
