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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace IBatisNetSelf.DataMapper.Configuration
{
    /// <summary>
    /// Builds an ISqlMapper instance from the supplied resources (e.g. XML configuration files).
    /// </summary>
    public class DomSqlMapBuilder
    {
        #region Embedded resource

        // Which files we allow to be used as Embedded Resources ?
        // - slqMap.config [Yes]
        // - providers.config [Yes]
        // - sqlMap files [Yes]
        // - properties file (like Database.config) [Yes]
        // see contribution, NHibernate usage,
        // see http://www.codeproject.com/csharp/EmbeddedResourceStrings.asp
        // see http://www.devhood.com/tutorials/tutorial_details.aspx?tutorial_id=75
        #endregion

        #region Constant

        private const string PROPERTY_ELEMENT_KEY_ATTR = "key";
        private const string PROPERTY_ELEMENT_VALUE_ATTR = "value";

        /// <summary>
        /// 
        /// </summary>
        private const string DATAMAPPER_NAMESPACE_PREFIX = "mapper";
        private const string PROVIDERS_NAMESPACE_PREFIX = "provider";
        private const string MAPPING_NAMESPACE_PREFIX = "mapping";
        private const string DATAMAPPER_XML_NAMESPACE = "http://ibatis.apache.org/dataMapper";
        private const string PROVIDER_XML_NAMESPACE = "http://ibatis.apache.org/providers";
        private const string MAPPING_XML_NAMESPACE = "http://ibatis.apache.org/mapping";

        /// <summary>
        /// Default filename of main configuration file.
        /// </summary>
        public const string DEFAULT_FILE_CONFIG_NAME = "SqlMap.config";

        /// <summary>
        /// Default provider name
        /// </summary>
        private const string DEFAULT_PROVIDER_NAME = "_DEFAULT_PROVIDER_NAME";

        /// <summary>
        /// Dot representation.
        /// </summary>
        public const string DOT = ".";

        /// <summary>
        /// Token for SqlMapConfig xml root element.
        /// </summary>
        private const string XML_DATAMAPPER_CONFIG_ROOT = "sqlMapConfig";

        /// <summary>
        /// Token for xml path to SqlMapConfig settings element.
        /// </summary>
        private const string XML_CONFIG_SETTINGS = "sqlMapConfig/settings/setting";

        /// <summary>
        /// Token for default providers config file name.
        /// </summary>
        private const string PROVIDERS_FILE_NAME = "providers.config";

        /// <summary>
        /// Token for xml path to SqlMapConfig providers element.
        /// </summary>
        private const string XML_CONFIG_PROVIDERS = "sqlMapConfig/providers";

        /// <summary>
        /// Token for xml path to properties elements.
        /// </summary>
        private const string XML_PROPERTIES = "properties";

        /// <summary>
        /// Token for xml path to property elements.
        /// </summary>
        private const string XML_PROPERTY = "property";

        /// <summary>
        /// Token for xml path to settings add elements.
        /// </summary>
        private const string XML_SETTINGS_ADD = "/*/add";

        /// <summary>
        /// Token for xml path to global properties elements.
        /// </summary>
        private const string XML_GLOBAL_PROPERTIES = "*/add";

        /// <summary>
        /// Token for xml path to provider elements.
        /// </summary>
        private const string XML_PROVIDER = "providers/provider";

        /// <summary>
        /// Token for xml path to database provider elements.
        /// </summary>
        private const string XML_DATABASE_PROVIDER = "sqlMapConfig/database/provider";

        /// <summary>
        /// Token for xml path to database source elements.
        /// </summary>
        private const string XML_DATABASE_DATASOURCE = "sqlMapConfig/database/dataSource";

        /// <summary>
        /// Token for xml path to global type alias elements.
        /// </summary>
        private const string XML_GLOBAL_TYPEALIAS = "sqlMapConfig/alias/typeAlias";

        /// <summary>
        /// Token for xml path to global type alias elements.
        /// </summary>
        private const string XML_GLOBAL_TYPEHANDLER = "sqlMapConfig/typeHandlers/typeHandler";

        /// <summary>
        /// Token for xml path to sqlMap elements.
        /// </summary>
        private const string XML_SQLMAP = "sqlMapConfig/sqlMaps/sqlMap";

        /// <summary>
        /// Token for mapping xml root.
        /// </summary>
        private const string XML_MAPPING_ROOT = "sqlMap";

        /// <summary>
        /// Token for xml path to type alias elements.
        /// </summary>
        private const string XML_TYPEALIAS = "sqlMap/alias/typeAlias";

        /// <summary>
        /// Token for xml path to resultMap elements.
        /// </summary>
        private const string XML_RESULTMAP = "sqlMap/resultMaps/resultMap";

        /// <summary>
        /// Token for xml path to parameterMap elements.
        /// </summary>
        private const string XML_PARAMETERMAP = "sqlMap/parameterMaps/parameterMap";

        /// <summary>
        /// Token for xml path to sql elements.
        /// </summary>
        private const string SQL_STATEMENT = "sqlMap/statements/sql";

        /// <summary>
        /// Token for xml path to statement elements.
        /// </summary>
        private const string XML_STATEMENT = "sqlMap/statements/statement";

        /// <summary>
        /// Token for xml path to select elements.
        /// </summary>
        private const string XML_SELECT = "sqlMap/statements/select";

        /// <summary>
        /// Token for xml path to insert elements.
        /// </summary>
        private const string XML_INSERT = "sqlMap/statements/insert";

        /// <summary>
        /// Token for xml path to selectKey elements.
        /// </summary>
        private const string XML_SELECTKEY = "selectKey";

        /// <summary>
        /// Token for xml path to update elements.
        /// </summary>
        private const string XML_UPDATE = "sqlMap/statements/update";

        /// <summary>
        /// Token for xml path to delete elements.
        /// </summary>
        private const string XML_DELETE = "sqlMap/statements/delete";

        /// <summary>
        /// Token for xml path to procedure elements.
        /// </summary>
        private const string XML_PROCEDURE = "sqlMap/statements/procedure";

        /// <summary>
        /// Token for xml path to cacheModel elements.
        /// </summary>
        private const string XML_CACHE_MODEL = "sqlMap/cacheModels/cacheModel";

        /// <summary>
        /// Token for xml path to flushOnExecute elements.
        /// </summary>
        private const string XML_FLUSH_ON_EXECUTE = "flushOnExecute";

        /// <summary>
        /// Token for xml path to search statement elements.
        /// </summary>
        private const string XML_SEARCH_STATEMENT = "sqlMap/statements";

        /// <summary>
        /// Token for xml path to search parameterMap elements.
        /// </summary>
        private const string XML_SEARCH_PARAMETER = "sqlMap/parameterMaps/parameterMap[@id='";

        /// <summary>
        /// Token for xml path to search resultMap elements.
        /// </summary>
        private const string XML_SEARCH_RESULTMAP = "sqlMap/resultMaps/resultMap[@id='";

        /// <summary>
        /// Token for useStatementNamespaces attribute.
        /// </summary>
        private const string ATR_USE_STATEMENT_NAMESPACES = "useStatementNamespaces";
        /// <summary>
        /// Token for cacheModelsEnabled attribute.
        /// </summary>
        private const string ATR_CACHE_MODELS_ENABLED = "cacheModelsEnabled";

        /// <summary>
        /// Token for validateSqlMap attribute.
        /// </summary>
        private const string ATR_VALIDATE_SQLMAP = "validateSqlMap";
        /// <summary>
        /// Token for useReflectionOptimizer attribute.
        /// </summary>
        private const string ATR_USE_REFLECTION_OPTIMIZER = "useReflectionOptimizer";

        #endregion

        #region Fields

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ConfigurationScope configScope = null;
        private DeSerializerFactory deSerializerFactory = null;
        private InlineParameterMapParser inlineParamMapParser = null;
        private IObjectFactory objectFactory = null;
        private ISetAccessorFactory setAccessorFactory = null;
        private IGetAccessorFactory getAccessorFactory = null;
        private ISqlMapper sqlMapper = null;
        private bool validateSqlMapConfig = true;

        #endregion

        #region Properties

        /// <summary>
        /// Allow properties to be set before configuration.
        /// </summary>
        public NameValueCollection Properties
        {
            set { configScope.Properties.Add(value); }
        }

        /// <summary>
        /// Allow a custom <see cref="ISetAccessorFactory"/> to be set before configuration.
        /// </summary>
        public ISetAccessorFactory SetAccessorFactory
        {
            set { setAccessorFactory = value; }
        }

        /// <summary>
        /// Allow a custom <see cref="IGetAccessorFactory"/> to be set before configuration.
        /// </summary>
        public IGetAccessorFactory GetAccessorFactory
        {
            set { getAccessorFactory = value; }
        }

        /// <summary>
        /// Allow a custom <see cref="IObjectFactory"/> to be set before configuration.
        /// </summary>
        public IObjectFactory ObjectFactory
        {
            set { objectFactory = value; }
        }

        /// <summary>
        /// Allow a custom <see cref="ISqlMapper"/> to be set before configuration.
        /// </summary>
        public ISqlMapper SqlMapper
        {
            set { sqlMapper = value; }
        }

        /// <summary>
        /// Enable validation of SqlMap document. This property must be set before configuration.
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
            this.configScope = new ConfigurationScope();
            this.inlineParamMapParser = new InlineParameterMapParser();
            this.deSerializerFactory = new DeSerializerFactory(this.configScope);
        }

        #endregion

        #region Configure

        /// <summary>
        /// Configure a SqlMapper from default resource file named 'SqlMap.config'.
        /// </summary>
        /// <returns>An ISqlMapper instance.</returns>
        /// <remarks>
        /// The file path is relative to the application root. For ASP.Net applications
        /// this would be the same directory as the Web.config file. For other .Net
        /// applications the SqlMap.config file should be placed in the same folder
        /// as the executable. 
        /// </remarks>
        public ISqlMapper Configure()
        {
            return Configure(Resources.GetConfigAsXmlDocument(DEFAULT_FILE_CONFIG_NAME));
        }

        /// <summary>
        /// Configure and returns an ISqlMapper instance.
        /// </summary>
        /// <param name="document">An xml sql map configuration document.</param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper Configure(XmlDocument document)
        {
            return Build(document, false);
        }


        /// <summary>
        /// Configure an ISqlMapper object from a file path.
        /// </summary>
        /// <param name="resource">
        /// A relative ressource path from your Application root 
        /// or a absolue file path file:\\c:\dir\a.config
        /// </param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper Configure(string resource)
        {
            XmlDocument document;
            if (resource.StartsWith("file://"))
            {
                document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
            }
            else
            {
                document = Resources.GetResourceAsXmlDocument(resource);
            }
            return Build(document, false);
        }

        /// <summary>
        ///  Configure an ISqlMapper object from a stream.
        /// </summary>
        /// <param name="resource">A Stream resource.</param>
        /// <returns>An SqlMap</returns>
        public ISqlMapper Configure(Stream resource)
        {
            XmlDocument document = Resources.GetStreamAsXmlDocument(resource);
            return Build(document, false);
        }

        /// <summary>
        ///  Configure an ISqlMapper object from a FileInfo.
        /// </summary>
        /// <param name="resource">A FileInfo resource.</param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper Configure(FileInfo resource)
        {
            XmlDocument document = Resources.GetFileInfoAsXmlDocument(resource);
            return Build(document, false);
        }

        /// <summary>
        ///  Configure an ISqlMapper object from an Uri.
        /// </summary>
        /// <param name="resource">A Uri resource.</param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper Configure(Uri resource)
        {
            XmlDocument document = Resources.GetUriAsXmlDocument(resource);
            return Build(document, false);
        }

        /// <summary>
        /// Configure and monitor the default configuration file (SqlMap.config) for modifications 
        /// and automatically reconfigure SqlMap. 
        /// </summary>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper ConfigureAndWatch(ConfigureHandler configureDelegate)
        {
            return ConfigureAndWatch(DEFAULT_FILE_CONFIG_NAME, configureDelegate);
        }

        /// <summary>
        /// Configure and monitor the configuration file for modifications 
        /// and automatically reconfigure the ISqlMapper instance.
        /// </summary>
        /// <param name="resource">
        /// A relative ressource path from your Application root 
        /// or an absolue file path file:\\c:\dir\a.config
        /// </param>
        ///<param name="configureDelegate">
        /// Delegate called when the file has changed.
        /// </param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper ConfigureAndWatch(string resource, ConfigureHandler configureDelegate)
        {
            XmlDocument document = null;
            if (resource.StartsWith("file://"))
            {
                document = Resources.GetUrlAsXmlDocument(resource.Remove(0, 7));
            }
            else
            {
                document = Resources.GetResourceAsXmlDocument(resource);
            }

            ConfigWatcherHandler.ClearFilesMonitored();
            ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(resource));

            TimerCallback callBakDelegate = new TimerCallback(OnConfigFileChange);

            StateConfig state = new StateConfig();
            state.FileName = resource;
            state.ConfigureHandler = configureDelegate;

            ISqlMapper sqlMapper = Build(document, true);

            new ConfigWatcherHandler(callBakDelegate, state);

            return sqlMapper;
        }

        /// <summary>
        /// Configure and monitor the configuration file for modifications 
        /// and automatically reconfigure the ISqlMapper instance.
        /// </summary>
        /// <param name="aResource">
        /// A FileInfo to a SqlMap.config file.
        /// </param>
        ///<param name="aConfigureDelegate">
        /// Delegate called when the file has changed.
        /// </param>
        /// <returns>An ISqlMapper instance.</returns>
        public ISqlMapper ConfigureAndWatch(FileInfo aResource, ConfigureHandler aConfigureDelegate)
        {
            XmlDocument _document = Resources.GetFileInfoAsXmlDocument(aResource);

            ConfigWatcherHandler.ClearFilesMonitored();
            ConfigWatcherHandler.AddFileToWatch(aResource);

            TimerCallback _callBakDelegate = new TimerCallback(OnConfigFileChange);

            StateConfig _state = new StateConfig();
            _state.FileName = aResource.FullName;
            _state.ConfigureHandler = aConfigureDelegate;

            ISqlMapper _sqlMapper = Build(_document, true);

            new ConfigWatcherHandler(_callBakDelegate, _state);

            return _sqlMapper;
        }

        /// <summary>
        /// Callback called when the SqlMap.config file has changed.
        /// </summary>
        /// <param name="obj">The <see cref="StateConfig"/> object.</param>
        public static void OnConfigFileChange(object obj)
        {
            StateConfig _state = (StateConfig)obj;
            _state.ConfigureHandler(null);
        }

        #endregion


        #region Methods

        /// <summary>
        /// Load statements (select, insert, update, delete), parameters, and resultMaps.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="dataSource"></param>
        /// <param name="useConfigFileWatcher"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <remarks>Used by Dao</remarks>
        public ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, NameValueCollection properties)
        {
            this.configScope.Properties.Add(properties);
            return Build(document, dataSource, useConfigFileWatcher, true);
        }

        /// <summary>
        /// Load SqlMap configuration from
        /// from the XmlDocument passed in parameter.
        /// </summary>
        /// <param name="document">The xml sql map configuration.</param>
        /// <param name="useConfigFileWatcher"></param>
        public ISqlMapper Build(XmlDocument document, bool useConfigFileWatcher)
        {
            return Build(document, null, useConfigFileWatcher, false);
        }

        /// <summary>
        /// Build an ISqlMapper instance.
        /// </summary>
        /// <param name="document">An xml configuration document.</param>
        /// <param name="dataSource">A data source.</param>
        /// <param name="useConfigFileWatcher"></param>
        /// <param name="isCallFromDao"></param>
        /// <returns>Returns an ISqlMapper instance.</returns>
        private ISqlMapper Build(XmlDocument document, DataSource dataSource, bool useConfigFileWatcher, bool isCallFromDao)
        {
            this.configScope.SqlMapConfigDocument = document;
            this.configScope.DataSource = dataSource;
            this.configScope.IsCallFromDao = isCallFromDao;
            this.configScope.UseConfigFileWatcher = useConfigFileWatcher;

            this.configScope.XmlNamespaceManager = new XmlNamespaceManager(this.configScope.SqlMapConfigDocument.NameTable);
            //AddNamespace (string prefix, string uri) 将给定的命名空间添加到集合
            //prefix:与命名空间关联的前缀。 使用 String.Empty 来添加默认命名空间。
            //uri:要添加的命名空间
            this.configScope.XmlNamespaceManager.AddNamespace(DATAMAPPER_NAMESPACE_PREFIX, DATAMAPPER_XML_NAMESPACE);
            this.configScope.XmlNamespaceManager.AddNamespace(PROVIDERS_NAMESPACE_PREFIX, PROVIDER_XML_NAMESPACE);
            this.configScope.XmlNamespaceManager.AddNamespace(MAPPING_NAMESPACE_PREFIX, MAPPING_XML_NAMESPACE);

            try
            {
                if (this.validateSqlMapConfig)
                {
                    ValidateSchema(document.ChildNodes[1], "SqlMapConfig.xsd");
                }
                //根据配置文件 初始化
                Initialize();
                return configScope.SqlMapper;
            }
            catch (Exception e)
            {
                throw new ConfigurationException(this.configScope.ErrorContext.ToString(), e);
            }
        }

        /// <summary>
        /// Validates an XmlNode against a schema file.
        /// </summary>
        /// <param name="aSection">The doc to validate.</param>
        /// <param name="aSchemaFileName">Schema file name.</param>
        private void ValidateSchema(XmlNode aSection, string aSchemaFileName)
        {

            XmlReader? _validatingReader = null;
            Stream? _xsdStream = null;

            configScope.ErrorContext.Activity = "Validate SqlMap config";
            try
            {
                //Validate the document using a schema               
                _xsdStream = GetStream(aSchemaFileName);

                if (_xsdStream == null)
                {
                    // TODO: avoid using hard-coded value "IBatisNet.DataMapper"
                    throw new ConfigurationException("Unable to locate embedded resource [IBatisNetSelf.DataMapper." + aSchemaFileName + "]. If you are building from source, verfiy the file is marked as an embedded resource.");
                }

                XmlSchema? _schema = XmlSchema.Read(_xsdStream, new ValidationEventHandler(ValidationCallBack));

                XmlReaderSettings _readerSettings = new XmlReaderSettings();
                _readerSettings.ValidationType= ValidationType.Schema;
                _readerSettings.ValidationEventHandler += ValidationCallBack;
                if (_schema != null)
                {
                    _readerSettings.Schemas.Add(_schema);
                }

                _validatingReader = XmlReader.Create(new XmlTextReader(new StringReader(aSection.OuterXml)), _readerSettings);

                // Validate the document
                while (_validatingReader.Read()) { }

                if (!configScope.IsXmlValid)
                {
                    throw new ConfigurationException("Invalid SqlMap.config document. cause :" + configScope.ErrorContext.Resource);
                }
            }
            finally
            {
                if (_validatingReader != null) _validatingReader.Close();
                if (_xsdStream != null) _xsdStream.Close();
            }
        }


        private void ValidationCallBack(object? sender, ValidationEventArgs args)
        {
            configScope.IsXmlValid = false;
            configScope.ErrorContext.Resource += args.Message + Environment.NewLine;
        }


        /// <summary>
        /// Reset PreparedStatements cache
        /// </summary>
        private void Reset()
        {
        }

        /// <summary>
        /// Intialize the internal ISqlMapper instance.
        /// </summary>
        private void Initialize()
        {
            Reset();

            Stopwatch _sw = null;
            Stopwatch _swTotal = null;

            if (logger.IsDebugEnabled)
            {
                _swTotal = Stopwatch.StartNew();
                _sw = Stopwatch.StartNew();
            }

            #region Load Global Properties
            if (this.configScope.IsCallFromDao == false)
            {
                string _xPathProperties = ApplyDataMapperNamespacePrefix(XML_DATAMAPPER_CONFIG_ROOT);
                this.configScope.CurrentNodeContext = this.configScope.SqlMapConfigDocument.SelectSingleNode(_xPathProperties, this.configScope.XmlNamespaceManager);

                this.ParseGlobalProperties();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global Properties cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Load settings
            configScope.ErrorContext.Activity = "loading global settings";

            string _xPathSettings = ApplyDataMapperNamespacePrefix(XML_CONFIG_SETTINGS);
            XmlNodeList _settings = this.configScope.SqlMapConfigDocument.SelectNodes(_xPathSettings, configScope.XmlNamespaceManager);

            if (_settings != null)
            {
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
                logger.Debug($"Loading global settings cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #endregion

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
                this.configScope.SqlMapper = new SqlMapper(this.objectFactory, _accessorFactory);
            }
            else
            {
                this.configScope.SqlMapper = this.sqlMapper;
            }

            ParameterMap _emptyParameterMap = new ParameterMap(this.configScope.DataExchangeFactory);
            _emptyParameterMap.Id = ConfigurationScope.EMPTY_PARAMETER_MAP;
            this.configScope.SqlMapper.AddParameterMap(_emptyParameterMap);

            this.configScope.SqlMapper.IsCacheModelsEnabled = this.configScope.IsCacheModelsEnabled;

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Factory init cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #region Cache Alias

            TypeAlias _cacheAlias = new TypeAlias(typeof(MemoryCacheControler));
            _cacheAlias.Name = "MEMORY";
            this.configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.Name, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(LruCacheController));
            _cacheAlias.Name = "LRU";
            this.configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.Name, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(FifoCacheController));
            _cacheAlias.Name = "FIFO";
            this.configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.Name, _cacheAlias);

            _cacheAlias = new TypeAlias(typeof(AnsiStringTypeHandler));
            _cacheAlias.Name = "AnsiStringTypeHandler";
            this.configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(_cacheAlias.Name, _cacheAlias);

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"TypeAlias init cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }

            #endregion

            #region Load providers
            if (this.configScope.IsCallFromDao == false)
            {
                this.GetProviders();
            }
            #endregion

            #region Load DataBase

            #region Choose the  provider
            IDbProvider _provider = null;
            if (this.configScope.IsCallFromDao == false)
            {
                _provider = ParseProvider();
                this.configScope.ErrorContext.Reset();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load providers cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Load the DataSources
            this.configScope.ErrorContext.Activity = "loading Database DataSource";
            XmlNode _nodeDataSource = this.configScope.SqlMapConfigDocument.SelectSingleNode(ApplyDataMapperNamespacePrefix(XML_DATABASE_DATASOURCE), configScope.XmlNamespaceManager);

            if (_nodeDataSource == null)
            {
                if (this.configScope.IsCallFromDao == false)
                {
                    throw new ConfigurationException("There's no dataSource tag in SqlMap.config.");
                }
                else  // patch from Luke Yang
                {
                    this.configScope.SqlMapper.DataSource = this.configScope.DataSource;
                }
            }
            else
            {
                if (configScope.IsCallFromDao == false)
                {
                    this.configScope.ErrorContext.Resource = _nodeDataSource.OuterXml.ToString();
                    this.configScope.ErrorContext.MoreInfo = "parse DataSource";

                    DataSource _dataSource = DataSourceDeSerializer.Deserialize(_nodeDataSource);

                    _dataSource.DbProvider = _provider;
                    _dataSource.ConnectionString = XmlNodeUtils.ParsePropertyTokens(_dataSource.ConnectionString, configScope.Properties);

                    this.configScope.DataSource = _dataSource;
                    this.configScope.SqlMapper.DataSource = configScope.DataSource;
                }
                else
                {
                    this.configScope.SqlMapper.DataSource = this.configScope.DataSource;
                }
                this.configScope.ErrorContext.Reset();
            }

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load the DataSources cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion
            #endregion 

            #region Load Global TypeAlias
            foreach (XmlNode xmlNode in this.configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_GLOBAL_TYPEALIAS), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.Activity = "loading global Type alias";
                TypeAliasDeSerializer.Deserialize(xmlNode, this.configScope);
            }
            this.configScope.ErrorContext.Reset();
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global TypeAlias cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Load TypeHandlers
            foreach (XmlNode xmlNode in this.configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_GLOBAL_TYPEHANDLER), this.configScope.XmlNamespaceManager))
            {
                try
                {
                    this.configScope.ErrorContext.Activity = "loading typeHandler";
                    TypeHandlerDeSerializer.Deserialize(xmlNode, this.configScope);
                }
                catch (Exception e)
                {
                    NameValueCollection prop = XmlNodeUtils.ParseAttributes(xmlNode, this.configScope.Properties);

                    throw new ConfigurationException(
                        String.Format("Error registering TypeHandler class \"{0}\" for handling .Net type \"{1}\" and dbType \"{2}\". Cause: {3}",
                        XmlNodeUtils.GetStringAttribute(prop, "callback"),
                        XmlNodeUtils.GetStringAttribute(prop, "type"),
                        XmlNodeUtils.GetStringAttribute(prop, "dbType"),
                        e.Message), e);
                }
            }
            this.configScope.ErrorContext.Reset();

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load Global TypeAlias cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Load sqlMap mapping files
            foreach (XmlNode xmlNode in this.configScope.SqlMapConfigDocument.SelectNodes(ApplyDataMapperNamespacePrefix(XML_SQLMAP), this.configScope.XmlNamespaceManager))
            {
                this.configScope.CurrentNodeContext = xmlNode;
                this.ConfigureSqlMap();
            }
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Load sqlMap mapping files cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Attach CacheModel to statement
            if (this.configScope.IsCacheModelsEnabled)
            {
                foreach (DictionaryEntry entry in this.configScope.SqlMapper.MappedStatements)
                {
                    this.configScope.ErrorContext.Activity = "Set CacheModel to statement";

                    IMappedStatement _mappedStatement = (IMappedStatement)entry.Value;
                    if (_mappedStatement.Statement.CacheModelName.Length > 0)
                    {
                        this.configScope.ErrorContext.MoreInfo = "statement : " + _mappedStatement.Statement.Id;
                        this.configScope.ErrorContext.Resource = "cacheModel : " + _mappedStatement.Statement.CacheModelName;
                        _mappedStatement.Statement.CacheModel = this.configScope.SqlMapper.GetCache(_mappedStatement.Statement.CacheModelName);
                    }
                }
            }
            this.configScope.ErrorContext.Reset();

            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Attach CacheModel to statement cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Register Trigger Statements for Cache Models
            foreach (DictionaryEntry entry in this.configScope.CacheModelFlushOnExecuteStatements)
            {
                string cacheModelId = (string)entry.Key;
                IList statementsToRegister = (IList)entry.Value;

                if (statementsToRegister != null && statementsToRegister.Count > 0)
                {
                    foreach (string statementName in statementsToRegister)
                    {
                        IMappedStatement mappedStatement = this.configScope.SqlMapper.MappedStatements[statementName] as IMappedStatement;

                        if (mappedStatement != null)
                        {
                            CacheModel _cacheModel = this.configScope.SqlMapper.GetCache(cacheModelId);

                            if (logger.IsDebugEnabled)
                            {
                                logger.Debug("Registering trigger statement [" + mappedStatement.Id + "] to cache model [" + _cacheModel.Id + "]");
                            }

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
                logger.Debug($"Register Trigger Statements for Cache Models cost {_sw.ElapsedMilliseconds} ms");
                _sw.Restart();
            }
            #endregion

            #region Resolve resultMap / Discriminator / PropertyStategy attributes on Result/Argument Property 
            foreach (DictionaryEntry entry in this.configScope.SqlMapper.ResultMaps)
            {
                this.configScope.ErrorContext.Activity = "Resolve 'resultMap' attribute on Result Property";

                ResultMap _resultMap = (ResultMap)entry.Value;
                for (int index = 0; index < _resultMap.Properties.Count; index++)
                {
                    ResultProperty result = _resultMap.Properties[index];
                    if (result.NestedResultMapName.Length > 0)
                    {
                        result.NestedResultMap = this.configScope.SqlMapper.GetResultMap(result.NestedResultMapName);
                    }
                    result.PropertyStrategy = PropertyStrategyFactory.Get(result);
                }
                for (int index = 0; index < _resultMap.Parameters.Count; index++)
                {
                    ResultProperty result = _resultMap.Parameters[index];
                    if (result.NestedResultMapName.Length > 0)
                    {
                        result.NestedResultMap = this.configScope.SqlMapper.GetResultMap(result.NestedResultMapName);
                    }
                    result.ArgumentStrategy = ArgumentStrategyFactory.Get((ArgumentProperty)result);
                }
                if (_resultMap.Discriminator != null)
                {
                    _resultMap.Discriminator.Initialize(this.configScope);
                }
            }

            this.configScope.ErrorContext.Reset();
            if (logger.IsDebugEnabled)
            {
                _sw.Stop();
                logger.Debug($"Resolve resultMap / Discriminator / PropertyStategy attributes on Result/Argument Property cost {_sw.ElapsedMilliseconds} ms");

                _swTotal.Stop();
                logger.Debug($"Build Sqlmap cost total {_swTotal.ElapsedMilliseconds} ms");
            }
            #endregion
        }

        /// <summary>
        /// Load and initialize providers from specified file.
        /// </summary>
        private void GetProviders()
        {
            IDbProvider _provider;
            XmlDocument _xmlProviders;

            configScope.ErrorContext.Activity = "loading Providers";

            XmlNode providersNode;
            providersNode = configScope.SqlMapConfigDocument.SelectSingleNode(ApplyDataMapperNamespacePrefix(XML_CONFIG_PROVIDERS), configScope.XmlNamespaceManager);

            if (providersNode != null)
            {
                _xmlProviders = Resources.GetSubfileAsXmlDocument(providersNode, configScope.Properties);
            }
            else
            {
                _xmlProviders = Resources.GetConfigAsXmlDocument(PROVIDERS_FILE_NAME);
            }

            foreach (XmlNode node in _xmlProviders.SelectNodes(ApplyProviderNamespacePrefix(XML_PROVIDER), configScope.XmlNamespaceManager))
            {
                configScope.ErrorContext.Resource = node.InnerXml.ToString();
                _provider = ProviderDeSerializer.Deserialize(node);

                if (_provider.IsEnabled)
                {
                    configScope.ErrorContext.ObjectId = _provider.Name;
                    configScope.ErrorContext.MoreInfo = "initialize provider";

                    _provider.Initialize();
                    configScope.Providers.Add(_provider.Name, _provider);

                    if (_provider.IsDefault)
                    {
                        if (configScope.Providers[DEFAULT_PROVIDER_NAME] == null)
                        {
                            configScope.Providers.Add(DEFAULT_PROVIDER_NAME, _provider);
                        }
                        else
                        {
                            throw new ConfigurationException(
                                string.Format("Error while configuring the Provider named \"{0}\" There can be only one default Provider.", _provider.Name));
                        }
                    }
                }
            }
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
                    throw new ConfigurationException(
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
                    throw new ConfigurationException(
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

            if (this.configScope.UseConfigFileWatcher)
            {
                if (_sqlMapNode.Attributes["resource"] != null || _sqlMapNode.Attributes["url"] != null)
                {
                    ConfigWatcherHandler.AddFileToWatch(Resources.GetFileInfo(Resources.GetValueOfNodeResourceUrl(_sqlMapNode, this.configScope.Properties)));
                }
            }

            // Load the file 
            this.configScope.SqlMapDocument = Resources.GetSubfileAsXmlDocument(_sqlMapNode, this.configScope.Properties);

            if (this.configScope.ValidateSqlMap)
            {
                ValidateSchema(this.configScope.SqlMapDocument.ChildNodes[1], "SqlMap.xsd");
            }

            this.configScope.SqlMapNamespace = this.configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_MAPPING_ROOT), this.configScope.XmlNamespaceManager).Attributes["namespace"].Value;

            #region Load TypeAlias

            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_TYPEALIAS), this.configScope.XmlNamespaceManager))
            {
                TypeAliasDeSerializer.Deserialize(xmlNode, this.configScope);
            }
            this.configScope.ErrorContext.MoreInfo = string.Empty;
            this.configScope.ErrorContext.ObjectId = string.Empty;

            #endregion

            #region Load resultMap

            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_RESULTMAP), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading resultMap tag";
                this.configScope.CurrentNodeContext = xmlNode; // A ResultMap node

                this.BuildResultMap();
            }

            #endregion

            #region Load parameterMaps

            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_PARAMETERMAP), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading parameterMap tag";
                this.configScope.CurrentNodeContext = xmlNode; // A ParameterMap node

                this.BuildParameterMap();
            }

            #endregion

            #region Load statements

            #region Sql tag  
            //可被其它语句引用的可重用语句块。
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(SQL_STATEMENT), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading sql tag";
                this.configScope.CurrentNodeContext = xmlNode; // A sql tag

                SqlDeSerializer.Deserialize(xmlNode, this.configScope);
            }
            #endregion

            #region Statement tag
            Statement _statement;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_STATEMENT), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading statement tag";
                this.configScope.CurrentNodeContext = xmlNode; // A statement tag

                _statement = StatementDeSerializer.Deserialize(xmlNode, this.configScope);
                _statement.CacheModelName = this.configScope.ApplyNamespace(_statement.CacheModelName);
                _statement.ParameterMapName = this.configScope.ApplyNamespace(_statement.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    _statement.Id = this.configScope.ApplyNamespace(_statement.Id);
                }
                this.configScope.ErrorContext.ObjectId = _statement.Id;
                _statement.Initialize(this.configScope);

                // Build ISql (analyse sql statement)		
                this.ProcessSqlStatement(_statement);

                // Build MappedStatement
                MappedStatement _mappedStatement = new MappedStatement(this.configScope.SqlMapper, _statement);
                IMappedStatement _mapStatement = _mappedStatement;
                if (_statement.CacheModelName != null && _statement.CacheModelName.Length > 0 && this.configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }

                this.configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #region Select tag
            Select _select;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_SELECT), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading select tag";
                this.configScope.CurrentNodeContext = xmlNode; // A select node

                _select = SelectDeSerializer.Deserialize(xmlNode, this.configScope);
                _select.CacheModelName = this.configScope.ApplyNamespace(_select.CacheModelName);
                _select.ParameterMapName = this.configScope.ApplyNamespace(_select.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    _select.Id = this.configScope.ApplyNamespace(_select.Id);
                }
                this.configScope.ErrorContext.ObjectId = _select.Id;

                _select.Initialize(this.configScope);

                if (_select.Generate != null)
                {
                    this.GenerateCommandText(this.configScope, _select);
                }
                else
                {
                    //Build ISql (analyse sql statement)		
                    this.ProcessSqlStatement(_select);
                }

                // Build MappedStatement
                MappedStatement _mappedStatement = new SelectMappedStatement(this.configScope.SqlMapper, _select);
                IMappedStatement _mapStatement = _mappedStatement;
                if (_select.CacheModelName != null && _select.CacheModelName.Length > 0 && this.configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }

                this.configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #region Insert tag
            Insert _insert;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_INSERT), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading insert tag";
                this.configScope.CurrentNodeContext = xmlNode; // A insert tag

                MappedStatement mappedStatement;

                _insert = InsertDeSerializer.Deserialize(xmlNode, this.configScope);
                _insert.CacheModelName = this.configScope.ApplyNamespace(_insert.CacheModelName);
                _insert.ParameterMapName = this.configScope.ApplyNamespace(_insert.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    _insert.Id = this.configScope.ApplyNamespace(_insert.Id);
                }
                this.configScope.ErrorContext.ObjectId = _insert.Id;
                _insert.Initialize(this.configScope);

                // Build ISql (analyse sql command text)
                if (_insert.Generate != null)
                {
                    GenerateCommandText(this.configScope, _insert);
                }
                else
                {
                    ProcessSqlStatement(_insert);
                }

                // Build MappedStatement
                mappedStatement = new InsertMappedStatement(this.configScope.SqlMapper, _insert);

                this.configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);

                #region statement SelectKey
                // Set sql statement SelectKey 
                if (_insert.SelectKey != null)
                {
                    this.configScope.ErrorContext.MoreInfo = "loading selectKey tag";
                    this.configScope.CurrentNodeContext = xmlNode.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SELECTKEY), this.configScope.XmlNamespaceManager);

                    _insert.SelectKey.Id = _insert.Id;
                    _insert.SelectKey.Initialize(this.configScope);
                    _insert.SelectKey.Id += DOT + "SelectKey";

                    // Initialize can also use this.configScope.ErrorContext.ObjectId to get the id
                    // of the parent <select> node
                    // insert.SelectKey.Initialize( this.configScope );
                    // insert.SelectKey.Id = insert.Id + DOT + "SelectKey";

                    ProcessSqlStatement(_insert.SelectKey);

                    // Build MappedStatement
                    mappedStatement = new MappedStatement(this.configScope.SqlMapper, _insert.SelectKey);

                    this.configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
                }
                #endregion
            }
            #endregion

            #region Update tag
            Update update;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_UPDATE), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading update tag";
                this.configScope.CurrentNodeContext = xmlNode; // A update tag

                MappedStatement mappedStatement;

                update = UpdateDeSerializer.Deserialize(xmlNode, this.configScope);
                update.CacheModelName = this.configScope.ApplyNamespace(update.CacheModelName);
                update.ParameterMapName = this.configScope.ApplyNamespace(update.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    update.Id = this.configScope.ApplyNamespace(update.Id);
                }
                this.configScope.ErrorContext.ObjectId = update.Id;
                update.Initialize(this.configScope);

                // Build ISql (analyse sql statement)	
                if (update.Generate != null)
                {
                    GenerateCommandText(this.configScope, update);
                }
                else
                {
                    // Build ISql (analyse sql statement)		
                    ProcessSqlStatement(update);
                }

                // Build MappedStatement
                mappedStatement = new UpdateMappedStatement(this.configScope.SqlMapper, update);

                this.configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
            }
            #endregion

            #region Delete tag
            Delete delete;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_DELETE), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading delete tag";
                this.configScope.CurrentNodeContext = xmlNode; // A delete tag
                MappedStatement mappedStatement;

                delete = DeleteDeSerializer.Deserialize(xmlNode, this.configScope);
                delete.CacheModelName = this.configScope.ApplyNamespace(delete.CacheModelName);
                delete.ParameterMapName = this.configScope.ApplyNamespace(delete.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    delete.Id = this.configScope.ApplyNamespace(delete.Id);
                }
                this.configScope.ErrorContext.ObjectId = delete.Id;
                delete.Initialize(this.configScope);

                // Build ISql (analyse sql statement)
                if (delete.Generate != null)
                {
                    GenerateCommandText(this.configScope, delete);
                }
                else
                {
                    // Build ISql (analyse sql statement)		
                    ProcessSqlStatement(delete);
                }

                // Build MappedStatement
                mappedStatement = new DeleteMappedStatement(this.configScope.SqlMapper, delete);

                this.configScope.SqlMapper.AddMappedStatement(mappedStatement.Id, mappedStatement);
            }
            #endregion

            #region Procedure tag
            Procedure _procedure;
            foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_PROCEDURE), this.configScope.XmlNamespaceManager))
            {
                this.configScope.ErrorContext.MoreInfo = "loading procedure tag";
                this.configScope.CurrentNodeContext = xmlNode; // A procedure tag

                _procedure = ProcedureDeSerializer.Deserialize(xmlNode, this.configScope);
                _procedure.CacheModelName = this.configScope.ApplyNamespace(_procedure.CacheModelName);
                _procedure.ParameterMapName = this.configScope.ApplyNamespace(_procedure.ParameterMapName);

                if (this.configScope.UseStatementNamespaces)
                {
                    _procedure.Id = this.configScope.ApplyNamespace(_procedure.Id);
                }
                this.configScope.ErrorContext.ObjectId = _procedure.Id;
                _procedure.Initialize(this.configScope);

                // Build ISql (analyse sql command text)
                this.ProcessSqlStatement(_procedure);

                // Build MappedStatement
                MappedStatement _mappedStatement = new MappedStatement(this.configScope.SqlMapper, _procedure);
                IMappedStatement _mapStatement = _mappedStatement;
                if (_procedure.CacheModelName != null && _procedure.CacheModelName.Length > 0 && this.configScope.IsCacheModelsEnabled)
                {
                    _mapStatement = new CachingStatement(_mappedStatement);
                }

                this.configScope.SqlMapper.AddMappedStatement(_mapStatement.Id, _mapStatement);
            }
            #endregion

            #endregion

            #region Load CacheModels

            if (this.configScope.IsCacheModelsEnabled)
            {
                CacheModel cacheModel;
                foreach (XmlNode xmlNode in this.configScope.SqlMapDocument.SelectNodes(ApplyMappingNamespacePrefix(XML_CACHE_MODEL), this.configScope.XmlNamespaceManager))
                {
                    cacheModel = CacheModelDeSerializer.Deserialize(xmlNode, this.configScope);
                    cacheModel.Id = this.configScope.ApplyNamespace(cacheModel.Id);

                    // Attach ExecuteEventHandler
                    foreach (XmlNode flushOn in xmlNode.SelectNodes(ApplyMappingNamespacePrefix(XML_FLUSH_ON_EXECUTE), this.configScope.XmlNamespaceManager))
                    {
                        string statementName = flushOn.Attributes["statement"].Value;
                        if (this.configScope.UseStatementNamespaces)
                        {
                            statementName = this.configScope.ApplyNamespace(statementName);
                        }

                        // delay registering statements to cache model until all sqlMap files have been processed
                        IList statementNames = (IList)this.configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id];
                        if (statementNames == null)
                        {
                            statementNames = new ArrayList();
                        }
                        statementNames.Add(statementName);
                        this.configScope.CacheModelFlushOnExecuteStatements[cacheModel.Id] = statementNames;
                    }

                    // Get Properties
                    foreach (XmlNode propertie in xmlNode.SelectNodes(ApplyMappingNamespacePrefix(XML_PROPERTY), this.configScope.XmlNamespaceManager))
                    {
                        string name = propertie.Attributes["name"].Value;
                        string value = propertie.Attributes["value"].Value;

                        cacheModel.AddProperty(name, value);
                    }

                    cacheModel.Initialize();

                    this.configScope.SqlMapper.AddCache(cacheModel);
                }
            }

            #endregion

            this.configScope.ErrorContext.Reset();
        }


        /// <summary>
        /// Process the Sql Statement
        /// </summary>
        /// <param name="aStatement"></param>
        private void ProcessSqlStatement(IStatement aStatement)
        {
            bool _isDynamic = false;
            XmlNode _commandTextNode = this.configScope.CurrentNodeContext;
            DynamicSql _dynamicSql = new DynamicSql(this.configScope, aStatement);
            StringBuilder _sqlBuffer = new StringBuilder();

            this.configScope.ErrorContext.MoreInfo = "process the Sql statement";

            //Resolve "extend" attribute on Statement
            if (aStatement.ExtendStatement.Length > 0)
            {
                // Find 'super' statement
                XmlNode _supperStatementNode = this.configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_STATEMENT) + "/child::*[@id='" + aStatement.ExtendStatement + "']", this.configScope.XmlNamespaceManager);
                if (_supperStatementNode != null)
                {
                    _commandTextNode.InnerXml = _supperStatementNode.InnerXml + _commandTextNode.InnerXml;
                }
                else
                {
                    throw new ConfigurationException("Unable to find extend statement named '" + aStatement.ExtendStatement + "' on statement '" + aStatement.Id + "'.'");
                }
            }

            this.configScope.ErrorContext.MoreInfo = "parse dynamic tags on sql statement";

            _isDynamic = this.ParseDynamicTags(_commandTextNode, _dynamicSql, _sqlBuffer, _isDynamic, false, aStatement);

            if (_isDynamic)
            {
                //动态sql内联ParemeterMap，在请求时生成。
                aStatement.Sql = _dynamicSql;
            }
            else
            {
                string _sql = _sqlBuffer.ToString();
                //根据sql语句中参数定义(#in_praremName#)生成内联ParemeterMap,并赋值给aStatement.ParameterMap
                this.ApplyInlineParemeterMap(aStatement, _sql);
            }
        }


        /// <summary>
        /// Parse dynamic tags
        /// </summary>
        /// <param name="aCommandTextNode"></param>
        /// <param name="aDynamic"></param>
        /// <param name="aSqlBuffer"></param>
        /// <param name="aIsDynamic"></param>
        /// <param name="aPostParseRequired"></param>
        /// <param name="aStatement"></param>
        /// <returns></returns>
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

                    _text = XmlNodeUtils.ParsePropertyTokens(_text, this.configScope.Properties);

                    SqlText _sqlText;
                    if (aPostParseRequired)
                    {
                        _sqlText = new SqlText();
                        _sqlText.Text = _text.ToString();
                    }
                    else
                    {
                        _sqlText = this.inlineParamMapParser.ParseInlineParameterMap(this.configScope, null, _text);
                    }

                    aDynamic.AddChild(_sqlText);
                    aSqlBuffer.Append(_text);
                }
                else if (_childXmlNode.Name == "include")
                {
                    NameValueCollection _prop = XmlNodeUtils.ParseAttributes(_childXmlNode, this.configScope.Properties);
                    string _refid = XmlNodeUtils.GetStringAttribute(_prop, "refid");
                    XmlNode _includeNode = (XmlNode)this.configScope.SqlIncludes[_refid];

                    if (_includeNode == null)
                    {
                        String _nsrefid = this.configScope.ApplyNamespace(_refid);
                        _includeNode = (XmlNode)this.configScope.SqlIncludes[_nsrefid];
                        if (_includeNode == null)
                        {
                            throw new ConfigurationException("Could not find SQL tag to include with refid '" + _refid + "'");
                        }
                    }
                    aIsDynamic = ParseDynamicTags(_includeNode, aDynamic, aSqlBuffer, aIsDynamic, false, aStatement);
                }
                else
                {
                    string _nodeName = _childXmlNode.Name;
                    IDeSerializer _serializer = this.deSerializerFactory.GetDeSerializer(_nodeName);

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
        /// <param name="aSqlStatement"></param>
        private void ApplyInlineParemeterMap(IStatement aStatement, string aSqlStatement)
        {
            string _newSql = aSqlStatement;

            this.configScope.ErrorContext.MoreInfo = "apply inline parameterMap";

            // Check the inline parameter
            if (aStatement.ParameterMap == null)
            {
                // Build a Parametermap with the inline parameters.
                // if they exist. Then delete inline infos from sqltext.

                SqlText _sqlText = this.inlineParamMapParser.ParseInlineParameterMap(this.configScope, aStatement, _newSql);

                if (_sqlText.Parameters.Length > 0)
                {
                    ParameterMap _inLineParametermap = new ParameterMap(this.configScope.DataExchangeFactory);
                    _inLineParametermap.Id = aStatement.Id + "-InLineParameterMap";
                    if (aStatement.ParameterClass != null)
                    {
                        _inLineParametermap.Class = aStatement.ParameterClass;
                    }
                    _inLineParametermap.Initialize(this.configScope.DataSource.DbProvider.UsePositionalParameters, this.configScope);

                    if (aStatement.ParameterClass == null &&
                        _sqlText.Parameters.Length == 1 && _sqlText.Parameters[0].PropertyName == "value")//#value# parameter with no parameterClass attribut
                    {
                        _inLineParametermap.DataExchange = this.configScope.DataExchangeFactory.GetDataExchangeForClass(typeof(int));//Get the primitiveDataExchange
                    }
                    aStatement.ParameterMap = _inLineParametermap;

                    int _lenght = _sqlText.Parameters.Length;
                    for (int index = 0; index < _lenght; index++)
                    {
                        _inLineParametermap.AddParameterProperty(_sqlText.Parameters[index]);
                    }
                }
                _newSql = _sqlText.Text;
            }

            ISql _sql = null;

            _newSql = _newSql.Trim();

            if (SimpleDynamicSql.IsSimpleDynamicSql(_newSql))
            {
                _sql = new SimpleDynamicSql(this.configScope, _newSql, aStatement);
                //对简单动态sql不进行预处理（BuildPreparedStatement），在请求时再进行
            }
            else
            {
                if (aStatement is Procedure)
                {
                    _sql = new ProcedureSql(this.configScope, _newSql, aStatement);
                    // Could not call BuildPreparedStatement for procedure because when Unit Test
                    // the database is not here (but in theory procedure must be prepared like statement)
                    // It's even better as we can then switch DataSource.
                }
                else if (aStatement is Statement)
                {
                    _sql = new StaticSql(this.configScope, aStatement);
                    ISqlMapSession _session = new SqlMapSession(this.configScope.SqlMapper);

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
            this.configScope.ErrorContext.Activity = "loading global properties";

            string _xPath = ApplyDataMapperNamespacePrefix(XML_PROPERTIES);
            XmlNode _nodeProperties = this.configScope.CurrentNodeContext.SelectSingleNode(_xPath, this.configScope.XmlNamespaceManager);

            if (_nodeProperties != null)
            {
                if (_nodeProperties.HasChildNodes)
                {
                    foreach (XmlNode _propertyNode in _nodeProperties.SelectNodes(ApplyDataMapperNamespacePrefix(XML_PROPERTY), this.configScope.XmlNamespaceManager))
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
            this.configScope.ErrorContext.Reset();
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
            XmlNode _parameterMapNode = this.configScope.CurrentNodeContext;

            this.configScope.ErrorContext.MoreInfo = "Build ParameterMap";

            // Get the parameterMap id
            string _parameterMapId = this.configScope.ApplyNamespace((_parameterMapNode.Attributes.GetNamedItem("id")).Value);
            this.configScope.ErrorContext.ObjectId = _parameterMapId;

            // Did we already process it ?
            if (this.configScope.SqlMapper.ParameterMaps.Contains(_parameterMapId) == false)
            {
                _parameterMap = ParameterMapDeSerializer.Deserialize(_parameterMapNode, this.configScope);

                _parameterMap.Id = this.configScope.ApplyNamespace(_parameterMap.Id);
                string _extendMapAttribute = _parameterMap.ExtendMap;
                _parameterMap.ExtendMap = this.configScope.ApplyNamespace(_parameterMap.ExtendMap);

                //extends属性
                if (_parameterMap.ExtendMap.Length > 0)
                {
                    ParameterMap _superMap;
                    // Did we already build Extend ParameterMap ?
                    if (this.configScope.SqlMapper.ParameterMaps.Contains(_parameterMap.ExtendMap) == false)
                    {
                        //sqlMap/parameterMaps/parameterMap[@id='value'] 选取属性id='value'的parameterMap节点
                        XmlNode _superNode = this.configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_PARAMETER) + _extendMapAttribute + "']", this.configScope.XmlNamespaceManager);

                        if (_superNode != null)
                        {
                            this.configScope.ErrorContext.MoreInfo = "Build parent ParameterMap";
                            this.configScope.CurrentNodeContext = _superNode;
                            this.BuildParameterMap();
                            _superMap = this.configScope.SqlMapper.GetParameterMap(_parameterMap.ExtendMap);
                        }
                        else
                        {
                            throw new ConfigurationException("In mapping file '" + this.configScope.SqlMapNamespace + "' the parameterMap '" + _parameterMap.Id + "' can not resolve extends attribute '" + _parameterMap.ExtendMap + "'");
                        }
                    }
                    else
                    {
                        _superMap = this.configScope.SqlMapper.GetParameterMap(_parameterMap.ExtendMap);
                    }
                    // Add extends property
                    int index = 0;

                    foreach (string propertyName in _superMap.GetPropertyNameArray())
                    {
                        ParameterProperty _property = _superMap.GetProperty(propertyName).Clone();
                        _property.Initialize(this.configScope, _parameterMap.Class);
                        _parameterMap.InsertParameterProperty(index, _property);
                        index++;
                    }
                }
                
                this.configScope.SqlMapper.AddParameterMap(_parameterMap);
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
                _resultMap = ResultMapDeSerializer.Deserialize(_resultMapNode, this.configScope);

                string _attributeExtendMap = _resultMap.ExtendMap;
                _resultMap.ExtendMap = this.configScope.ApplyNamespace(_resultMap.ExtendMap);

                if (_resultMap.ExtendMap != null && _resultMap.ExtendMap.Length > 0)
                {
                    IResultMap _superMap = null;
                    // Did we already build Extend ResultMap?
                    if (this.configScope.SqlMapper.ResultMaps.Contains(_resultMap.ExtendMap) == false)
                    {
                        XmlNode superNode = this.configScope.SqlMapDocument.SelectSingleNode(ApplyMappingNamespacePrefix(XML_SEARCH_RESULTMAP) + _attributeExtendMap + "']", this.configScope.XmlNamespaceManager);

                        if (superNode != null)
                        {
                            this.configScope.ErrorContext.MoreInfo = "Build parent ResultMap";
                            this.configScope.CurrentNodeContext = superNode;
                            BuildResultMap();
                            _superMap = this.configScope.SqlMapper.GetResultMap(_resultMap.ExtendMap);
                        }
                        else
                        {
                            throw new ConfigurationException("In mapping file '" + this.configScope.SqlMapNamespace + "' the resultMap '" + _resultMap.Id + "' can not resolve extends attribute '" + _resultMap.ExtendMap + "'");
                        }
                    }
                    else
                    {
                        _superMap = this.configScope.SqlMapper.GetResultMap(_resultMap.ExtendMap);
                    }

                    // Add parent property
                    for (int index = 0; index < _superMap.Properties.Count; index++)
                    {
                        ResultProperty property = _superMap.Properties[index].Clone();
                        property.Initialize(this.configScope, _resultMap.Class);
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
                    if (_resultMap.Parameters.Count == 0)
                    {
                        for (int i = 0; i < _superMap.Parameters.Count; i++)
                        {
                            _resultMap.Parameters.Add(_superMap.Parameters[i]);
                        }
                        if (_resultMap.Parameters.Count > 0)
                        {
                            _resultMap.SetObjectFactory(this.configScope);
                        }
                    }


                    // Verify that that each groupBy element correspond to a class member
                    // of one of result property
                    for (int i = 0; i < _resultMap.GroupByPropertyNames.Count; i++)
                    {
                        string memberName = _resultMap.GroupByPropertyNames[i];
                        if (!_resultMap.Properties.Contains(memberName))
                        {
                            throw new ConfigurationException(
                                string.Format(
                                    "Could not configure ResultMap named \"{0}\". Check the groupBy attribute. Cause: there's no result property named \"{1}\".",
                                    _resultMap.Id, memberName));
                        }
                    }
                }
                
                _resultMap.InitializeGroupByProperties();
                this.configScope.SqlMapper.AddResultMap(_resultMap);
            }
        }


        /// <summary>
        /// Gets a resource stream.
        /// </summary>
        /// <param name="schemaResourceKey">The schema resource key.</param>
        /// <returns>A resource stream.</returns>
        public Stream GetStream(string schemaResourceKey)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("IBatisNetCore.DataMapper." + schemaResourceKey);
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
