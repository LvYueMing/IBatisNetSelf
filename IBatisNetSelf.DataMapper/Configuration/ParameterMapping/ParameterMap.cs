using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace IBatisNetSelf.DataMapper.Configuration.ParameterMapping
{
    /// <summary>
    /// Summary description for ParameterMap.
    /// </summary>
    [Serializable]
    [XmlRoot("parameterMap", Namespace = "http://ibatis.apache.org/mapping")]
    public class ParameterMap
    {
        /// <summary>
        /// Token for xml path to parameter elements.
        /// </summary>
        private const string XML_PARAMATER = "parameter";

        #region private
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [NonSerialized]
        private string id = string.Empty;
        [NonSerialized]
        // Properties list
        private ParameterPropertyCollection properties = new ParameterPropertyCollection();
        // Same list as properties but without doubled (Test UpdateAccountViaParameterMap2)
        [NonSerialized]
        private ParameterPropertyCollection propertiesList = new ParameterPropertyCollection();
        //(property Name, property)
        [NonSerialized]
        private Hashtable propertiesMap = new Hashtable(); // Corrected ?? Support Request 1043181, move to HashTable
        [NonSerialized]
        private string extendMap = string.Empty;
        [NonSerialized]
        private bool usePositionalParameters = false;
        [NonSerialized]
        private string className = string.Empty;
        [NonSerialized]
        private Type parameterClass = null;
        [NonSerialized]
        private DataExchangeFactory dataExchangeFactory = null;
        [NonSerialized]
        private IDataExchange dataExchange = null;
        #endregion

        #region Properties

        /// <summary>
        /// The parameter class name.
        /// </summary>
        [XmlAttribute("class")]
        public string ClassName
        {
            get { return className; }
            set
            {
                if (_logger.IsInfoEnabled)
                {
                    if ((value == null) || (value.Length < 1))
                    {
                        _logger.Info("The class attribute is recommended for better performance in a ParameterMap tag '" + id + "'.");
                    }
                }


                className = value;
            }
        }

        /// <summary>
        /// Identifier used to identify the ParameterMap amongst the others.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if ((value == null) || (value.Length < 1))
                    throw new ArgumentNullException("The id attribute is mandatory in a ParameterMap tag.");

                id = value;
            }
        }

        /// <summary>
        /// The parameter type class.
        /// </summary>
        [XmlIgnore]
        public Type Class
        {
            set { parameterClass = value; }
            get { return parameterClass; }
        }



        /// <summary>
        /// The collection of ParameterProperty
        /// </summary>
        [XmlIgnore]
        public ParameterPropertyCollection Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public ParameterPropertyCollection PropertiesList
        {
            get { return propertiesList; }
        }

        /// <summary>
        /// Extend Parametermap attribute
        /// </summary>
        /// <remarks>The id of a ParameterMap</remarks>
        [XmlAttribute("extends")]
        public string ExtendMap
        {
            get { return extendMap; }
            set { extendMap = value; }
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
        /// Do not use direclty, only for serialization.
        /// </summary>
        /// <param name="dataExchangeFactory"></param>
        public ParameterMap(DataExchangeFactory dataExchangeFactory)
        {
            this.dataExchangeFactory = dataExchangeFactory;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Get the ParameterProperty at index.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>A ParameterProperty</returns>
        public ParameterProperty GetProperty(int index)
        {
            if (this.usePositionalParameters) //obdc/oledb
            {
                return this.properties[index];
            }
            else
            {
                return this.propertiesList[index];
            }
        }

        /// <summary>
        /// Get a ParameterProperty by his name.
        /// </summary>
        /// <param name="name">The name of the ParameterProperty</param>
        /// <returns>A ParameterProperty</returns>
        public ParameterProperty GetProperty(string name)
        {
            return (ParameterProperty)this.propertiesMap[name];
        }


        /// <summary>
        /// Add a ParameterProperty to the ParameterProperty list.
        /// </summary>
        /// <param name="property"></param>
        public void AddParameterProperty(ParameterProperty property)
        {
            // These mappings will replace any mappings that this map 
            // had for any of the keys currently in the specified map. 
            this.propertiesMap[property.PropertyName] = property;
            this.properties.Add(property);

            if (propertiesList.Contains(property) == false)
            {
                this.propertiesList.Add(property);
            }
        }

        /// <summary>
        /// Insert a ParameterProperty in the ParameterProperty list at the specified index..
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which ParameterProperty should be inserted. 
        /// </param>
        /// <param name="property">The ParameterProperty to insert. </param>
        public void InsertParameterProperty(int index, ParameterProperty property)
        {
            // These mappings will replace any mappings that this map 
            // had for any of the keys currently in the specified map. 
            this.propertiesMap[property.PropertyName] = property;
            this.properties.Insert(index, property);

            if (propertiesList.Contains(property) == false)
            {
                this.propertiesList.Insert(index, property);
            }
        }

        /// <summary>
        /// Retrieve the index for array property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public int GetParameterIndex(string propertyName)
        {
            int _index = -1;
            //_index = (Integer) parameterMappingIndex.get(propertyName);
            _index = Convert.ToInt32(propertyName.Replace("[", "").Replace("]", ""));
            return _index;
        }


        /// <summary>
        /// Get all Parameter Property Name 
        /// </summary>
        /// <returns>A string array</returns>
        public string[] GetPropertyNameArray()
        {
            string[] _propertyNameArray = new string[this.propertiesMap.Count];

            for (int index = 0; index < this.propertiesList.Count; index++)
            {
                _propertyNameArray[index] = this.propertiesList[index].PropertyName;
            }
            return _propertyNameArray;
        }


        /// <summary>
        /// Set parameter value, replace the null value if any.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataParameter"></param>
        /// <param name="parameterValue"></param>
        public void SetParameter(ParameterProperty mapping, IDataParameter dataParameter, object parameterValue)
        {
            object _value = this.dataExchange.GetData(mapping, parameterValue);

            ITypeHandler _typeHandler = mapping.TypeHandler;

            // Apply Null Value
            if (mapping.HasNullValue)
            {
                if (_typeHandler.Equals(_value, mapping.NullValue))
                {
                    _value = null;
                }
            }

            _typeHandler.SetParameter(dataParameter, _value, mapping.DbType);
        }

        /// <summary>
        /// Set output parameter value.
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataBaseValue"></param>
        /// <param name="target"></param>
        public void SetOutputParameter(ref object target, ParameterProperty mapping, object dataBaseValue)
        {
            this.dataExchange.SetData(ref target, mapping, dataBaseValue);
        }

        #region Configuration

        /// <summary>
        /// Initialize the parameter properties child.
        /// </summary>
        /// <param name="aConfigScope"></param>
        /// <param name="usePositionalParameters"></param>
        public void Initialize(bool usePositionalParameters, IScope aConfigScope)
        {
            this.usePositionalParameters = usePositionalParameters;
            if (this.className.Length > 0)
            {
                this.parameterClass = this.dataExchangeFactory.TypeHandlerFactory.GetType(className);
                this.dataExchange = this.dataExchangeFactory.GetDataExchangeForClass(parameterClass);
            }
            else
            {
                // Get the ComplexDataExchange
                this.dataExchange = this.dataExchangeFactory.GetDataExchangeForClass(null);
            }
        }


        /// <summary>
        /// Get the parameter properties child for the xmlNode parameter.
        /// </summary>
        /// <param name="aConfigScope"></param>
        public void BuildProperties(ConfigurationScope aConfigScope)
        {
            ParameterProperty _property = null;

            foreach (XmlNode parameterNode in aConfigScope.NodeContext.SelectNodes(DomSqlMapBuilder.ApplyMappingNamespacePrefix(XML_PARAMATER), aConfigScope.XmlNamespaceManager))
            {
                _property = ParameterPropertyDeSerializer.Deserialize(parameterNode, aConfigScope);

                _property.Initialize(aConfigScope, this.parameterClass);

                AddParameterProperty(_property);
            }
        }

        #endregion

        #endregion

    }
}
