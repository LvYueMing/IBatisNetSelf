using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.DataExchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Specialized;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.DataMapper.Configuration.Serializers;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// IResultMap 接口的主要实现类，负责定义 SQL 查询结果如何映射到 .NET 类
    /// 它支持复杂的结果结构，包括构造函数映射、子映射（SubMap）、鉴别器（Discriminator）等功能。
    /// </summary>
    [Serializable]
    [XmlRoot("resultMap", Namespace = "http://ibatis.apache.org/mapping")]
    public class ResultMap : IResultMap
    {
        /// <summary>
        /// 用于查找构造函数参数的反射标志，包含公共和非公共实例构造函数
        /// </summary>
        public static BindingFlags ANY_VISIBILITY_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // 以下常量用于从 XML 配置中查找对应的节点
        private const string XML_RESULT = "result";
        private const string XML_CONSTRUCTOR_ARGUMENT = "constructor/argument";
        private const string XML_DISCRIMNATOR = "discriminator";
        private const string XML_SUBMAP = "subMap";

        /// <summary>
        /// 空对象模式使用的 ResultMap 占位符（用于防止空引用）
        /// </summary>
        private static IResultMap nullResultMap = null;

        #region Fields
        [NonSerialized]
        private bool isInitalized = true;// 是否已初始化
        [NonSerialized]
        private string id = string.Empty;// resultMap 的唯一标识
        [NonSerialized]
        private string resultClassName = string.Empty;// 映射的目标类名（字符串）
        [NonSerialized]
        private string extendMap = string.Empty;// 要扩展的父映射
        [NonSerialized]
        private Type resultClass = null;// 目标类型（Type）
        [NonSerialized]
        private StringCollection groupByPropertyNames = new StringCollection();// groupBy 属性名集合

        [NonSerialized]
        private ResultPropertyCollection resultProperties = new ResultPropertyCollection(); // 所有映射属性
        [NonSerialized]
        private ResultPropertyCollection groupByProperties = new ResultPropertyCollection();// groupBy 的属性集合

        [NonSerialized]
        private ResultPropertyCollection constructorParams = new ResultPropertyCollection();// 构造函数参数属性集合

        [NonSerialized]
        private Discriminator discriminator = null; // 用于子映射切换的鉴别器
        [NonSerialized]
        private string sqlMapNameSpace = string.Empty;// 当前 SQL 映射的命名空间
        [NonSerialized]
        private IFactory objectFactory = null;// 对象创建工厂
        [NonSerialized]
        private DataExchangeFactory dataExchangeFactory = null;// 数据交换工厂
        [NonSerialized]
        private IDataExchange dataExchange = null;// 数据交换器
        #endregion

        #region Properties

        /// <summary>
        /// The GroupBy Properties.
        /// </summary>
        [XmlIgnore]
        public StringCollection GroupByPropertyNames
        {
            get { return groupByPropertyNames; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initalized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initalized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitalized
        {
            get { return true; }
            set { isInitalized = value; }
        }

        /// <summary>
        /// The discriminator used to choose the good SubMap
        /// </summary>
        [XmlIgnore]
        public Discriminator Discriminator
        {
            get { return discriminator; }
            set { discriminator = value; }
        }

        /// <summary>所有的 ResultProperty（不包含构造函数参数）</summary>
        [XmlIgnore]
        public ResultPropertyCollection Properties
        {
            get { return resultProperties; }
        }

        /// <summary>所有用于 GroupBy 的 ResultProperty</summary>
        [XmlIgnore]
        public ResultPropertyCollection GroupByProperties
        {
            get { return groupByProperties; }
        }

        /// <summary>构造函数参数使用的属性集合</summary>
        [XmlIgnore]
        public ResultPropertyCollection ConstructorParams
        {
            get { return constructorParams; }
        }

        /// <summary>
        /// Identifier used to identify the resultMap amongst the others.
        /// </summary>
        /// <example>GetProduct</example>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Extend ResultMap attribute
        /// </summary>
        [XmlAttribute("extends")]
        public string ExtendMap
        {
            get { return extendMap; }
            set { extendMap = value; }
        }

        /// <summary>目标映射的 CLR 类型</summary>
        [XmlIgnore]
        public Type Class
        {
            get { return resultClass; }
        }


        /// <summary>
        /// Sets the IDataExchange
        /// </summary>
        [XmlIgnore]
        public IDataExchange DataExchange
        {
            set { dataExchange = value; }
        }
        #endregion

        #region Constructor (s) / Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMap"/> class.
        /// </summary>
        /// <param name="aConfigScope">The config scope.</param>
        /// <param name="aClassName">The output class name of the resultMap.</param>
        /// <param name="aExtendMap">The extend result map bame.</param>
        /// <param name="aId">Identifier used to identify the resultMap amongst the others.</param>
        /// <param name="aGroupBy">The groupBy properties</param>
        public ResultMap(ConfigurationScope aConfigScope, string aId, string aClassName, string aExtendMap, string aGroupBy)
        {
            nullResultMap = new NullResultMap();

            this.dataExchangeFactory = aConfigScope.DataExchangeFactory;
            this.sqlMapNameSpace = aConfigScope.SqlMapNamespace;
            if ((aId == null) || (aId.Length < 1))
            {
                throw new ArgumentNullException("在ResultMap标签中，id属性是必选的");
            }
            //Namespace.id
            this.id = aConfigScope.ApplyNamespace(aId);
            if ((aClassName == null) || (aClassName.Length < 1))
            {
                throw new ArgumentNullException("在ResultMap标签id中，class属性是必需的:" + this.id);
            }
            this.resultClassName = aClassName;
            this.extendMap = aExtendMap;

            // 解析 groupBy 列表（逗号分隔）
            if (aGroupBy != null && aGroupBy.Length > 0)
            {
                string[] groupByProperties = aGroupBy.Split(',');
                for (int i = 0; i < groupByProperties.Length; i++)
                {
                    string _memberName = groupByProperties[i].Trim();
                    groupByPropertyNames.Add(_memberName);
                }
            }

        }
        #endregion

        #region Methods

        #region Configuration

        /// <summary>
        /// 初始化 ResultMap：解析类型、数据交换器、子节点（result、argument 等）
        /// </summary>
        public void Initialize(ConfigurationScope aConfigScope)
        {
            try
            {
                this.resultClass = aConfigScope.SqlMapper.TypeHandlerFactory.GetType(resultClassName);
                this.dataExchange = this.dataExchangeFactory.GetDataExchangeForClass(resultClass);

                // 解析 XML 子节点
                GetChildNode(aConfigScope);

                // 校验 groupBy 属性名是否都存在于 resultProperties 中
                for (int i = 0; i < this.groupByProperties.Count; i++)
                {
                    string _memberName = this.GroupByPropertyNames[i];
                    if (!resultProperties.Contains(_memberName))
                    {
                        throw new IBatisConfigException($"Could not configure ResultMap named \"{id}\". Check the groupBy attribute. Cause: there's no result property named \"{_memberName}\".");
                    }
                }
            }
            catch (Exception e)
            {
                throw new IBatisConfigException($"Could not configure ResultMap named \"{id}\", Cause: {e.Message}", e);
            }
        }

        /// <summary>
        /// 初始化 GroupBy 的 ResultProperty 集合（根据名称查找）
        /// </summary>
        public void InitializeGroupByProperties()
        {
            for (int i = 0; i < GroupByPropertyNames.Count; i++)
            {
                ResultProperty resultProperty = Properties.FindByPropertyName(this.GroupByPropertyNames[i]);
                this.GroupByProperties.Add(resultProperty);
            }
        }


        /// <summary>
        /// 解析 XML 中的子节点：constructor、result、discriminator、subMap
        /// </summary>
        private void GetChildNode(ConfigurationScope aConfigScope)
        {
            ResultProperty _mapping = null;
            SubMap _subMap = null;

            #region 解析构造函数参数 argument
            XmlNodeList _nodeList = aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_CONSTRUCTOR_ARGUMENT), aConfigScope.XmlNamespaceManager);
            if (_nodeList.Count > 0)
            {
                Type[] _parametersType = new Type[_nodeList.Count];
                string[] _parametersName = new string[_nodeList.Count];
                for (int i = 0; i < _nodeList.Count; i++)
                {
                    ArgumentProperty _argumentMapping = ArgumentPropertyDeSerializer.Deserialize(_nodeList[i], aConfigScope);
                    this.constructorParams.Add(_argumentMapping);
                    _parametersName[i] = _argumentMapping.ArgumentName;
                }
                ConstructorInfo _constructorInfo = this.GetConstructor(this.resultClass, _parametersName);
                for (int i = 0; i < constructorParams.Count; i++)
                {
                    ArgumentProperty _argumentMapping = (ArgumentProperty)this.constructorParams[i];

                    aConfigScope.ErrorContext.MoreInfo = "initialize argument property : " + _argumentMapping.ArgumentName;
                    _argumentMapping.Initialize(aConfigScope, _constructorInfo);
                    _parametersType[i] = _argumentMapping.MemberType;
                }
                // Init the object factory
                objectFactory = aConfigScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, _parametersType);
            }
            else
            {
                // 没有构造参数时，创建无参对象工厂（如果是复杂对象）
                if (Type.GetTypeCode(resultClass) == TypeCode.Object)
                {
                    objectFactory = aConfigScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, Type.EmptyTypes);
                }
            }

            #endregion

            #region 解析 result 节点

            foreach (XmlNode resultNode in aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_RESULT), aConfigScope.XmlNamespaceManager))
            {
                _mapping = ResultPropertyDeSerializer.Deserialize(resultNode, aConfigScope);

                aConfigScope.ErrorContext.MoreInfo = "initialize result property: " + _mapping.PropertyName;

                _mapping.Initialize(aConfigScope, resultClass);

                resultProperties.Add(_mapping);
            }
            #endregion

            #region 解析 discriminator 节点

            XmlNode discriminatorNode = aConfigScope.CurrentNodeContext.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_DISCRIMNATOR), aConfigScope.XmlNamespaceManager);
            if (discriminatorNode != null)
            {
                aConfigScope.ErrorContext.MoreInfo = "initialize discriminator";

                this.Discriminator = DiscriminatorDeSerializer.Deserialize(discriminatorNode, aConfigScope);
                this.Discriminator.SetMapping(aConfigScope, resultClass);
            }
            #endregion

            #region 解析 subMap 节点

            if (aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_SUBMAP), aConfigScope.XmlNamespaceManager).Count > 0 && this.Discriminator == null)
            {
                throw new IBatisConfigException("The discriminator is null, but somehow a subMap was reached.  This is a bug.");
            }
            foreach (XmlNode resultNode in aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_SUBMAP), aConfigScope.XmlNamespaceManager))
            {
                aConfigScope.ErrorContext.MoreInfo = "initialize subMap";
                _subMap = SubMapDeSerializer.Deserialize(resultNode, aConfigScope);

                this.Discriminator.Add(_subMap);
            }
            #endregion
        }

        /// <summary>
        /// 根据构造函数参数类型初始化对象工厂
        /// </summary>
        public void SetObjectFactory(ConfigurationScope configScope)
        {
            Type[] parametersType = new Type[constructorParams.Count];
            for (int i = 0; i < constructorParams.Count; i++)
            {
                ArgumentProperty argumentMapping = (ArgumentProperty)constructorParams[i];
                parametersType[i] = argumentMapping.MemberType;
            }
            // Init the object factory
            objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, parametersType);
        }

        /// <summary>
        /// 匹配构造函数（按参数名匹配）
        /// </summary>
        /// <param name="aType">The <see cref="System.Type"/> to find the constructor in.</param> 
        /// <param name="aParametersName">The parameters name to use to find the appropriate constructor.</param>
        /// <returns>
        /// An <see cref="ConstructorInfo"/> that can be used to create the type with 
        /// the specified parameters.
        /// </returns>
        /// <exception cref="DataMapperException">
        /// Thrown when no constructor with the correct signature can be found.
        /// </exception> 
        private ConstructorInfo GetConstructor(Type aType, string[] aParametersName)
        {
            ConstructorInfo[] _constructors = aType.GetConstructors(ANY_VISIBILITY_INSTANCE);
            foreach (ConstructorInfo _constructor in _constructors)
            {
                ParameterInfo[] _parameters = _constructor.GetParameters();

                if (_parameters.Length == aParametersName.Length)
                {
                    bool _found = true;

                    for (int j = 0; j < _parameters.Length; j++)
                    {
                        bool _equal = (_parameters[j].Name == aParametersName[j]);
                        if (!_equal)
                        {
                            _found = false;
                            break;
                        }
                    }

                    if (_found)
                    {
                        return _constructor;
                    }
                }
            }
            throw new DataMapperException("Cannot find an appropriate constructor which map parameters in class: " + aType.Name);
        }

        #endregion

        /// <summary>
        /// 创建一个映射结果对象的实例
        /// </summary>
        public object CreateInstanceOfResult(object[] parameters)
        {
            TypeCode typeCode = Type.GetTypeCode(resultClass);

            if (typeCode == TypeCode.Object)
            {
                return this.objectFactory.CreateInstance(parameters);
            }
            else
            {
                return TypeUtils.InstantiatePrimitiveType(typeCode);
            }
        }

        /// <summary>
        /// 为映射对象设置某个属性值
        /// </summary>
        public void SetValueOfProperty(ref object target, ResultProperty aProperty, object dataBaseValue)
        {
            dataExchange.SetData(ref target, aProperty, dataBaseValue);
        }

        /// <summary>
        /// 根据 discriminator 判断应该使用哪个 subMap（递归调用以支持嵌套 discriminator）
        /// </summary>
        public IResultMap ResolveSubMap(IDataReader dataReader)
        {
            IResultMap subMap = this;
            if (discriminator != null)
            {
                ResultProperty mapping = discriminator.ResultProperty;
                object dataBaseValue = mapping.GetDataBaseValue(dataReader);

                if (dataBaseValue != null)
                {
                    subMap = discriminator.GetSubMap(dataBaseValue.ToString());

                    if (subMap == null)
                    {
                        subMap = this;
                    }
                    else if (subMap != this)
                    {
                        subMap = subMap.ResolveSubMap(dataReader);
                    }
                }
                else
                {
                    subMap = nullResultMap;
                }
            }
            return subMap;
        }


        #endregion
    }
}
