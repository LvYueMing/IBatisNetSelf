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
    /// Main implementation of ResultMap interface
    /// </summary>
    [Serializable]
    [XmlRoot("resultMap", Namespace = "http://ibatis.apache.org/mapping")]
    public class ResultMap : IResultMap
    {
        /// <summary>
        /// Token for xml path to argument constructor elements.
        /// </summary>
        public static BindingFlags ANY_VISIBILITY_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Token for xml path to result elements.
        /// </summary>
        private const string XML_RESULT = "result";

        /// <summary>
        /// Token for xml path to result elements.
        /// </summary>
        private const string XML_CONSTRUCTOR_ARGUMENT = "constructor/argument";

        /// <summary>
        /// Token for xml path to discriminator elements.
        /// </summary>
        private const string XML_DISCRIMNATOR = "discriminator";

        /// <summary>
        /// Token for xml path to subMap elements.
        /// </summary>
        private const string XML_SUBMAP = "subMap";

        private static IResultMap nullResultMap = null;

        #region Fields
        [NonSerialized]
        private bool isInitalized = true;
        [NonSerialized]
        private string id = string.Empty;
        [NonSerialized]
        private string resultClassName = string.Empty;
        [NonSerialized]
        private string extendMap = string.Empty;
        [NonSerialized]
        private Type resultClass = null;
        [NonSerialized]
        private StringCollection groupByPropertyNames = new StringCollection();

        [NonSerialized]
        private ResultPropertyCollection resultProperties = new ResultPropertyCollection();
        [NonSerialized]
        private ResultPropertyCollection groupByProperties = new ResultPropertyCollection();

        [NonSerialized]
        private ResultPropertyCollection parameters = new ResultPropertyCollection();

        [NonSerialized]
        private Discriminator discriminator = null;
        [NonSerialized]
        private string sqlMapNameSpace = string.Empty;
        [NonSerialized]
        private IFactory objectFactory = null;
        [NonSerialized]
        private DataExchangeFactory dataExchangeFactory = null;
        [NonSerialized]
        private IDataExchange dataExchange = null;
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

        /// <summary>
        /// The collection of ResultProperty.
        /// </summary>
        [XmlIgnore]
        public ResultPropertyCollection Properties
        {
            get { return resultProperties; }
        }

        /// <summary>
        /// The GroupBy Properties.
        /// </summary>
        [XmlIgnore]
        public ResultPropertyCollection GroupByProperties
        {
            get { return groupByProperties; }
        }

        /// <summary>
        /// The collection of constructor parameters.
        /// </summary>
        [XmlIgnore]
        public ResultPropertyCollection Parameters
        {
            get { return parameters; }
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

        /// <summary>
        /// The output type class of the resultMap.
        /// </summary>
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
        /// Initialize the resultMap from an xmlNode..
        /// </summary>
        /// <param name="aConfigScope"></param>
        public void Initialize(ConfigurationScope aConfigScope)
        {
            try
            {
                this.resultClass = aConfigScope.SqlMapper.TypeHandlerFactory.GetType(resultClassName);
                this.dataExchange = this.dataExchangeFactory.GetDataExchangeForClass(resultClass);

                // Load the child node
                GetChildNode(aConfigScope);

                // Verify that that each groupBy element correspond to a class member
                // of one of result property
                for (int i = 0; i < this.groupByProperties.Count; i++)
                {
                    string _memberName = this.GroupByPropertyNames[i];
                    if (!resultProperties.Contains(_memberName))
                    {
                        throw new ConfigurationException($"Could not configure ResultMap named \"{id}\". Check the groupBy attribute. Cause: there's no result property named \"{_memberName}\".");
                    }
                }
            }
            catch (Exception e)
            {
                throw new ConfigurationException($"Could not configure ResultMap named \"{id}\", Cause: {e.Message}", e);
            }
        }

        /// <summary>
        /// Initializes the groupBy properties.
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
        /// Get the result properties and the subMap properties.
        /// </summary>
        /// <param name="aConfigScope"></param>
        private void GetChildNode(ConfigurationScope aConfigScope)
        {
            ResultProperty _mapping = null;
            SubMap _subMap = null;

            #region Load the parameters constructor
            XmlNodeList _nodeList = aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_CONSTRUCTOR_ARGUMENT), aConfigScope.XmlNamespaceManager);
            if (_nodeList.Count > 0)
            {
                Type[] _parametersType = new Type[_nodeList.Count];
                string[] _parametersName = new string[_nodeList.Count];
                for (int i = 0; i < _nodeList.Count; i++)
                {
                    ArgumentProperty _argumentMapping = ArgumentPropertyDeSerializer.Deserialize(_nodeList[i], aConfigScope);
                    this.parameters.Add(_argumentMapping);
                    _parametersName[i] = _argumentMapping.ArgumentName;
                }
                ConstructorInfo _constructorInfo = this.GetConstructor(this.resultClass, _parametersName);
                for (int i = 0; i < parameters.Count; i++)
                {
                    ArgumentProperty _argumentMapping = (ArgumentProperty)this.parameters[i];

                    aConfigScope.ErrorContext.MoreInfo = "initialize argument property : " + _argumentMapping.ArgumentName;
                    _argumentMapping.Initialize(aConfigScope, _constructorInfo);
                    _parametersType[i] = _argumentMapping.MemberType;
                }
                // Init the object factory
                objectFactory = aConfigScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, _parametersType);
            }
            else
            {
                if (Type.GetTypeCode(resultClass) == TypeCode.Object)
                {
                    objectFactory = aConfigScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, Type.EmptyTypes);
                }
            }

            #endregion

            #region Load the Result Properties

            foreach (XmlNode resultNode in aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_RESULT), aConfigScope.XmlNamespaceManager))
            {
                _mapping = ResultPropertyDeSerializer.Deserialize(resultNode, aConfigScope);

                aConfigScope.ErrorContext.MoreInfo = "initialize result property: " + _mapping.PropertyName;

                _mapping.Initialize(aConfigScope, resultClass);

                resultProperties.Add(_mapping);
            }
            #endregion

            #region Load the Discriminator Property

            XmlNode discriminatorNode = aConfigScope.CurrentNodeContext.SelectSingleNode(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_DISCRIMNATOR), aConfigScope.XmlNamespaceManager);
            if (discriminatorNode != null)
            {
                aConfigScope.ErrorContext.MoreInfo = "initialize discriminator";

                this.Discriminator = DiscriminatorDeSerializer.Deserialize(discriminatorNode, aConfigScope);
                this.Discriminator.SetMapping(aConfigScope, resultClass);
            }
            #endregion

            #region Load the SubMap Properties

            if (aConfigScope.CurrentNodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_SUBMAP), aConfigScope.XmlNamespaceManager).Count > 0 && this.Discriminator == null)
            {
                throw new ConfigurationException("The discriminator is null, but somehow a subMap was reached.  This is a bug.");
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
        /// Sets the object factory.
        /// </summary>
        public void SetObjectFactory(ConfigurationScope configScope)
        {
            Type[] parametersType = new Type[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                ArgumentProperty argumentMapping = (ArgumentProperty)parameters[i];
                parametersType[i] = argumentMapping.MemberType;
            }
            // Init the object factory
            objectFactory = configScope.SqlMapper.ObjectFactory.CreateFactory(resultClass, parametersType);
        }

        /// <summary>
        /// Finds the constructor that takes the parameters.
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
        /// Create an instance Of result.
        /// </summary>
        /// <param name="parameters">
        /// An array of values that matches the number, order and type 
        /// of the parameters for this constructor. 
        /// </param>
        /// <returns>An object.</returns>
        public object CreateInstanceOfResult(object[] parameters)
        {
            TypeCode typeCode = Type.GetTypeCode(resultClass);

            if (typeCode == TypeCode.Object)
            {
                return objectFactory.CreateInstance(parameters);
            }
            else
            {
                return TypeUtils.InstantiatePrimitiveType(typeCode);
            }
        }

        /// <summary>
        /// Set the value of an object property.
        /// </summary>
        /// <param name="target">The object to set the property.</param>
        /// <param name="aProperty">The result property to use.</param>
        /// <param name="dataBaseValue">The database value to set.</param>
        public void SetValueOfProperty(ref object target, ResultProperty aProperty, object dataBaseValue)
        {
            dataExchange.SetData(ref target, aProperty, dataBaseValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
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
