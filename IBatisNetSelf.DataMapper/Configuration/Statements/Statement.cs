using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Cache;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Configuration.Sql;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for Statement.
    /// </summary>
    [Serializable]
    [XmlRoot("statement", Namespace = "http://ibatis.apache.org/mapping")]
    public class Statement : IStatement
    {

        #region Fields

        [NonSerialized] 
        private bool allowRemapping = false;
        [NonSerialized]
        private string id = string.Empty;
        // ResultMap
        [NonSerialized]
        private string resultMapName = string.Empty;
        [NonSerialized]
        private ResultMapCollection resultsMap = new ResultMapCollection();
        // ParameterMap
        [NonSerialized]
        private string parameterMapName = string.Empty;
        [NonSerialized]
        private ParameterMap parameterMap = null;
        // Result Class
        [NonSerialized]
        private string resultClassName = string.Empty;
        [NonSerialized]
        private Type resultClass = null;
        // Parameter Class
        [NonSerialized]
        private string parameterClassName = string.Empty;
        [NonSerialized]
        private Type parameterClass = null;
        // List Class
        [NonSerialized]
        private string listClassName = string.Empty;
        [NonSerialized]
        private Type listClass = null;
        // CacheModel
        [NonSerialized]
        private string cacheModelName = string.Empty;
        [NonSerialized]
        private CacheModel cacheModel = null;
        [NonSerialized]
        private ISql sql = null;
        [NonSerialized]
        private string extendStatement = string.Empty;
        [NonSerialized]
        private IFactory listClassFactory = null;

        #endregion

        #region Properties

        /// <summary>
        /// Allow remapping of dynamic SQL
        /// </summary>
        [XmlAttribute("remapResults")]
        public bool AllowRemapping
        {
            get { return allowRemapping; }
            set { allowRemapping = value; }
        }

        /// <summary>
        /// Extend statement attribute
        /// </summary>
        [XmlAttribute("extends")]
        public virtual string ExtendStatement
        {
            get { return extendStatement; }
            set { extendStatement = value; }
        }

        /// <summary>
        /// The CacheModel name to use.
        /// </summary>
        [XmlAttribute("cacheModel")]
        public string CacheModelName
        {
            get { return cacheModelName; }
            set { cacheModelName = value; }
        }

        /// <summary>
        /// Tell us if a cacheModel is attached to this statement.
        /// </summary>
        [XmlIgnore]
        public bool HasCacheModel
        {
            get { return cacheModelName.Length > 0; }
        }

        /// <summary>
        /// The CacheModel used by this statement.
        /// </summary>
        [XmlIgnore]
        public CacheModel CacheModel
        {
            get { return cacheModel; }
            set { cacheModel = value; }
        }

        /// <summary>
        /// The list class name to use for strongly typed collection.
        /// </summary>
        [XmlAttribute("listClass")]
        public string ListClassName
        {
            get { return listClassName; }
            set { listClassName = value; }
        }


        /// <summary>
        /// The list class type to use for strongly typed collection.
        /// </summary>
        [XmlIgnore]
        public Type ListClass
        {
            get { return listClass; }
        }

        /// <summary>
        /// The result class name to used.
        /// </summary>
        [XmlAttribute("resultClass")]
        public string ResultClassName
        {
            get { return resultClassName; }
            set { resultClassName = value; }
        }

        /// <summary>
        /// The result class type to used.
        /// </summary>
        [XmlIgnore]
        public Type ResultClass
        {
            get { return resultClass; }
        }

        /// <summary>
        /// The parameter class name to used.
        /// </summary>
        [XmlAttribute("parameterClass")]
        public string ParameterClassName
        {
            get { return parameterClassName; }
            set { parameterClassName = value; }
        }

        /// <summary>
        /// The parameter class type to used.
        /// </summary>
        [XmlIgnore]
        public Type ParameterClass
        {
            get { return parameterClass; }
        }

        /// <summary>
        /// Name used to identify the statement amongst the others.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if ((value == null) || (value.Length < 1))
                    throw new DataMapperException("The id attribute is required in a statement tag.");

                id = value;
            }
        }


        /// <summary>
        /// The sql statement
        /// </summary>
        [XmlIgnore]
        public ISql Sql
        {
            get { return sql; }
            set
            {
                if (value == null)
                    throw new DataMapperException("The sql statement query text is required in the statement tag " + id);

                sql = value;
            }
        }


        /// <summary>
        /// The ResultMaps name used by the statement.
        /// </summary>
        [XmlAttribute("resultMap")]
        public string ResultMapName
        {
            get { return resultMapName; }
            set { resultMapName = value; }
        }

        /// <summary>
        /// The ParameterMap name used by the statement.
        /// </summary>
        [XmlAttribute("parameterMap")]
        public string ParameterMapName
        {
            get { return parameterMapName; }
            set { parameterMapName = value; }
        }

        /// <summary>
        /// The ResultMap used by the statement.
        /// </summary>
        [XmlIgnore]
        public ResultMapCollection ResultsMap
        {
            get { return resultsMap; }
        }

        /// <summary>
        /// The parameterMap used by the statement.
        /// </summary>
        [XmlIgnore]
        public ParameterMap ParameterMap
        {
            get { return parameterMap; }
            set { parameterMap = value; }
        }

        /// <summary>
        /// The type of the statement (text or procedure)
        /// Default Text.
        /// </summary>
        /// <example>Text or StoredProcedure</example>
        [XmlIgnore]
        public virtual CommandType CommandType
        {
            get { return CommandType.Text; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize an statement for the sqlMap.
        /// </summary>
        /// <param name="aConfigurationScope">The scope of the configuration</param>
        internal virtual void Initialize(ConfigurationScope aConfigurationScope)
        {
            if (this.resultMapName.Length > 0)
            {
                string[] _names = this.resultMapName.Split(',');
                for (int i = 0; i < _names.Length; i++)
                {
                    string _name = aConfigurationScope.ApplyNamespace(_names[i].Trim());
                    this.resultsMap.Add(aConfigurationScope.SqlMapper.GetResultMap(_name));
                }
            }
            if (this.parameterMapName.Length > 0)
            {
                this.parameterMap = aConfigurationScope.SqlMapper.GetParameterMap(this.parameterMapName);
            }
            if (this.resultClassName.Length > 0)
            {
                string[] _classNames = this.resultClassName.Split(',');
                for (int i = 0; i < _classNames.Length; i++)
                {
                    this.resultClass = aConfigurationScope.SqlMapper.TypeHandlerFactory.GetType(_classNames[i].Trim());
                    IFactory _resultClassFactory = null;
                    if (Type.GetTypeCode(this.resultClass) == TypeCode.Object &&
                        (this.resultClass.IsValueType == false))
                    {
                        _resultClassFactory = aConfigurationScope.SqlMapper.ObjectFactory.CreateFactory(this.resultClass, Type.EmptyTypes);
                    }
                    IDataExchange _dataExchange = aConfigurationScope.DataExchangeFactory.GetDataExchangeForClass(this.resultClass);
                    IResultMap _autoMap = new AutoResultMap(this.resultClass, _resultClassFactory, _dataExchange);
                    this.resultsMap.Add(_autoMap);
                }
            }
            if (this.parameterClassName.Length > 0)
            {
                this.parameterClass = aConfigurationScope.SqlMapper.TypeHandlerFactory.GetType(this.parameterClassName);
            }
            if (this.listClassName.Length > 0)
            {
                this.listClass = aConfigurationScope.SqlMapper.TypeHandlerFactory.GetType(this.listClassName);
                this.listClassFactory = aConfigurationScope.SqlMapper.ObjectFactory.CreateFactory(this.listClass, Type.EmptyTypes);
            }
        }


        /// <summary>
        /// Create an instance of 'IList' class.
        /// </summary>
        /// <returns>An object which implment IList.</returns>
        public IList CreateInstanceOfListClass()
        {
            return (IList)listClassFactory.CreateInstance(null);
        }

        #endregion

    }
}
