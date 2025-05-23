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
        /// 是否允许 remap 动态 SQL 的结果
        /// </summary>
        [XmlAttribute("remapResults")]
        public bool AllowRemapping
        {
            get { return allowRemapping; }
            set { allowRemapping = value; }
        }

        /// <summary>
        /// 扩展自某个 Statement，可复用配置
        /// </summary>
        [XmlAttribute("extends")]
        public virtual string ExtendStatement
        {
            get { return extendStatement; }
            set { extendStatement = value; }
        }

        /// <summary>
        /// 要使用的缓存模型名
        /// </summary>
        [XmlAttribute("cacheModel")]
        public string CacheModelName
        {
            get { return cacheModelName; }
            set { cacheModelName = value; }
        }

        /// <summary>
        /// 是否已指定缓存模型
        /// </summary>
        [XmlIgnore]
        public bool HasCacheModel
        {
            get { return cacheModelName.Length > 0; }
        }

        /// <summary>
        /// 关联的 CacheModel 实例
        /// </summary>
        [XmlIgnore]
        public CacheModel CacheModel
        {
            get { return cacheModel; }
            set { cacheModel = value; }
        }

        /// <summary>
        /// 指定返回集合的类型名称，如 System.Collections.Generic.List`1
        /// </summary>
        [XmlAttribute("listClass")]
        public string ListClassName
        {
            get { return listClassName; }
            set { listClassName = value; }
        }


        /// <summary>
        /// 返回集合的类型（Type 对象）
        /// </summary>
        [XmlIgnore]
        public Type ListClass
        {
            get { return listClass; }
        }

        /// <summary>
        /// 结果对象类型名称
        /// </summary>
        [XmlAttribute("resultClass")]
        public string ResultClassName
        {
            get { return resultClassName; }
            set { resultClassName = value; }
        }

        /// <summary>
        /// 结果对象类型（Type 对象）
        /// </summary>
        [XmlIgnore]
        public Type ResultClass
        {
            get { return resultClass; }
        }

        /// <summary>
        /// 参数对象的类型名称
        /// </summary>
        [XmlAttribute("parameterClass")]
        public string ParameterClassName
        {
            get { return parameterClassName; }
            set { parameterClassName = value; }
        }

        /// <summary>
        /// 参数类型（Type 对象）
        /// </summary>
        [XmlIgnore]
        public Type ParameterClass
        {
            get { return parameterClass; }
        }

        /// <summary>
        /// 句的唯一标识符
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
        /// 语句的 SQL 内容对象
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
        /// resultMap 的名称，用于结果映射
        /// </summary>
        [XmlAttribute("resultMap")]
        public string ResultMapName
        {
            get { return resultMapName; }
            set { resultMapName = value; }
        }

        /// <summary>
        /// 参数映射名称
        /// </summary>
        [XmlAttribute("parameterMap")]
        public string ParameterMapName
        {
            get { return parameterMapName; }
            set { parameterMapName = value; }
        }

        /// <summary>
        /// 结果映射集合（支持多个 resultMap）
        /// </summary>
        [XmlIgnore]
        public ResultMapCollection ResultsMap
        {
            get { return resultsMap; }
        }

        /// <summary>
        /// 参数映射对象
        /// </summary>
        [XmlIgnore]
        public ParameterMap ParameterMap
        {
            get { return parameterMap; }
            set { parameterMap = value; }
        }

        /// <summary>
        /// 命令类型，默认为 Text（SQL 语句）
        /// </summary>
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
                // 支持多个 resultMap 名称
                string[] _names = this.resultMapName.Split(',');
                for (int i = 0; i < _names.Length; i++)
                {
                    // 添加命名空间前缀
                    string _name = aConfigurationScope.ApplyNamespace(_names[i].Trim());
                    // 加入 resultMap 集合
                    this.resultsMap.Add(aConfigurationScope.SqlMapper.GetResultMap(_name));
                }
            }
            if (this.parameterMapName.Length > 0)
            {
                // 获取参数映射
                this.parameterMap = aConfigurationScope.SqlMapper.GetParameterMap(this.parameterMapName);
            }
            if (this.resultClassName.Length > 0)
            {
                this.resultClass = aConfigurationScope.SqlMapper.TypeHandlerFactory.GetType(this.resultClassName);
                IFactory _resultClassFactory = null;
                if (Type.GetTypeCode(this.resultClass) == TypeCode.Object && (this.resultClass.IsValueType == false))
                {
                    _resultClassFactory = aConfigurationScope.SqlMapper.ObjectFactory.CreateFactory(this.resultClass, Type.EmptyTypes);
                }
                IDataExchange _dataExchange = aConfigurationScope.DataExchangeFactory.GetDataExchangeForClass(this.resultClass);
                IResultMap _autoMap = new AutoResultMap(this.resultClass, _resultClassFactory, _dataExchange);
                this.resultsMap.Add(_autoMap);
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
