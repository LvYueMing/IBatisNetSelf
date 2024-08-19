using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects;

namespace IBatisNetSelf.DataMapper.Scope
{
    /// <summary>
    /// The ConfigurationScope maintains the state of the build process.
    /// </summary>
    public class ConfigurationScope : IScope
    {
        /// <summary>
        /// Empty parameter map
        /// </summary>
        public const string EMPTY_PARAMETER_MAP = "IBatis.Empty.ParameterMap";

        #region Fields

        private ErrorContext errorContext = null;
        private HybridDictionary providers = new HybridDictionary();
        private HybridDictionary sqlIncludes = new HybridDictionary();

        private NameValueCollection properties = new NameValueCollection();

        private XmlDocument sqlMapConfigDocument = null;
        private XmlDocument sqlMapDocument = null;
        private XmlNode nodeContext = null;

        //settings
        private bool useStatementNamespaces = false;
        private bool isCacheModelsEnabled = false;
        private bool useReflectionOptimizer = true;
        private bool validateSqlMap = false;

        private bool useConfigFileWatcher = false;
        private bool isCallFromDao = false;

        private ISqlMapper sqlMapper = null;
        private string sqlMapNamespace = null;
        private DataSource dataSource = null;
        private bool isXmlValid = true;
        private XmlNamespaceManager nsmgr = null;
        private HybridDictionary cacheModelFlushOnExecuteStatements = new HybridDictionary();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConfigurationScope()
        {
            this.errorContext = new ErrorContext();

            this.providers.Clear();
        }
        #endregion


        #region Properties

        /// <summary>
        /// The list of sql fragment
        /// </summary>
        public HybridDictionary SqlIncludes
        {
            get => this.sqlIncludes;
        }

        /// <summary>
        /// XmlNamespaceManager
        /// </summary>
        public XmlNamespaceManager XmlNamespaceManager
        {
            set => this.nsmgr = value;
            get => this.nsmgr;
        }

        /// <summary>
        /// Set if the parser should validate the sqlMap documents
        /// </summary>
        public bool ValidateSqlMap
        {
            set => this.validateSqlMap = value;
            get => this.validateSqlMap;
        }

        /// <summary>
        /// Tells us if the xml configuration file validate the schema 
        /// </summary>
        public bool IsXmlValid
        {
            set => this.isXmlValid = value;
            get => this.isXmlValid;
        }


        /// <summary>
        /// The current SqlMap namespace.
        /// </summary>
        public string SqlMapNamespace
        {
            set => this.sqlMapNamespace = value;
            get => this.sqlMapNamespace;
        }

        /// <summary>
        /// The SqlMapper we are building.
        /// </summary>
        public ISqlMapper SqlMapper
        {
            set => this.sqlMapper = value;
            get => this.sqlMapper;
        }


        /// <summary>
        /// A factory for DataExchange objects
        /// </summary>
        public DataExchangeFactory DataExchangeFactory
        {
            get => this.sqlMapper.DataExchangeFactory;
        }

        /// <summary>
        /// Tell us if we are in a DataMapper context.
        /// </summary>
        public bool IsCallFromDao
        {
            set => this.isCallFromDao = value;
            get => this.isCallFromDao;
        }

        /// <summary>
        /// Tell us if we cache model is enabled.
        /// </summary>
        public bool IsCacheModelsEnabled
        {
            set => this.isCacheModelsEnabled = value;
            get => this.isCacheModelsEnabled;
        }

        /// <summary>
        /// External data source
        /// </summary>
        public DataSource DataSource
        {
            set => this.dataSource = value;
            get => this.dataSource;
        }

        /// <summary>
        /// The current context node we are analizing
        /// </summary>
        public XmlNode NodeContext
        {
            set => this.nodeContext = value;
            get => this.nodeContext;
        }

        /// <summary>
        /// The XML SqlMap config file
        /// </summary>
        public XmlDocument SqlMapConfigDocument
        {
            set => this.sqlMapConfigDocument = value;
            get => this.sqlMapConfigDocument;
        }

