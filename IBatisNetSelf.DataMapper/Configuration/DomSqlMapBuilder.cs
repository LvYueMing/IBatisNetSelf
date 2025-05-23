using IBatisNetSelf.Common;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Alias;
using IBatisNetSelf.DataMapper.Configuration.Cache;
using IBatisNetSelf.DataMapper.Configuration.Cache.Fifo;
using IBatisNetSelf.DataMapper.Configuration.Cache.Lru;
using IBatisNetSelf.DataMapper.Configuration.Cache.Memory;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Configuration.Serializers;
using IBatisNetSelf.DataMapper.Configuration.Sql;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNetSelf.DataMapper.Configuration.Sql.SimpleDynamic;
using IBatisNetSelf.DataMapper.Configuration.Sql.Static;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace IBatisNetSelf.DataMapper.Configuration
{
    /// <summary>
    /// 构建一个 ISqlMapper 实例，使用提供的资源（如 XML 配置文件）
    /// </summary>
    public class DomSqlMapBuilder
    {
        #region Embedded resource

        // 允许作为嵌入资源的文件类型：
        // - slqMap.config [是]
        // - providers.config [是]
        // - sqlMap 文件 [是]
        // - properties 文件（如 Database.config）[是]
        // 参见相关贡献、NHibernate 的用法，
        // http://www.codeproject.com/csharp/EmbeddedResourceStrings.asp
        // http://www.devhood.com/tutorials/tutorial_details.aspx?tutorial_id=75
        #endregion

        #region Constant
        // 属性元素中 key 属性的常量名
        private const string PROPERTY_ELEMENT_KEY_ATTR = "key";
        // 属性元素中 value 属性的常量名
        private const string PROPERTY_ELEMENT_VALUE_ATTR = "value";


        // DataMapper 命名空间前缀常量
        private const string DATAMAPPER_NAMESPACE_PREFIX = "mapper";
        // Providers 命名空间前缀常量
        private const string PROVIDERS_NAMESPACE_PREFIX = "provider";
        // Mapping 命名空间前缀常量
        private const string MAPPING_NAMESPACE_PREFIX = "mapping";
        // iBATIS DataMapper 的 XML 命名空间
        private const string DATAMAPPER_XML_NAMESPACE = "http://ibatis.apache.org/dataMapper";
        // iBATIS Providers 的 XML 命名空间
        private const string PROVIDER_XML_NAMESPACE = "http://ibatis.apache.org/providers";
        // iBATIS Mapping 的 XML 命名空间
        private const string MAPPING_XML_NAMESPACE = "http://ibatis.apache.org/mapping";

        /// <summary>
        /// 主配置文件默认文件名
        /// </summary>
        public const string DEFAULT_FILE_CONFIG_NAME = "SqlMap.config";

        /// <summary>
        /// 默认驱动程序名称
        /// </summary>
        private const string DEFAULT_PROVIDER_NAME = "_DEFAULT_PROVIDER_NAME";

        /// <summary>
        /// 用于表示点的常量（如命名空间或层次路径）
        /// </summary>
        public const string DOT = ".";

        /// <summary>
        ///  SqlMapConfig 的 XML 根元素名称
        /// </summary>
        private const string XML_DATAMAPPER_CONFIG_ROOT = "sqlMapConfig";

        /// <summary>
        /// 配置中 settings 节点路径
        /// </summary>
        private const string XML_CONFIG_SETTINGS = "sqlMapConfig/settings/setting";

        /// <summary>
        /// 默认的 providers 配置文件名称
        /// </summary>
        private const string PROVIDERS_FILE_NAME = "providers.config";

        /// <summary>
        /// 配置中 providers 节点路径
        /// </summary>
        private const string XML_CONFIG_PROVIDERS = "sqlMapConfig/providers";

        /// <summary>
        /// properties 元素的 XML 路径
        /// </summary>
        private const string XML_PROPERTIES = "properties";

        /// <summary>
        /// 单个 property 元素的 XML 路径
        /// </summary>
        private const string XML_PROPERTY = "property";

        /// <summary>
        /// settings 元素中 add 子节点路径
        /// </summary>
        private const string XML_SETTINGS_ADD = "/*/add";

        /// <summary>
        /// 全局 properties 的路径（如 configScope 中）
        /// </summary>
        private const string XML_GLOBAL_PROPERTIES = "*/add";

        /// <summary>
        /// provider 元素的 XML 路径
        /// </summary>
        private const string XML_PROVIDER = "providers/provider";

        /// <summary>
        /// database 中 provider 元素的路径
        /// </summary>
        private const string XML_DATABASE_PROVIDER = "sqlMapConfig/database/provider";

        /// <summary>
        /// database 中 dataSource 元素的路径
        /// </summary>
        private const string XML_DATABASE_DATASOURCE = "sqlMapConfig/database/dataSource";

        /// <summary>
        /// 全局类型别名的路径
        /// </summary>
        private const string XML_GLOBAL_TYPEALIAS = "sqlMapConfig/alias/typeAlias";

        /// <summary>
        /// 全局类型处理器的路径
        /// </summary>
        private const string XML_GLOBAL_TYPEHANDLER = "sqlMapConfig/typeHandlers/typeHandler";

        /// <summary>
        ///sqlMap 文件元素路径
        /// </summary>
        private const string XML_SQLMAP = "sqlMapConfig/sqlMaps/sqlMap";

        /// <summary>
        /// 映射文件的根节点名称
        /// </summary>
        private const string XML_MAPPING_ROOT = "sqlMap";

        /// <summary>
        /// 类型别名节点路径
        /// </summary>
        private const string XML_TYPEALIAS = "sqlMap/alias/typeAlias";

        /// <summary>
        /// 结果映射 resultMap 元素路径
        /// </summary>
        private const string XML_RESULTMAP = "sqlMap/resultMaps/resultMap";

        /// <summary>
        /// 参数映射 parameterMap 元素路径
        /// </summary>
        private const string XML_PARAMETERMAP = "sqlMap/parameterMaps/parameterMap";

        /// <summary>
        /// 静态 SQL 元素路径
        /// </summary>
        private const string SQL_STATEMENT = "sqlMap/statements/sql";

        /// <summary>
        ///通用语句元素路径
        /// </summary>
        private const string XML_STATEMENT = "sqlMap/statements/statement";

        /// <summary>
        /// select 语句路径
        /// </summary>
        private const string XML_SELECT = "sqlMap/statements/select";

        /// <summary>
        /// insert 语句路径
        /// </summary>
        private const string XML_INSERT = "sqlMap/statements/insert";

        /// <summary>
        /// selectKey（返回主键）元素路径
        /// </summary>
        private const string XML_SELECTKEY = "selectKey";

        /// <summary>
        /// update 语句路径
        /// </summary>
        private const string XML_UPDATE = "sqlMap/statements/update";

        /// <summary>
        /// delete 语句路径
        /// </summary>
        private const string XML_DELETE = "sqlMap/statements/delete";

        /// <summary>
        /// 存储过程语句路径.
        /// </summary>
        private const string XML_PROCEDURE = "sqlMap/statements/procedure";

        /// <summary>
        /// 缓存模型节点路径
        /// </summary>
        private const string XML_CACHE_MODEL = "sqlMap/cacheModels/cacheModel";

        /// <summary>
        /// flushOnExecute 节点路径
        /// </summary>
        private const string XML_FLUSH_ON_EXECUTE = "flushOnExecute";

        /// <summary>
        /// 搜索语句节点路径
        /// </summary>
        private const string XML_SEARCH_STATEMENT = "sqlMap/statements";

        /// <summary>
        /// 搜索参数映射路径，带 ID 属性匹配
        /// </summary>
        private const string XML_SEARCH_PARAMETER = "sqlMap/parameterMaps/parameterMap[@id='";

        /// <summary>
        /// 搜索结果映射路径，带 ID 属性匹配
        /// </summary>
        private const string XML_SEARCH_RESULTMAP = "sqlMap/resultMaps/resultMap[@id='";

        /// <summary>
        /// 是否启用语句命名空间的属性名
        /// </summary>
        private const string ATR_USE_STATEMENT_NAMESPACES = "useStatementNamespaces";

        /// <summary>
        /// 是否启用缓存模型的属性名
        /// </summary>
        private const string ATR_CACHE_MODELS_ENABLED = "cacheModelsEnabled";

        /// <summary>
        /// 是否验证 SqlMap 配置的属性名
        /// </summary>
        private const string ATR_VALIDATE_SQLMAP = "validateSqlMap";

        /// <summary>
        /// 是否启用反射优化器的属性名
        /// </summary>
        private const string ATR_USE_REFLECTION_OPTIMIZER = "useReflectionOptimizer";

        #endregion

        #region Fields

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // 配置作用域对象
        private ConfigurationScope configScope = null;
        // 反序列化工厂对象
        private DeSerializerFactory deSerializerFactory = null;
        // 内联参数解析器
        private InlineParameterMapParser inlineParamMapParser = null;
        // 对象工厂接口
        private IObjectFactory objectFactory = null;
        // Set 访问器工厂
        private ISetAccessorFactory setAccessorFactory = null;
        // Get 访问器工厂
        private IGetAccessorFactory getAccessorFactory = null;
        // ISqlMapper 接口实例
        private ISqlMapper sqlMapper = null;
        // 是否验证 SqlMap 配置标志位
        private bool validateSqlMapConfig = true;

        #endregion

        #region Properties

        /// <summary>
        /// 在配置之前，允许设置属性
        /// </summary>
        public NameValueCollection Properties
        {
            set { configScope.Properties.Add(value); }
        }

        /// <summary>
        /// 在配置之前，允许设置自定义的 SetAccessorFactory
        /// </summary>
        public ISetAccessorFactory SetAccessorFactory
        {
            set { setAccessorFactory = value; }
        }

        /// <summary>
        /// 在配置之前，允许设置自定义的 GetAccessorFactory
        /// </summary>
        public IGetAccessorFactory GetAccessorFactory
        {
            set { getAccessorFactory = value; }
        }

        /// <summary>
        /// 在配置之前，允许设置自定义的对象工厂
        /// </summary>
        public IObjectFactory ObjectFactory
        {
            set { objectFactory = value; }
        }

        /// <summary>
        /// 在配置之前，允许设置自定义的 SqlMapper 实例
        /// </summary>
        public ISqlMapper SqlMapper
        {
            set { sqlMapper = value; }
        }

        /// <summary>
        /// 是否启用 SqlMap 配置文件验证（需在配置之前设置）
        /// </summary>
        public bool ValidateSqlMapConfig
        {
            set { validateSqlMapConfig = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a DomSqlMapBuilder.
        /// </summary>
        public DomSqlMapBuilder()
        {
            // 创建一个 ConfigurationScope 实例，用于管理整个 SQL 映射的配置作用域（如属性、设置、映射文件等）。
            configScope = new ConfigurationScope();
            // 创建一个 InlineParameterMapParser 实例，用于解析内联参数映射（例如 SQL 中使用的 "#property#"）。
            inlineParamMapParser = new InlineParameterMapParser();
            // 创建一个反序列化器工厂实例，并传入 configScope，以便根据配置生成合适的 XML 解析器。
            deSerializerFactory = new DeSerializerFactory(configScope);
        }

        #endregion

        #region Configure

        /// <summary>
        /// 使用默认的资源文件 'SqlMap.config' 配置一个 SqlMapper。
        /// </summary>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        /// <remarks>
        /// 文件路径是相对于应用程序根目录的。对于 ASP.Net 应用，该文件应与 Web.config 在同一目录；
        /// 对于其他 .Net 应用，该文件应与可执行文件在同一目录。
        /// </remarks>
        public ISqlMapper Configure()
        {
            // 使用默认配置文件名 "SqlMap.config" 从嵌入资源中加载配置 XML 文档，并调用重载方法进行构建。
            return Configure(Resources.GetConfigAsXmlDocument(DEFAULT_FILE_CONFIG_NAME));
        }

        /// <summary>
        /// 使用提供的 XML 配置文档配置并返回一个 ISqlMapper 实例。
        /// </summary>
        /// <param name="document">一个 XML 格式的 sql map 配置文档。</param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper Configure(XmlDocument document)
        {
            // 调用 Build 方法来构建 SqlMapper 实例，第二个参数 false 表示不是解析嵌入式 sqlMap。
            return Build(document, false);
        }


        /// <summary>
        /// 通过文件路径配置一个 ISqlMapper 实例。
        /// </summary>
        /// <param name="resource">
        /// 一个相对于应用程序根目录的路径，或一个绝对路径（如：file:\\c:\dir\a.config）
        /// </param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper Configure(string resource)
        {
            XmlDocument document;

            // 如果路径以 "file://" 开头，则是一个绝对路径文件 URL，需要截去前缀并读取本地文件
            if (resource.StartsWith("file://"))
            {
                document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7)); // 移除 "file://" 前缀
            }
            else
            {
                // 否则作为嵌入资源路径处理
                document = Resources.GetResourceAsXmlDocument(resource);
            }

            // 使用 XML 文档进行构建
            return Build(document, false);
        }

        /// <summary>
        /// 从流中配置 ISqlMapper 对象。
        /// </summary>
        /// <param name="resource">一个表示 XML 配置的 Stream 对象。</param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper Configure(Stream resource)
        {
            // 从 Stream 中读取并解析为 XmlDocument
            XmlDocument document = Resources.GetStreamAsXmlDocument(resource);

            // 构建 SqlMapper 实例，false 表示不是 sqlMap 映射文件
            return Build(document, false);
        }

        /// <summary>
        /// 从 FileInfo 中配置 ISqlMapper 对象。
        /// </summary>
        /// <param name="resource">一个表示 XML 配置文件的 FileInfo 对象。</param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper Configure(FileInfo resource)
        {
            // 从 FileInfo 中读取并解析为 XmlDocument
            XmlDocument document = Resources.GetFileInfoAsXmlDocument(resource);

            // 构建 SqlMapper 实例
            return Build(document, false);
        }

        /// <summary>
        /// 从 Uri 中配置 ISqlMapper 对象。
        /// </summary>
        /// <param name="resource">一个表示远程或本地地址的 Uri 对象。</param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper Configure(Uri resource)
        {
            // 从 Uri 指定的位置获取 XML 配置并解析为 XmlDocument
            XmlDocument document = Resources.GetUriAsXmlDocument(resource);

            // 构建 SqlMapper 实例
            return Build(document, false);
        }

        /// <summary>
        /// 配置并监控默认配置文件（SqlMap.config）的变更，自动重新配置 SqlMapper。
        /// </summary>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper ConfigureAndWatch(ConfigureHandler configureDelegate)
        {
            // 使用默认配置文件名并附带一个配置委托进行监控和构建
            return ConfigureAndWatch(DEFAULT_FILE_CONFIG_NAME, configureDelegate);
        }

        /// <summary>
        /// 配置并监控配置文件的变更，在配置文件修改后自动重新配置 ISqlMapper 实例。
        /// </summary>
        /// <param name="resource">
        /// 相对路径（相对于应用根目录）或绝对路径（如 file:\\c:\dir\a.config）
        /// </param>
        ///<param name="configureDelegate">
        /// 当配置文件发生变化时触发的委托方法。
        /// </param>
        /// <returns>返回一个 ISqlMapper 实例。</returns>
        public ISqlMapper ConfigureAndWatch(string resource, ConfigureHandler configureDelegate)
        {
            XmlDocument document = null;

            // 判断资源路径是否以 "file://" 开头（即为绝对路径）
            if (resource.StartsWith("file://"))
            {
                // 去除前缀 "file://" 并从绝对路径读取 XML 文档
                document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
            }
            else
            {
                // 从嵌入资源或相对路径中读取 XML 文档
                document = Resources.GetResourceAsXmlDocument(resource);
            }

            // 清除之前监控的文件列表，防止重复监控
            ConfigWatcherHandler.ClearFilesMonitored();

            // 将当前配置文件加入监控列表
            ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(resource));

            // 创建配置文件变更时的回调方法
            TimerCallback callBakDelegate = new TimerCallback(OnConfigFileChange);

            // 初始化回调状态对象，用于传递配置信息和回调委托
            StateConfig state = new StateConfig();
            state.FileName = resource;
            state.ConfigureHandler = configureDelegate;

            // 生成 SqlMapper 实例，true 表示在构建中启用额外配置选项（如监控）
            ISqlMapper sqlMapper = Build(document, true);

            // 启动一个新的监控处理器来监听配置文件变化
            new ConfigWatcherHandler(callBakDelegate, state);

            // 返回配置好的 SqlMapper 实例
            return sqlMapper;
        }


        /// <summary>
        /// 配置并监控指定的配置文件，一旦修改自动重新配置 ISqlMapper 实例。
        /// </summary>
        /// <param name="aResource">指向 SqlMap.config 文件的 FileInfo 对象。</param>
        /// <param name="aConfigureDelegate">配置文件变更时要调用的委托。</param>
        /// <returns>返回配置好的 ISqlMapper 实例。</returns>
        public ISqlMapper ConfigureAndWatch(FileInfo aResource, ConfigureHandler aConfigureDelegate)
        {
            // 从 FileInfo 读取 XML 配置文档
            XmlDocument _document = Resources.GetFileInfoAsXmlDocument(aResource);

            // 清除以前监控的所有文件，确保只监控当前配置文件
            ConfigWatcherHandler.ClearFilesMonitored();

            // 添加当前文件到监控列表中
            ConfigWatcherHandler.AddFileToWatch(aResource);

            // 创建回调函数指针，变更时将调用 OnConfigFileChange 方法
            TimerCallback _callBakDelegate = new TimerCallback(OnConfigFileChange);

            // 构造回调状态对象，封装配置文件名和处理方法
            StateConfig _state = new StateConfig();
            _state.FileName = aResource.FullName;
            _state.ConfigureHandler = aConfigureDelegate;

            // 构建 SqlMapper 实例，并启用监控标志
            ISqlMapper _sqlMapper = Build(_document, true);

            // 启动配置监控处理器（监听文件变化）
            new ConfigWatcherHandler(_callBakDelegate, _state);

            // 返回构建好的 Mapper
            return _sqlMapper;
        }

        /// <summary>
        /// 当配置文件发生更改时调用的回调方法。
        /// </summary>
        /// <param name="obj">传入的对象应为 StateConfig 类型。</param>
        public static void OnConfigFileChange(object obj)
        {
            // 将传入对象转换为 StateConfig 类型
            StateConfig _state = (StateConfig)obj;

            // 调用配置回调委托（传入 null，通常意味着触发重新加载）
            _state.ConfigureHandler(null);
        }


        #endregion


        #region Methods
 
        /// <summary>
        /// 加载语句（select、insert、update、delete）、参数（parameters）和结果映射（resultMaps）。
        /// </summary>
        /// <param name="document">Xml 配置文档。</param>
        /// <param name="dataSource">指定的数据源（可选）。</param>
        /// <param name="useConfigFileWatcher">是否启用配置文件监控。</param>
        /// <param name="properties">额外的配置信息（NameValueCollection 格式）。</param>
        /// <returns>返回 ISqlMapper 实例。</returns>
        /// <remarks>该方法通常由 DAO 层调用。</remarks>
        public ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, NameValueCollection properties)
        {
            // 将传入的属性合并到配置作用域（ConfigurationScope）中
            configScope.Properties.Add(properties);
            // 调用重载方法，开始构建 SqlMapper
            return Build(document, dataSource, useConfigFileWatcher, true);
        }

        /// <summary>
        /// 从 XmlDocument 加载 SqlMap 配置。
        /// </summary>
        /// <param name="document">SQL 映射配置的 XML 文档。</param>
        /// <param name="useConfigFileWatcher">是否启用配置文件监控。</param>
        /// <returns>返回 ISqlMapper 实例。</returns>
        public ISqlMapper Build(XmlDocument document, bool useConfigFileWatcher)
        {
            // 传入 null 数据源，构建过程不包含数据源；不使用附加属性。
            return Build(document, null, useConfigFileWatcher, false);
        }


        /// <summary>
        /// 构建一个 ISqlMapper 实例。
        /// </summary>
        /// <param name="document">SQL 映射配置的 XML 文档。</param>
        /// <param name="dataSource">指定的数据源。</param>
        /// <param name="useConfigFileWatcher">是否启用配置文件监控功能。</param>
        /// <param name="isCallFromDao">是否由 DAO 层调用。</param>
        /// <returns>返回构建完成的 ISqlMapper 实例。</returns>
        private ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, bool isCallFromDao)
        {
            // 设置配置作用域的 SQL 映射配置文档
            configScope.SqlMapConfigDocument = document;
            // 设置配置作用域的数据源
            configScope.DataSource = dataSource;
            // 标记是否由 DAO 层发起调用
            configScope.IsCallFromDao = isCallFromDao;
            // 设置是否启用配置文件监听功能
            configScope.UseConfigFileWatcher = useConfigFileWatcher;

            // 创建一个 XmlNamespaceManager，用于管理 XML 命名空间
            configScope.XmlNamespaceManager = new XmlNamespaceManager(configScope.SqlMapConfigDocument.NameTable);
            //AddNamespace (string prefix, string uri) 将给定的命名空间添加到集合
            //prefix:与命名空间关联的前缀。 使用 String.Empty 来添加默认命名空间。
            //uri:要添加的命名空间
            configScope.XmlNamespaceManager.AddNamespace(DATAMAPPER_NAMESPACE_PREFIX, DATAMAPPER_XML_NAMESPACE);
            configScope.XmlNamespaceManager.AddNamespace(PROVIDERS_NAMESPACE_PREFIX, PROVIDER_XML_NAMESPACE);
            configScope.XmlNamespaceManager.AddNamespace(MAPPING_NAMESPACE_PREFIX, MAPPING_XML_NAMESPACE);

            try
            {
                // 如果启用了配置验证，使用 XSD 验证配置文档结构是否正确
                if (validateSqlMapConfig)
                {
                    ValidateSchema(document.ChildNodes[1], "SqlMapConfig.xsd");
                }

                // 初始化配置（包括解析配置文件、创建映射、注册处理器等）
                Initialize();
                // 返回最终构建好的 SqlMapper 实例
                return configScope.SqlMapper;
            }
            catch (Exception e)
            {
                throw new IBatisConfigException(configScope.ErrorContext.ToString(), e);
            }
        }

        /// <summary>
        /// 使用指定的架构文件验证一个 XmlNode 节点的合法性。
        /// </summary>
        /// <param name="aSection">要验证的 XML 节点（通常是整个 SqlMap 配置文档的根节点）。</param>
        /// <param name="aSchemaFileName">用于验证的 XSD 架构文件名。</param>
        private void ValidateSchema(XmlNode aSection, string aSchemaFileName)
        {
            XmlReader? _validatingReader = null;   // 用于 XML 验证的读取器
            Stream? _xsdStream = null;             // XSD 架构文件的流

            // 设置错误上下文信息，便于后续异常处理时使用
            configScope.ErrorContext.Activity = "Validate SqlMap config";

            try
            {
                // 加载嵌入式资源（XSD 文件）作为验证流
                _xsdStream = GetStream(aSchemaFileName);

                if (_xsdStream == null)
                {
                    // 如果找不到 XSD 文件，抛出异常（提示可能是构建设置问题）
                    throw new IBatisConfigException(
                        "Unable to locate embedded resource [IBatisNetSelf.DataMapper." + aSchemaFileName +
                        "]. If you are building from source, verify the file is marked as an embedded resource.");
                }

                // 从 XSD 流中读取架构对象
                XmlSchema? _schema = XmlSchema.Read(_xsdStream, new ValidationEventHandler(ValidationCallBack));

                // 设置 XML 读取器的验证选项
                XmlReaderSettings _readerSettings = new XmlReaderSettings();
                _readerSettings.ValidationType = ValidationType.Schema;  // 指定验证类型为 XSD
                _readerSettings.ValidationEventHandler += ValidationCallBack;  // 注册验证回调
                if (_schema != null)
                {
                    // 添加读取到的架构
                    _readerSettings.Schemas.Add(_schema);
                }

                // 使用指定设置创建用于验证的 XmlReader
                _validatingReader = XmlReader.Create(
                    new XmlTextReader(new StringReader(aSection.OuterXml)),
                    _readerSettings);

                // 实际执行验证过程：遍历整个 XML 文档
                while (_validatingReader.Read()) { }

                // 如果配置作用域标记 XML 非法，则抛出异常
                if (!configScope.IsXmlValid)
                {
                    throw new IBatisConfigException(
                        "Invalid SqlMap.config document. cause :" + configScope.ErrorContext.Resource);
                }
            }
            finally
            {
                // 无论是否出错，都需要关闭读取器和流，释放资源
                if (_validatingReader != null) _validatingReader.Close();
                if (_xsdStream != null) _xsdStream.Close();
            }
        }



        /// <summary>
        /// 在验证 XML 时触发的回调，用于处理验证错误或警告。
        /// </summary>
        /// <param name="sender">触发事件的对象（通常是 XmlReader）。</param>
        /// <param name="args">包含验证错误信息的事件参数。</param>
        private void ValidationCallBack(object? sender, ValidationEventArgs args)
        {
            // 一旦发现验证错误，则设置为无效配置
            configScope.IsXmlValid = false;

            // 将错误信息追加到错误上下文中，便于后续报错时输出详细信息
            configScope.ErrorContext.Resource += args.Message + Environment.NewLine;
        }



        /// <summary>
        /// 重置 PreparedStatements 的缓存。
        /// </summary>
        /// <remarks>
        /// 当前方法为空，可能预留给未来实现缓存重置功能。
        /// </remarks>
        private void Reset()
        {
            // TODO: 实现 PreparedStatements 缓存的重置逻辑
        }


        /// <summary>
        /// Intialize the internal ISqlMapper instance.
        /// </summary>
        private void Initialize()
        {
            // 重置当前配置作用域的状态
            Reset();

            Stopwatch _sw = null;
            Stopwatch _swTotal = null;

            // 如果启用了调试日志，则开始计时器
            if (logger.IsDebugEnabled)
            {
                logger.Debug($"SqlMapper 开始初始化！");
                _swTotal = Stopwatch.StartNew();
                _sw = Stopwatch.StartNew();
            }

            #region 读取全局属性（来自 sqlmap.config 的 <properties>） 
            if (configScope.IsCallFromDao == false)
            {
                // 获取 sqlmap.config 中 sqlMapConfig 节点
                string _xPathProperties = ApplyDataMapperNamespacePrefix(XML_DATAMAPPER_CONFIG_ROOT);
                configScope.CurrentNodeContext = configScope.SqlMapConfigDocument.SelectSingleNode(_xPathProperties, configScope.XmlNamespaceManager);

                // 解析全局属性
                ParseGlobalProperties();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global Properties; 耗时： {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region 读取 settings 配置（来自 sqlmap.config 的 <settings>）  
            configScope.ErrorContext.Activity = "loading global settings";

            string _xPathSettings = ApplyDataMapperNamespacePrefix(XML_CONFIG_SETTINGS);
            XmlNodeList? _settings = configScope.SqlMapConfigDocument.SelectNodes(_xPathSettings, configScope.XmlNamespaceManager);

            if (_settings != null)
            {
                // 判断每个 setting 属性是否存在，并读取其值
                foreach (XmlNode _setting in _settings)
                {
                    if (_setting.Attributes[ATR_USE_STATEMENT_NAMESPACES] != null)
                    {
                        string _value = XmlNodeUtils.ParsePropertyTokens(_setting.Attributes[ATR_USE_STATEMENT_NAMESPACES].Value, configScope.Properties);
                        configScope.UseStatementNamespaces = Convert.ToBoolean(_value);
                    }
                    if (_setting.Attributes[ATR_CACHE_MODELS_ENABLED] != null)
                    {
                        string _value = XmlNodeUtils.ParsePropertyTokens(_setting.Attributes[ATR_CACHE_MODELS_ENABLED].Value, configScope.Properties);
                        configScope.IsCacheModelsEnabled = Convert.ToBoolean(_value);
                    }
                    if (_setting.Attributes[ATR_USE_REFLECTION_OPTIMIZER] != null)
                    {
                        string _value = XmlNodeUtils.ParsePropertyTokens(_setting.Attributes[ATR_USE_REFLECTION_OPTIMIZER].Value, configScope.Properties);
                        configScope.UseReflectionOptimizer = Convert.ToBoolean(_value);
                    }
                    if (_setting.Attributes[ATR_VALIDATE_SQLMAP] != null)
                    {
                        string _value = XmlNodeUtils.ParsePropertyTokens(_setting.Attributes[ATR_VALIDATE_SQLMAP].Value, configScope.Properties);
                        configScope.ValidateSqlMap = Convert.ToBoolean(_value);
                    }
                }
            }
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Loading global settings; 耗时： {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #endregion

            #region 初始化对象工厂、访问器工厂等
            if (this.objectFactory == null)
            {
                this.objectFactory = new ObjectFactory(configScope.UseReflectionOptimizer);
            }
            if (this.setAccessorFactory == null)
            {
                this.setAccessorFactory = new SetAccessorFactory(configScope.UseReflectionOptimizer);
            }
            if (this.getAccessorFactory == null)
            {
                this.getAccessorFactory = new GetAccessorFactory(configScope.UseReflectionOptimizer);
            }
            if (this.sqlMapper == null)
            {
                AccessorFactory _accessorFactory = new AccessorFactory(this.setAccessorFactory, this.getAccessorFactory);
                configScope.SqlMapper = new SqlMapper(this.objectFactory, _accessorFactory);
            }
            else
            {
                configScope.SqlMapper = this.sqlMapper;
            }

            // 添加一个空的参数映射
            ParameterMap _emptyParameterMap = new ParameterMap(configScope.DataExchangeFactory);
            _emptyParameterMap.Id = ConfigurationScope.EMPTY_PARAMETER_MAP;
            configScope.SqlMapper.AddParameterMap(_emptyParameterMap);

            configScope.SqlMapper.IsCacheModelsEnabled = configScope.IsCacheModelsEnabled;

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Create ObjectFactory、SetAccessorFactory、GetAccessorFactory、SqlMapper Object; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #endregion

            #region 初始化缓存控制器别名

            // 添加各种缓存控制器的别名，如 MEMORY、LRU、FIFO
            TypeAlias _cacheAlias = new TypeAlias(typeof(MemoryCacheControler));
            _cacheAlias.AliasName = "MEMORY";
            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.AliasName, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(LruCacheController));
            _cacheAlias.AliasName = "LRU";
            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.AliasName, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(FifoCacheController));
            _cacheAlias.AliasName = "FIFO";
            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.AliasName, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(AnsiStringTypeHandler));
            _cacheAlias.AliasName = "AnsiStringTypeHandler";
            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.AliasName, _cacheAlias);

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Cache Alias Init; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #endregion

            #region 加载数据库驱动程序 （来自 sqlmap.config 的 <providers>）
            if (configScope.IsCallFromDao == false)
            {
                GetProviders();
            }
            #endregion

            #region 加载数据库配置（Provider 和 DataSource）

            #region 选择数据库驱动程序
            IDbProvider _provider = null;
            if (configScope.IsCallFromDao == false)
            {
                _provider = ParseProvider();
                configScope.ErrorContext.Reset();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load providers; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region 加载数据库配置（来自 sqlmap.config 的 <database>）
            configScope.ErrorContext.Activity = "loading Database DataSource";
            XmlNode _nodeDataSource = configScope.SqlMapConfigDocument.SelectSingleNode(ApplyDataMapperNamespacePrefix(XML_DATABASE_DATASOURCE), configScope.XmlNamespaceManager);

            if (_nodeDataSource == null)
            {
                if (configScope.IsCallFromDao == false)
                {
                    throw new IBatisConfigException("There's no dataSource tag in SqlMap.config.");
                }
                else  // patch from Luke Yang
                {
                    configScope.SqlMapper.DataSource = configScope.DataSource;
                }
            }
            else
            {
                if (configScope.IsCallFromDao == false)
                {
                    configScope.ErrorContext.Resource = _nodeDataSource.OuterXml.ToString();
                    configScope.ErrorContext.MoreInfo = "parse DataSource";

                    DataSource _dataSource = DataSourceDeSerializer.Deserialize(_nodeDataSource);

                    _dataSource.DbProvider = _provider;
                    _dataSource.ConnectionString = XmlNodeUtils.ParsePropertyTokens(_dataSource.ConnectionString, configScope.Properties);

                    configScope.DataSource = _dataSource;
                    configScope.SqlMapper.DataSource = configScope.DataSource;
                }
                else
                {
                    configScope.SqlMapper.DataSource = configScope.DataSource;
                }
                configScope.ErrorContext.Reset();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load the DataSources; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #endregion 

            #region 加载全局 TypeAlias （来自 sqlmap.config 的 <alias>）
            foreach (XmlNode xmlNode in configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_GLOBAL_TYPEALIAS), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.Activity = "loading global Type alias";
                TypeAliasDeSerializer.Deserialize(xmlNode, configScope);
            }
            configScope.ErrorContext.Reset();
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global TypeAlias; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region 加载 TypeHandler （来自 sqlmap.config 的 <typeHandlers>）
            foreach (XmlNode xmlNode in configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_GLOBAL_TYPEHANDLER), configScope.XmlNamespaceManager))
            {
                try
                {
                    configScope.ErrorContext.Activity = "loading typeHandler";
                    TypeHandlerDeSerializer.Deserialize(xmlNode, configScope);
                }
                catch (Exception e)
                {
                    NameValueCollection prop = XmlNodeUtils.ParseAttributes(xmlNode, configScope.Properties);

                    throw new IBatisConfigException(
                        String.Format("Error registering TypeHandler class \"{0}\" for handling .Net type \"{1}\" and dbType \"{2}\". Cause: {3}",
                        XmlNodeUtils.GetStringAttribute(prop, "callback"),
                        XmlNodeUtils.GetStringAttribute(prop, "type"),
                        XmlNodeUtils.GetStringAttribute(prop, "dbType"),
                        e.Message), e);
                }
            }
            configScope.ErrorContext.Reset();

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global TypeAlias; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region 加载 SQL 映射文件 （来自 sqlmap.config 的 <sqlMaps>） 
            foreach (XmlNode xmlNode in configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_SQLMAP), configScope.XmlNamespaceManager))
            {
                configScope.CurrentNodeContext = xmlNode;
                ConfigureSqlMap();
            }
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load sqlMap mapping files; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region  绑定缓存模型（设置缓存对象的引用）
            if (configScope.IsCacheModelsEnabled)
            {
                foreach (DictionaryEntry entry in configScope.SqlMapper.MappedStatements)
                {
                    configScope.ErrorContext.Activity = "Set CacheModel to statement";

                    IMappedStatement _mappedStatement = (IMappedStatement)entry.Value;
                    if (_mappedStatement.Statement.CacheModelName.Length > 0)
                    {
                        configScope.ErrorContext.MoreInfo = "statement : " + _mappedStatement.Statement.Id;
                        configScope.ErrorContext.Resource = "cacheModel : " + _mappedStatement.Statement.CacheModelName;
                        _mappedStatement.Statement.CacheModel = configScope.SqlMapper.GetCache(_mappedStatement.Statement.CacheModelName);
                    }
                }
            }
            configScope.ErrorContext.Reset();

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Attach CacheModel to statement; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region 注册缓存模型的刷新触发器（FlushOnExecute）
            // 为缓存模型注册触发器语句（即当执行某语句时触发缓存刷新）
            foreach (DictionaryEntry entry in configScope.CacheModelFlushOnExecuteStatements)
            {
                string cacheModelId = (string)entry.Key;
                IList statementsToRegister = (IList)entry.Value;

                if (statementsToRegister != null && statementsToRegister.Count > 0)
                {
                    foreach (string statementName in statementsToRegister)
                    {
                        IMappedStatement mappedStatement = configScope.SqlMapper.MappedStatements[statementName] as IMappedStatement;

                        if (mappedStatement != null)
                        {
                            // 获取对应的缓存模型
                            CacheModel _cacheModel = configScope.SqlMapper.GetCache(cacheModelId);

                            if (logger.IsDebugEnabled)
                            {
                                logger.Debug("Registering trigger statement [" + mappedStatement.Id + "] to cache model [" + _cacheModel.Id + "]");
                            }
                            // 注册该语句为缓存模型的触发器语句
                            _cacheModel.RegisterTriggerStatement(mappedStatement);
                        }
                        else
                        {
                            if (logger.IsWarnEnabled)
                            {
                                logger.Warn("Unable to register trigger statement [" + statementName + "] to cache model [" + cacheModelId + "]. Statement does not exist.");
                            }
                        }
                    }
                }
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Register Trigger Statements for Cache Models; 耗时：{_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region  解析嵌套 ResultMap（映射关系中的嵌套结构）
            // 解析 ResultMap 中的嵌套 resultMap、Discriminator 和 属性/参数 策略
            foreach (DictionaryEntry entry in configScope.SqlMapper.ResultMaps)
            {
                configScope.ErrorContext.Activity = "Resolve 'resultMap' attribute on Result Property";

                ResultMap _resultMap = (ResultMap)entry.Value;
                // 处理 ResultProperty 属性上的嵌套 resultMap 和策略
                for (int index = 0; index < _resultMap.Properties.Count; index++)
                {
                    ResultProperty result = _resultMap.Properties[index];
                    if (result.NestedResultMapName.Length > 0)
                    {
                        result.NestedResultMap = configScope.SqlMapper.GetResultMap(result.NestedResultMapName);
                    }
                    result.PropertyStrategy = PropertyStrategyFactory.Get(result);
                }
                // 处理构造函数参数上的嵌套 resultMap 和策略
                for (int index = 0; index < _resultMap.ConstructorParams.Count; index++)
                {
                    ResultProperty result = _resultMap.ConstructorParams[index];
                    if (result.NestedResultMapName.Length > 0)
                    {
                        result.NestedResultMap = configScope.SqlMapper.GetResultMap(result.NestedResultMapName);
                    }
                    result.ArgumentStrategy = ArgumentStrategyFactory.Get((ArgumentProperty)result);
                }
                // 初始化 Discriminator（用来做多态映射）
                if (_resultMap.Discriminator != null)
                {
                    _resultMap.Discriminator.Initialize(configScope);
                }
            }

            configScope.ErrorContext.Reset();
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Resolve resultMap / Discriminator / PropertyStategy attributes on Result/Argument Property; 耗时：{_sw.ElapsedMilliseconds} ms");

                _swTotal.Stop();
                logger.Debug($"SqlMapper 初始化完成; 总计耗时： {_swTotal.ElapsedMilliseconds} ms");
            }
            #endregion
        }


        /// <summary>
        /// 从指定的配置文件中加载并初始化数据库提供者（Providers）。
        /// </summary>
        private void GetProviders()
        {
            IDbProvider _provider;
            XmlDocument _xmlProviders;

            // 设置当前错误上下文的活动描述，用于错误追踪
            configScope.ErrorContext.Activity = "loading Providers";

            // 从 SqlMap 配置文件中查找 <providers> 节点，使用命名空间管理器
            XmlNode providersNode = configScope.SqlMapConfigDocument.SelectSingleNode(ApplyDataMapperNamespacePrefix(XML_CONFIG_PROVIDERS), configScope.XmlNamespaceManager);

            if (providersNode != null)
            {
                // 如果 <providers> 节点存在，则加载该节点及其子节点的 XmlDocument
                _xmlProviders = Resources.GetSubfileAsXmlDocument(providersNode, configScope.Properties);
            }
            else
            {
                // 如果不存在，则加载默认的 Providers 配置文件（例如 Providers.config）
                _xmlProviders = Resources.GetConfigAsXmlDocument(PROVIDERS_FILE_NAME);
            }

            // 遍历 <provider> 节点
            foreach (XmlNode node in _xmlProviders.SelectNodes(ApplyProviderNamespacePrefix(XML_PROVIDER), configScope.XmlNamespaceManager))
            {
                // 设置当前处理节点的原始 XML 作为错误资源，便于定位错误
                configScope.ErrorContext.Resource = node.InnerXml.ToString();

                // 反序列化 XML 节点为 IDbProvider 对象
                _provider = ProviderDeSerializer.Deserialize(node);

                // 如果该提供者被启用
                if (_provider.IsEnabled)
                {
                    // 设置错误上下文的对象 ID 和详细信息
                    configScope.ErrorContext.ObjectId = _provider.Name;
                    configScope.ErrorContext.MoreInfo = "initialize provider";

                    // 初始化提供者（建立连接池、驱动加载等）
                    _provider.Initialize();

                    // 将提供者加入配置范围内的 Providers 字典中
                    configScope.Providers.Add(_provider.Name, _provider);

                    // 如果该提供者是默认提供者
                    if (_provider.IsDefault)
                    {
                        // 确认默认提供者字典中还没有默认提供者
                        if (configScope.Providers[DEFAULT_PROVIDER_NAME] == null)
                        {
                            configScope.Providers.Add(DEFAULT_PROVIDER_NAME, _provider);
                        }
                        else
                        {
                            // 抛出异常：配置错误，只能有一个默认提供者
                            throw new IBatisConfigException(
                                string.Format("Error while configuring the Provider named \"{0}\" There can be only one default Provider.", _provider.Name));
                        }
                    }
                }
            }
            // 清空错误上下文
            configScope.ErrorContext.Reset();
        }



        /// <summary>
        /// Parse the provider tag.
        /// </summary>
        /// <returns>A provider object.</returns>
        private IDbProvider ParseProvider()
        {
            configScope.ErrorContext.Activity = "load DataBase Provider";
            XmlNode node = configScope.SqlMapConfigDocument.SelectSingleNode(ApplyDataMapperNamespacePrefix(XML_DATABASE_PROVIDER), configScope.XmlNamespaceManager);

            if (node != null)
            {
                configScope.ErrorContext.Resource = node.OuterXml.ToString();
                string providerName = XmlNodeUtils.ParsePropertyTokens(node.Attributes["name"].Value, configScope.Properties);

                configScope.ErrorContext.ObjectId = providerName;

                if (configScope.Providers.Contains(providerName))
                {
                    return (IDbProvider)configScope.Providers[providerName];
                }
                else
                {
                    throw new IBatisConfigException(
                        string.Format("Error while configuring the Provider named \"{0}\". Cause : The provider is not in 'providers.config' or is not enabled.",
                            providerName));
                }
            }
            else
            {
                if (configScope.Providers.Contains(DEFAULT_PROVIDER_NAME))
                {
                    return (IDbProvider)configScope.Providers[DEFAULT_PROVIDER_NAME];
                }
                else
                {
                    throw new IBatisConfigException(
                        string.Format("Error while configuring the SqlMap. There is no provider marked default in 'providers.config' file."));
                }
            }
        }


        /// <summary>
        /// Load sqlMap statement.
        /// </summary>
        private void ConfigureSqlMap()
        {
            XmlNode _sqlMapNode = this.configScope.CurrentNodeContext;

            this.configScope.ErrorContext.Activity = "loading sqlMap file";
            this.configScope.ErrorContext.Resource = _sqlMapNode.OuterXml.ToString();

            if (configScope.UseConfigFileWatcher)
            {
                if (_sqlMapNode.Attributes["resource"] != null || _sqlMapNode.Attributes["url"] != null)
                {
                    ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(Resources.GetValueOfNodeResourceUrl(_sqlMapNode, configScope.Properties)));
                }
            }

            // Load the file 
            configScope.SqlMapDocument = Resources.GetSubfileAsXmlDocument(_sqlMapNode, configScope.Properties);

            //Validate SqlMap
            if (configScope.ValidateSqlMap)
            {
                ValidateSchema(configScope.SqlMapDocument.ChildNodes[1], "SqlMap.xsd");
            }

            configScope.SqlMapNamespace = configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_MAPPING_ROOT), configScope.XmlNamespaceManager).Attributes["namespace"].Value;

            #region Load TypeAlias

            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_TYPEALIAS), configScope.XmlNamespaceManager))
            {
                TypeAliasDeSerializer.Deserialize(xmlNode, configScope);
            }
            configScope.ErrorContext.MoreInfo = string.Empty;
            configScope.ErrorContext.ObjectId = string.Empty;

            #endregion

            #region Load resultMap

            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_RESULTMAP), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading resultMap tag";
                configScope.CurrentNodeContext = xmlNode; // A ResultMap node

                BuildResultMap();
            }

            #endregion

            #region Load parameterMaps

            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_PARAMETERMAP), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading parameterMap tag";
                configScope.CurrentNodeContext = xmlNode; // A ParameterMap node

                BuildParameterMap();
            }

            #endregion

            #region Load statements

            #region Sql tag  
            //可被其它语句引用的可重用语句块。
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(SQL_STATEMENT), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading sql tag";
                configScope.CurrentNodeContext = xmlNode; // A sql tag

                //存储到 ConfigurationScope.SqlIncludes 集合中
                SqlDeSerializer.Deserialize(xmlNode, configScope);
            }
            #endregion

            #region Statement tag
            Statement _statement;
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_STATEMENT), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading statement tag";
                configScope.CurrentNodeContext = xmlNode; // A statement tag

                _statement = StatementDeSerializer.Deserialize(xmlNode, configScope);
                _statement.CacheModelName = configScope.ApplyNamespace(_statement.CacheModelName);
                _statement.ParameterMapName = configScope.ApplyNamespace(_statement.ParameterMapName);

                if (configScope.UseStatementNamespaces)
                {
                    _statement.Id = configScope.ApplyNamespace(_statement.Id);
                }
                configScope.ErrorContext.ObjectId = _statement.Id;
                _statement.Initialize(configScope);

                // Build ISql (analyse sql statement)		
                ProcessSqlStatement(_statement);

                // Build MappedStatement
                MappedStatement _mappedStatement = new MappedStatement(configScope.SqlMapper, _statement);
                IMappedStatement _mapStatement = _mappedStatement;
                if (_statement.CacheModelName != null && _statement.CacheModelName.Length > 0 && configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }

                configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #region Select tag

            Select _selectSql;

            // 遍历所有 SQL 映射文件中的 <select> 节点
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_SELECT), configScope.XmlNamespaceManager))
            {
                // 设置错误上下文提示，用于异常信息中指出当前处理的是哪部分
                configScope.ErrorContext.MoreInfo = "loading select tag";
                // 当前解析节点上下文设置为当前 select 节点
                configScope.CurrentNodeContext = xmlNode; // A select node

                // 反序列化 select 节点，转换为 Select 对象（包含 id、sql 语句、参数映射、结果映射等）
                _selectSql = SelectDeSerializer.Deserialize(xmlNode, configScope);
                // 将 CacheModelName 和 ParameterMapName 应用命名空间前缀（如果配置开启了命名空间）
                _selectSql.CacheModelName = configScope.ApplyNamespace(_selectSql.CacheModelName);
                _selectSql.ParameterMapName = configScope.ApplyNamespace(_selectSql.ParameterMapName);

                // 如果启用了 statement 命名空间机制，则给当前 select 的 Id 添加命名空间前缀
                if (configScope.UseStatementNamespaces)
                {
                    _selectSql.Id = configScope.ApplyNamespace(_selectSql.Id);
                }
                // 设置错误上下文对象 ID 为当前 select 的 ID（用于报错信息中指明哪个 statement 出错）
                configScope.ErrorContext.ObjectId = _selectSql.Id;

                // 初始化 select 对象（例如解析参数、结果映射等相关逻辑）
                _selectSql.Initialize(configScope);


                // 如果配置了动态 SQL 生成器（<generate> 标签），调用 GenerateCommandText 动态生成 SQL 语句
                if (_selectSql.Generate != null)
                {
                    GenerateCommandText(configScope, _selectSql);
                }
                else
                {	
                    // 否则直接解析静态 SQL，构建 ISql 对象（用于后续执行 SQL）	
                    ProcessSqlStatement(_selectSql);
                }

                // 创建一个 Select 类型的 MappedStatement，绑定 select SQL 的定义与执行行为
                MappedStatement _mappedStatement = new SelectMappedStatement(configScope.SqlMapper, _selectSql);
                // 将其声明为接口类型，方便后续包装处理（如添加缓存）
                IMappedStatement _mapStatement = _mappedStatement;
                // 如果启用了缓存功能，并指定了 CacheModel，则用 CachingStatement 包装原始 MappedStatement
                if (_selectSql.CacheModelName != null && _selectSql.CacheModelName.Length > 0 && configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }
                // 将最终生成的 MappedStatement 注册到 SqlMapper 中，供后续查询执行时使用
                configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #region Insert tag
            Insert _insertSql;
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_INSERT), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading insert tag";
                configScope.CurrentNodeContext = xmlNode; // A insert tag

                MappedStatement _mappedStatement;

                _insertSql = InsertDeSerializer.Deserialize(xmlNode, configScope);
                _insertSql.CacheModelName = configScope.ApplyNamespace(_insertSql.CacheModelName);
                _insertSql.ParameterMapName = configScope.ApplyNamespace(_insertSql.ParameterMapName);

                if (configScope.UseStatementNamespaces)
                {
                    _insertSql.Id = configScope.ApplyNamespace(_insertSql.Id);
                }
                configScope.ErrorContext.ObjectId = _insertSql.Id;
                _insertSql.Initialize(configScope);

                // Build ISql (analyse sql command text)
                if (_insertSql.Generate != null)
                {
                    GenerateCommandText(configScope, _insertSql);
                }
                else
                {
                    ProcessSqlStatement(_insertSql);
                }

                // Build MappedStatement
                _mappedStatement = new InsertMappedStatement(configScope.SqlMapper, _insertSql);

                configScope.SqlMapper.AddMappedStatement(_mappedStatement.Id, _mappedStatement);

                #region statement SelectKey
                // Set sql statement SelectKey 
                if (_insertSql.SelectKey != null)
                {
                    configScope.ErrorContext.MoreInfo = "loading selectKey tag";
                    configScope.CurrentNodeContext = xmlNode.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SELECTKEY), configScope.XmlNamespaceManager);

                    _insertSql.SelectKey.Id = _insertSql.Id;
                    _insertSql.SelectKey.Initialize(configScope);
                    _insertSql.SelectKey.Id += DOT + "SelectKey";

                    // Initialize can also use this.configScope.ErrorContext.ObjectId to get the id
                    // of the parent <select> node
                    // insert.SelectKey.Initialize( this.configScope );
                    // insert.SelectKey.Id = insert.Id + DOT + "SelectKey";

                    ProcessSqlStatement(_insertSql.SelectKey);

                    // Build MappedStatement
                    _mappedStatement = new MappedStatement(configScope.SqlMapper, _insertSql.SelectKey);

                    configScope.SqlMapper.AddMappedStatement(_mappedStatement.Id, _mappedStatement);
                }
                #endregion
            }
            #endregion

            #region Update tag
            Update update;
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_UPDATE), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading update tag";
                configScope.CurrentNodeContext = xmlNode; // A update tag

                MappedStatement mappedStatement;

                update = UpdateDeSerializer.Deserialize(xmlNode, configScope);
                update.CacheModelName = configScope.ApplyNamespace(update.CacheModelName);
                update.ParameterMapName = configScope.ApplyNamespace(update.ParameterMapName);

                if (configScope.UseStatementNamespaces)
                {
                    update.Id = configScope.ApplyNamespace(update.Id);
                }
                configScope.ErrorContext.ObjectId = update.Id;
                update.Initialize(configScope);

                // Build ISql (analyse sql statement)	
                if (update.Generate != null)
                {
                    GenerateCommandText(configScope, update);
                }
                else
                {
                    // Build ISql (analyse sql statement)		
                    ProcessSqlStatement(update);
                }

                // Build MappedStatement
                mappedStatement = new UpdateMappedStatement(configScope.SqlMapper, update);

                configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
            }
            #endregion

            #region Delete tag
            Delete delete;
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_DELETE), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading delete tag";
                configScope.CurrentNodeContext = xmlNode; // A delete tag
                MappedStatement mappedStatement;

                delete = DeleteDeSerializer.Deserialize(xmlNode, configScope);
                delete.CacheModelName = configScope.ApplyNamespace(delete.CacheModelName);
                delete.ParameterMapName = configScope.ApplyNamespace(delete.ParameterMapName);

                if (configScope.UseStatementNamespaces)
                {
                    delete.Id = configScope.ApplyNamespace(delete.Id);
                }
                configScope.ErrorContext.ObjectId = delete.Id;
                delete.Initialize(configScope);

                // Build ISql (analyse sql statement)
                if (delete.Generate != null)
                {
                    GenerateCommandText(configScope, delete);
                }
                else
                {
                    // Build ISql (analyse sql statement)		
                    ProcessSqlStatement(delete);
                }

                // Build MappedStatement
                mappedStatement = new DeleteMappedStatement(configScope.SqlMapper, delete);

                configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
            }
            #endregion

            #region Procedure tag
            Procedure _procedure;
            foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_PROCEDURE), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.MoreInfo = "loading procedure tag";
                configScope.CurrentNodeContext = xmlNode; // A procedure tag

                _procedure = ProcedureDeSerializer.Deserialize(xmlNode, configScope);
                _procedure.CacheModelName = configScope.ApplyNamespace(_procedure.CacheModelName);
                _procedure.ParameterMapName = configScope.ApplyNamespace(_procedure.ParameterMapName);

                if (configScope.UseStatementNamespaces)
                {
                    _procedure.Id = configScope.ApplyNamespace(_procedure.Id);
                }
                configScope.ErrorContext.ObjectId = _procedure.Id;
                _procedure.Initialize(configScope);

                // Build ISql (analyse sql command text)
                ProcessSqlStatement(_procedure);

                // Build MappedStatement
                MappedStatement _mappedStatement = new MappedStatement(configScope.SqlMapper, _procedure);
                IMappedStatement _mapStatement = _mappedStatement;
                if (_procedure.CacheModelName != null && _procedure.CacheModelName.Length > 0 && configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }

                configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #endregion

            #region Load CacheModels

            if (configScope.IsCacheModelsEnabled)
            {
                CacheModel cacheModel;
                foreach (XmlNode xmlNode in configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_CACHE_MODEL), configScope.XmlNamespaceManager))
                {
                    cacheModel = CacheModelDeSerializer.Deserialize(xmlNode, configScope);
                    cacheModel.Id = configScope.ApplyNamespace(cacheModel.Id);

                    // Attach ExecuteEventHandler
                    foreach (XmlNode flushOn in xmlNode.SelectNodes(ApplyMappingNamespacePrefix(XML_FLUSH_ON_EXECUTE), configScope.XmlNamespaceManager))
                    {
                        string statementName = flushOn.Attributes["statement"].Value;
                        if (configScope.UseStatementNamespaces)
                        {
                            statementName = configScope.ApplyNamespace(statementName);
                        }

                        // delay registering statements to cache model until all sqlMap files have been processed
                        IList statementNames = (IList)configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id];
                        if (statementNames == null)
                        {
                            statementNames = new ArrayList();
                        }
                        statementNames.Add(statementName);
                        configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id] = statementNames;
                    }

                    // Get Properties
                    foreach (XmlNode propertie in xmlNode.SelectNodes(ApplyMappingNamespacePrefix(XML_PROPERTY), configScope.XmlNamespaceManager))
                    {
                        string name = propertie.Attributes["name"].Value;
                        string value = propertie.Attributes["value"].Value;

                        cacheModel.AddProperty(name, value);
                    }

                    cacheModel.Initialize();

                    configScope.SqlMapper.AddCache(cacheModel);
                }
            }

            #endregion

            configScope.ErrorContext.Reset();
        }


        /// <summary>
        /// Process the Sql Statement
        /// </summary>
        /// <param name="aStatement"></param>
        private void ProcessSqlStatement(IStatement aStatement)
        {
            bool _isDynamic = false;
            XmlNode _commandTextNode = configScope.CurrentNodeContext;
            DynamicSql _dynamicSql = new DynamicSql(configScope, aStatement);
            StringBuilder _sqlBuffer = new StringBuilder();

            configScope.ErrorContext.MoreInfo = "process the Sql statement";

            //Resolve "extend" attribute on Statement
            if (aStatement.ExtendStatement.Length > 0)
            {
                // Find 'super' statement
                // child:: 定位子节点
                // *	   匹配任意元素
                // @	   属性定位
                // 根元素 sqlmap 开始，选择其下 statements 子元素中的所有直接子元素，但仅限于那些拥有 id 属性且其值等于 aStatement.ExtendStatement 的元素
                XmlNode _supperStatementNode = configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_STATEMENT) + "/child::*[@id='" + aStatement.ExtendStatement + "']", configScope.XmlNamespaceManager);
                if (_supperStatementNode != null)
                {
                    _commandTextNode.InnerXml = _supperStatementNode.InnerXml + _commandTextNode.InnerXml;
                }
                else
                {
                    throw new IBatisConfigException("Unable to find extend statement named '" + aStatement.ExtendStatement + "' on statement '" + aStatement.Id + "'.'");
                }
            }

            configScope.ErrorContext.MoreInfo = "parse dynamic tags on sql statement";

            //是否动态sql,即是否包含sqltag 例如：isEqual
            _isDynamic = ParseDynamicTags(_commandTextNode, _dynamicSql, _sqlBuffer, _isDynamic, false, aStatement);

            if (_isDynamic)
            {
                //动态sql内联ParemeterMap，在请求时生成。
                aStatement.Sql = _dynamicSql;
            }
            else
            {
                string _sql = _sqlBuffer.ToString();
                //根据sql语句中参数定义(#in_praremName#)生成内联ParemeterMap,并赋值给aStatement.ParameterMap
                ApplyInlineParemeterMap(aStatement, _sql);
            }
        }


        /// <summary>
        /// Parse dynamic tags
        /// </summary>
        /// <param name="aCommandTextNode"></param>
        /// <param name="aDynamic"></param>
        /// <param name="aSqlBuffer"></param>
        /// <param name="aIsDynamic">是否包含sqltag 例如：isEqual</param>
        /// <param name="aPostParseRequired"></param>
        /// <param name="aStatement"></param>
        /// <returns>是否动态sql,即是否包含sqltag 例如：isEqual</returns>
        private bool ParseDynamicTags(XmlNode aCommandTextNode, IDynamicParent aDynamic,
            StringBuilder aSqlBuffer, bool aIsDynamic, bool aPostParseRequired, IStatement aStatement)
        {
            XmlNodeList _childrens = aCommandTextNode.ChildNodes;
            int _count = _childrens.Count;
            for (int i = 0; i < _count; i++)
            {
                XmlNode _childXmlNode = _childrens[i];
                if ((_childXmlNode.NodeType == XmlNodeType.CDATA) || (_childXmlNode.NodeType == XmlNodeType.Text))
                {
                    string _text = _childXmlNode.InnerText.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' '); //??

                    _text = XmlNodeUtils.ParsePropertyTokens(_text, configScope.Properties);

                    SqlText _sqlText;
                    if (aPostParseRequired)
                    {
                        _sqlText = new SqlText();
                        _sqlText.Text = _text.ToString();
                    }
                    else
                    {
                        _sqlText = inlineParamMapParser.ParseInlineParameterMap(configScope, null, _text);
                    }

                    aDynamic.AddChild(_sqlText);
                    aSqlBuffer.Append(_text);
                }
                else if (_childXmlNode.Name == "include")
                {
                    NameValueCollection _prop = XmlNodeUtils.ParseAttributes(_childXmlNode, configScope.Properties);
                    string _refid = XmlNodeUtils.GetStringAttribute(_prop, "refid");
                    XmlNode _includeNode = (XmlNode)configScope.SqlIncludes[_refid];

                    if (_includeNode == null)
                    {
                        String _nsrefid = configScope.ApplyNamespace(_refid);
                        _includeNode = (XmlNode)configScope.SqlIncludes[_nsrefid];
                        if (_includeNode == null)
                        {
                            throw new IBatisConfigException("Could not find SQL tag to include with refid '" + _refid + "'");
                        }
                    }
                    aIsDynamic = ParseDynamicTags(_includeNode, aDynamic, aSqlBuffer, aIsDynamic, false, aStatement);
                }
                else
                {
                    string _nodeName = _childXmlNode.Name;
                    IDeSerializer _serializer = deSerializerFactory.GetDeSerializer(_nodeName);

                    if (_serializer != null)
                    {
                        aIsDynamic = true;

                        SqlTag _sqltag = _serializer.Deserialize(_childXmlNode);

                        aDynamic.AddChild(_sqltag);

                        if (_childXmlNode.HasChildNodes)
                        {
                            aIsDynamic = ParseDynamicTags(_childXmlNode, _sqltag, aSqlBuffer, aIsDynamic, _sqltag.Handler.IsPostParseRequired, aStatement);
                        }
                    }
                }
            }

            return aIsDynamic;
        }


        #region Inline Parameter parsing

        /// <summary>
        /// Apply inline paremeterMap
        /// </summary>
        /// <param name="aStatement"></param>
        /// <param name="aSqlStr">sql字符串</param>
        private void ApplyInlineParemeterMap(IStatement aStatement, string aSqlStr)
        {
            string _newSql = aSqlStr;

            configScope.ErrorContext.MoreInfo = "apply inline parameterMap";

            // Check the inline parameter
            if (aStatement.ParameterMap == null)
            {
                // Build a Parametermap with the inline parameters.
                // if they exist. Then delete inline infos from sqltext.

                SqlText _sqlText = inlineParamMapParser.ParseInlineParameterMap(configScope, aStatement, _newSql);

                if (_sqlText.Parameters.Length > 0)
                {
                    ParameterMap _inLineParameterMap = new ParameterMap(configScope.DataExchangeFactory);
                    _inLineParameterMap.Id = aStatement.Id + "-InLineParameterMap";
                    if (aStatement.ParameterClass != null)
                    {
                        _inLineParameterMap.Class = aStatement.ParameterClass;
                    }
                    _inLineParameterMap.Initialize(configScope.DataSource.DbProvider.UsePositionalParameters, configScope);

                    if (aStatement.ParameterClass == null &&
                        _sqlText.Parameters.Length == 1 && _sqlText.Parameters[0].PropertyName == "value")//#value# parameter with no parameterClass attribut
                    {
                        _inLineParameterMap.DataExchange = configScope.DataExchangeFactory.GetDataExchangeForClass(typeof(int));//Get the primitiveDataExchange
                    }
                    aStatement.ParameterMap = _inLineParameterMap;

                    int _lenght = _sqlText.Parameters.Length;
                    for (int index = 0; index < _lenght; index++)
                    {
                        _inLineParameterMap.AddParameterProperty(_sqlText.Parameters[index]);
                    }
                }
                _newSql = _sqlText.Text;
            }

            ISql _sql = null;

            _newSql = _newSql.Trim();

            //简单动态sql,即是否包含$$
            if (SimpleDynamicSql.IsSimpleDynamicSql(_newSql))
            {
                _sql = new SimpleDynamicSql(configScope, _newSql, aStatement);
                //对简单动态sql不进行预处理（BuildPreparedStatement），在请求时再进行
            }
            else
            {
                if (aStatement is Procedure)
                {
                    _sql = new ProcedureSql(configScope, _newSql, aStatement);
                    // Could not call BuildPreparedStatement for procedure because when Unit Test
                    // the database is not here (but in theory procedure must be prepared like statement)
                    // It's even better as we can then switch DataSource.
                    //无法在单元测试中调用 BuildPreparedStatement 来准备存储过程，因为此时数据库并不存在（但理论上，存储过程应该像预编译语句一样准备好）。这样做还有一个好处，就是我们可以切换数据源。
                }
                else if (aStatement is Statement)//select update insert delete 等
                {
                    _sql = new StaticSql(configScope, aStatement);
                    ISqlMapSession _session = new SqlMapSession(configScope.SqlMapper);

                    //1.预先组织好要执行的sql(CommandText)，主要是sql的中参数命名及定位符的确定
                    //2.根据_sql对象的属性statement.ParameterMap,提前创建执行对象需要的参数对象（IDbDataParameter）
                    //3.将结果暂存到_sql的属性preparedStatement对象中
                    ((StaticSql)_sql).BuildPreparedStatement(_session, _newSql); 
                }
            }
            aStatement.Sql = _sql;
        }

        #endregion


        /// <summary>
        /// Initialize the list of variables defined in the
        /// properties file.
        /// </summary>
        private void ParseGlobalProperties()
        {
            configScope.ErrorContext.Activity = "loading global properties";

            string _xPath = ApplyDataMapperNamespacePrefix(XML_PROPERTIES);
            XmlNode _nodeProperties = configScope.CurrentNodeContext.SelectSingleNode(_xPath, configScope.XmlNamespaceManager);

            if (_nodeProperties != null)
            {
                if (_nodeProperties.HasChildNodes)
                {
                    foreach (XmlNode _propertyNode in _nodeProperties.SelectNodes(ApplyDataMapperNamespacePrefix(XML_PROPERTY), configScope.XmlNamespaceManager))
                    {
                        XmlAttribute _keyAttr = _propertyNode.Attributes[PROPERTY_ELEMENT_KEY_ATTR];
                        XmlAttribute _valueAttr = _propertyNode.Attributes[PROPERTY_ELEMENT_VALUE_ATTR];

                        /*    <properties>   
                         *       <property  key="Host" value="localhost"/>
                         *       <property  key="Port" value="1521"/>
                         *     </properties >
                         */
                        if (_keyAttr != null && _valueAttr != null)
                        {
                            configScope.Properties.Add(_keyAttr.Value, _valueAttr.Value);

                            if (logger.IsDebugEnabled)
                            {
                                logger.Debug(string.Format("获取 property \"{0}\" value \"{1}\"", _keyAttr.Value, _valueAttr.Value));
                            }
                        }
                        else
                        {
                            // Load the file defined by the attribute
                            /*例如：  <properties>
	                                    <property  resource="DataBase.config"/>
                                     </properties>
                            */
                            XmlDocument _propertiesConfig = Resources.GetSubfileAsXmlDocument(_propertyNode, configScope.Properties);

                            foreach (XmlNode _node in _propertiesConfig.SelectNodes(XML_GLOBAL_PROPERTIES, configScope.XmlNamespaceManager))
                            {
                                configScope.Properties[_node.Attributes[PROPERTY_ELEMENT_KEY_ATTR].Value] = _node.Attributes[PROPERTY_ELEMENT_VALUE_ATTR].Value;

                                if (logger.IsDebugEnabled)
                                {
                                    logger.Debug(string.Format("获取 property \"{0}\" value \"{1}\"", _node.Attributes[PROPERTY_ELEMENT_KEY_ATTR].Value, _node.Attributes[PROPERTY_ELEMENT_VALUE_ATTR].Value));
                                }
                            }
                        }
                    }
                }
                else
                {
                    // <properties> element's InnerXml is currently an empty string anyway
                    // since <settings> are in properties file
                    //例如：<properties resource="DataBase.config"/>

                    configScope.ErrorContext.Resource = _nodeProperties.OuterXml.ToString();

                    // Load the file defined by the attribute
                    XmlDocument propertiesConfig = Resources.GetSubfileAsXmlDocument(_nodeProperties, configScope.Properties);

                    foreach (XmlNode node in propertiesConfig.SelectNodes(XML_SETTINGS_ADD))
                    {
                        configScope.Properties[node.Attributes[PROPERTY_ELEMENT_KEY_ATTR].Value] = node.Attributes[PROPERTY_ELEMENT_VALUE_ATTR].Value;

                        if (logger.IsDebugEnabled)
                        {
                            logger.Debug(string.Format("Add property \"{0}\" value \"{1}\"", node.Attributes[PROPERTY_ELEMENT_KEY_ATTR].Value, node.Attributes[PROPERTY_ELEMENT_VALUE_ATTR].Value));
                        }
                    }
                }
            }
            configScope.ErrorContext.Reset();
        }



        /// <summary>
        /// Generate the command text for CRUD operation
        /// </summary>
        /// <param name="configScope"></param>
        /// <param name="statement"></param>
        private void GenerateCommandText(ConfigurationScope configScope, IStatement statement)
        {
            string _generatedSQL;

            //------ Build SQL CommandText
            _generatedSQL = SqlGenerator.BuildQuery(statement);

            ISql _sql = new StaticSql(configScope, statement);
            ISqlMapSession _session = new SqlMapSession(configScope.SqlMapper);

            ((StaticSql)_sql).BuildPreparedStatement(_session, _generatedSQL);
            statement.Sql = _sql;
        }


        /// <summary>
        /// Build a ParameterMap
        /// </summary>
        private void BuildParameterMap()
        {
            ParameterMap _parameterMap;
            XmlNode _parameterMapNode = configScope.CurrentNodeContext;

            configScope.ErrorContext.MoreInfo = "Build ParameterMap";

            // Get the parameterMap id
            string _parameterMapId = configScope.ApplyNamespace((_parameterMapNode.Attributes.GetNamedItem("id")).Value);
            configScope.ErrorContext.ObjectId = _parameterMapId;

            // Did we already process it ?
            if (configScope.SqlMapper.ParameterMaps.Contains(_parameterMapId) == false)
            {
                _parameterMap = ParameterMapDeSerializer.Deserialize(_parameterMapNode, configScope);

                _parameterMap.Id = configScope.ApplyNamespace(_parameterMap.Id);
                string _extendMapAttribute = _parameterMap.ExtendMap;
                _parameterMap.ExtendMap = configScope.ApplyNamespace(_parameterMap.ExtendMap);

                //extends属性
                if (_parameterMap.ExtendMap.Length > 0)
                {
                    ParameterMap _superMap;
                    // Did we already build Extend ParameterMap ?
                    if (configScope.SqlMapper.ParameterMaps.Contains(_parameterMap.ExtendMap) == false)
                    {
                        //sqlMap/parameterMaps/parameterMap[@id='value'] 选取属性id='value'的parameterMap节点
                        XmlNode _superNode = configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_PARAMETER) + _extendMapAttribute + "']", configScope.XmlNamespaceManager);

                        if (_superNode != null)
                        {
                            configScope.ErrorContext.MoreInfo = "Build parent ParameterMap";
                            configScope.CurrentNodeContext = _superNode;
                            BuildParameterMap();
                            _superMap = configScope.SqlMapper.GetParameterMap(_parameterMap.ExtendMap);
                        }
                        else
                        {
                            throw new IBatisConfigException("In mapping file '" + configScope.SqlMapNamespace + "' the parameterMap '" + _parameterMap.Id + "' can not resolve extends attribute '" + _parameterMap.ExtendMap + "'");
                        }
                    }
                    else
                    {
                        _superMap = configScope.SqlMapper.GetParameterMap(_parameterMap.ExtendMap);
                    }
                    // Add extends property
                    int index = 0;

                    foreach (string propertyName in _superMap.GetPropertyNameArray())
                    {
                        ParameterProperty _property = _superMap.GetProperty(propertyName).Clone();
                        _property.Initialize(configScope, _parameterMap.Class);
                        _parameterMap.InsertParameterProperty(index, _property);
                        index++;
                    }
                }

                configScope.SqlMapper.AddParameterMap(_parameterMap);
            }
        }


        /// <summary>
        /// Build a ResultMap
        /// </summary>
        private void BuildResultMap()
        {
            ResultMap _resultMap;
            XmlNode _resultMapNode = this.configScope.CurrentNodeContext;

            this.configScope.ErrorContext.MoreInfo = "build ResultMap";

            string _id = this.configScope.ApplyNamespace((_resultMapNode.Attributes.GetNamedItem("id")).Value);
            this.configScope.ErrorContext.ObjectId = _id;

            // Did we alredy process it
            if (this.configScope.SqlMapper.ResultMaps.Contains(_id) == false)
            {
                _resultMap = ResultMapDeSerializer.Deserialize(_resultMapNode, configScope);

                string _attributeExtendMap = _resultMap.ExtendMap;
                _resultMap.ExtendMap = this.configScope.ApplyNamespace(_resultMap.ExtendMap);

                if (_resultMap.ExtendMap != null && _resultMap.ExtendMap.Length > 0)
                {
                    IResultMap _superMap = null;
                    // Did we already build Extend ResultMap?
                    if (this.configScope.SqlMapper.ResultMaps.Contains(_resultMap.ExtendMap) == false)
                    {
                        XmlNode superNode = configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_RESULTMAP) + _attributeExtendMap + "']", configScope.XmlNamespaceManager);

                        if (superNode != null)
                        {
                            configScope.ErrorContext.MoreInfo = "Build parent ResultMap";
                            configScope.CurrentNodeContext = superNode;
                            BuildResultMap();
                            _superMap = configScope.SqlMapper.GetResultMap(_resultMap.ExtendMap);
                        }
                        else
                        {
                            throw new IBatisConfigException("In mapping file '" + configScope.SqlMapNamespace + "' the resultMap '" + _resultMap.Id + "' can not resolve extends attribute '" + _resultMap.ExtendMap + "'");
                        }
                    }
                    else
                    {
                        _superMap = configScope.SqlMapper.GetResultMap(_resultMap.ExtendMap);
                    }

                    // Add parent property
                    for (int index = 0; index < _superMap.Properties.Count; index++)
                    {
                        ResultProperty property = _superMap.Properties[index].Clone();
                        property.Initialize(configScope, _resultMap.Class);
                        _resultMap.Properties.Add(property);
                    }
                    // Add groupBy properties
                    if (_resultMap.GroupByPropertyNames.Count == 0)
                    {
                        for (int i = 0; i < _superMap.GroupByPropertyNames.Count; i++)
                        {
                            _resultMap.GroupByPropertyNames.Add(_superMap.GroupByPropertyNames[i]);
                        }
                    }
                    // Add constructor arguments 
                    if (_resultMap.ConstructorParams.Count == 0)
                    {
                        for (int i = 0; i < _superMap.ConstructorParams.Count; i++)
                        {
                            _resultMap.ConstructorParams.Add(_superMap.ConstructorParams[i]);
                        }
                        if (_resultMap.ConstructorParams.Count > 0)
                        {
                            _resultMap.SetObjectFactory(configScope);
                        }
                    }


                    // Verify that that each groupBy element correspond to a class member
                    // of one of result property
                    for (int i = 0; i < _resultMap.GroupByPropertyNames.Count; i++)
                    {
                        string memberName = _resultMap.GroupByPropertyNames[i];
                        if (!_resultMap.Properties.Contains(memberName))
                        {
                            throw new IBatisConfigException(
                                string.Format(
                                    "Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".",
                                    _resultMap.Id, memberName));
                        }
                    }
                }

                _resultMap.InitializeGroupByProperties();
                configScope.SqlMapper.AddResultMap(_resultMap);
            }
        }


        /// <summary>
        /// Gets a resource stream.
        /// </summary>
        /// <param name="schemaResourceKey">The schema resource key.</param>
        /// <returns>A resource stream.</returns>
        public Stream GetStream(string schemaResourceKey)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("IBatisNetSelf.DataMapper." + schemaResourceKey);
        }


        /// <summary>
        /// Apply the dataMapper namespace prefix
        /// </summary>
        /// <example>参数:sqlMapConfig/providers 结果：mapper:sqlMapConfig/mapper:providers
        /// </example>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public string ApplyDataMapperNamespacePrefix(string elementName)
        {
            return DATAMAPPER_NAMESPACE_PREFIX + ":" + elementName.
                Replace("/", "/" + DATAMAPPER_NAMESPACE_PREFIX + ":");
        }

        /// <summary>
        /// Apply the provider namespace prefix
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public string ApplyProviderNamespacePrefix(string elementName)
        {
            return PROVIDERS_NAMESPACE_PREFIX + ":" + elementName.
                Replace("/", "/" + PROVIDERS_NAMESPACE_PREFIX + ":");
        }

        /// <summary>
        /// Apply the provider namespace prefix
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static string ApplyMappingNamespacePrefix(string elementName)
        {
            return MAPPING_NAMESPACE_PREFIX + ":" + elementName.
                Replace("/", "/" + MAPPING_NAMESPACE_PREFIX + ":");
        }

        #endregion
    }
}