        /// <summary>
        /// A XML SqlMap file
        /// </summary>
        public XmlDocument SqlMapDocument
        {
            set => this.sqlMapDocument = value;
            get => this.sqlMapDocument;
        }

        /// <summary>
        /// Tell us if we use Configuration File Watcher
        /// </summary>
        public bool UseConfigFileWatcher
        {
            set => this.useConfigFileWatcher = value;
            get => this.useConfigFileWatcher;
        }

        /// <summary>
        /// Tell us if we use statements namespaces
        /// </summary>
        public bool UseStatementNamespaces
        {
            set => this.useStatementNamespaces = value;
            get => this.useStatementNamespaces;
        }

        /// <summary>
        ///  Get the request's error context
        /// </summary>
        public ErrorContext ErrorContext
        {
            get => this.errorContext;
        }

        /// <summary>
        ///  List of providers
        /// </summary>
        public HybridDictionary Providers
        {
            get => this.providers;
        }

        /// <summary>
        ///  List of global properties
        /// </summary>
        public NameValueCollection Properties
        {
            get => this.properties;
        }

        /// <summary>
        /// Indicates if we can use reflection optimizer.
        /// </summary>
        public bool UseReflectionOptimizer
        {
            get => this.useReflectionOptimizer;
            set => this.useReflectionOptimizer = value;
        }

        /// <summary>
        /// Temporary storage for mapping cache model ids (key is System.String) to statements (value is IList which contains IMappedStatements).
        /// </summary>
        public HybridDictionary CacheModelFlushOnExecuteStatements
        {
            get => this.cacheModelFlushOnExecuteStatements;
            set => this.cacheModelFlushOnExecuteStatements = value;
        }

        #endregion

        /// <summary>
        /// Register under Statement Name or Fully Qualified Statement Name
        /// </summary>
        /// <param name="id">An Identity</param>
        /// <returns>The new Identity</returns>
        public string ApplyNamespace(string id)
        {
            string _newId = id;

            if (this.sqlMapNamespace != null && this.sqlMapNamespace.Length > 0
                && id != null && id.Length > 0 && id.IndexOf(".") < 0)
            {
                _newId = this.sqlMapNamespace + DomSqlMapBuilder.DOT + id;
            }
            return _newId;
        }

        /// <summary>
        /// Resolves the type handler.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="clrType">Type of the CLR.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="forSetter">if set to <c>true</c> [for setter].</param>
        /// <returns></returns>
        public ITypeHandler ResolveTypeHandler(Type clazz, string memberName, string clrType, string dbType, bool forSetter)
        {
            ITypeHandler handler = null;
            if (clazz == null)
            {
                handler = this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            else if (typeof(IDictionary).IsAssignableFrom(clazz))
            {
                // IDictionary
                if (clrType == null || clrType.Length == 0)
                {
                    handler = this.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    try
                    {
                        Type type = TypeUtils.ResolveType(clrType);
                        handler = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                    }
                    catch (Exception e)
                    {

                        throw new ConfigurationException("Error. Could not set TypeHandler.  Cause: " + e.Message, e);
                    }
                }
            }
            else if (this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType) != null)
            {
                // Primitive
                handler = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(clazz, dbType);
            }
            else
            {
                // .NET object
                if (clrType == null || clrType.Length == 0)
                {
                    Type type = null;
                    if (forSetter)
                    {
                        type = ObjectProbe.GetMemberTypeForSetter(clazz, memberName);
                    }
                    else
                    {
                        type = ObjectProbe.GetMemberTypeForGetter(clazz, memberName);
                    }
                    handler = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                }
                else
                {
                    try
                    {
                        Type type = TypeUtils.ResolveType(clrType);
                        handler = this.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationException("Error. Could not set TypeHandler.  Cause: " + e.Message, e);
                    }
                }
            }

            return handler;
        }

    }
}
