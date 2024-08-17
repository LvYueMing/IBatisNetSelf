using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.Common
{
    public class DbProvider : IDbProvider
    {
        private const string SQLPARAMETER = "?";

        #region Fields
        [NonSerialized]
        private string name = string.Empty;
        [NonSerialized]
        private string description = string.Empty;
        [NonSerialized]
        private bool isDefault = false;
        [NonSerialized]
        private bool isEnabled = true;

        [NonSerialized]
        private string assemblyName = string.Empty;
        [NonSerialized]
        private string connectionClass = string.Empty;
        [NonSerialized]
        private string commandClass = string.Empty;

        [NonSerialized]
        private string parameterDbTypeClass = string.Empty;
        [NonSerialized]
        private Type? parameterDbType;
        [NonSerialized]
        private string parameterDbTypeProperty = string.Empty;

        [NonSerialized]
        private string dataAdapterClass = string.Empty;
        [NonSerialized]
        private string commandBuilderClass = string.Empty;
        [NonSerialized]
        private Type? commandBuilderType;


        [NonSerialized]
        private string parameterPrefix = string.Empty;
        [NonSerialized]
        private bool useParameterPrefixInSql = true;
        [NonSerialized]
        private bool useParameterPrefixInParameter = true;
        [NonSerialized]
        private bool usePositionalParameters = false;
        [NonSerialized]
        private bool templateConnectionIsICloneable = false;
        [NonSerialized]
        private bool templateDataAdapterIsICloneable = false;
        [NonSerialized]
        private bool setDbParameterSize = true;
        [NonSerialized]
        private bool setDbParameterPrecision = true;
        [NonSerialized]
        private bool setDbParameterScale = true;
        [NonSerialized]
        private bool useDeriveParameters = true;
        [NonSerialized]
        private bool allowMARS = false;


        [NonSerialized]
        private IDbConnection? templateConnection;
        [NonSerialized]
        private IDbDataAdapter? templateDataAdapter;

        #endregion

        #region  Properties

        /// <summary>
        /// Name used to identify the provider amongst the others.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return this.name; }
            set
            {
                this.CheckPropertyString("Name", value);
                this.name = value;
            }
        }

        /// <summary>
        /// Description.
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get => this.description; set => this.description = value; }

        /// <summary>
        /// The name of the assembly which conatins the definition of the provider.
        /// </summary>
        /// <example>Examples : "System.Data", "Microsoft.Data.Odbc"</example>
        public string AssemblyName
        {
            get { return this.assemblyName; }
            set
            {
                this.CheckPropertyString("AssemblyName", value);
                this.assemblyName = value;
            }
        }

        /// <summary>
        /// Tell us if it is the default data source.
        /// Default false.
        /// </summary>
        [XmlAttribute("default")]
        public bool IsDefault { get => this.isDefault; set => this.isDefault = value; }

        /// <summary>
        /// Tell us if this provider is enabled.
        /// Default true.
        /// </summary>
        [XmlAttribute("enabled")]
        public bool IsEnabled { get => this.isEnabled; set => this.isEnabled = value; }

        /// <summary>
        /// Tell us if this provider allows having multiple open <see cref="IDataReader"/> with
        /// the same <see cref="IDbConnection"/>.
        /// </summary>
        /// <remarks>
        /// It's a new feature in ADO.NET 2.0 and Sql Server 2005 that allows for multiple forward only read only result sets (MARS).
        /// Some databases have supported this functionality for a long time :
        /// Not Supported : DB2, MySql.Data, OLE DB provider [except Sql Server 2005 when using MDAC 9], SQLite, Obdc 
        /// Supported :  Sql Server 2005, Npgsql
        /// </remarks>
        [XmlAttribute("allowMARS")]
        public bool AllowMARS { get => this.allowMARS; set => this.allowMARS = value; }

        /// <summary>
        /// The connection class name to use.
        /// </summary>
        /// <example>
        /// "System.Data.OleDb.OleDbConnection", 
        /// "System.Data.SqlClient.SqlConnection", 
        /// "Microsoft.Data.Odbc.OdbcConnection"
        /// </example>
        [XmlAttribute("connectionClass")]
        public string DbConnectionClass
        {
            get { return this.connectionClass; }
            set
            {
                this.CheckPropertyString("DbConnectionClass", value);
                this.connectionClass = value;
            }
        }

        /// <summary>
        /// Does this ConnectionProvider require the use of a Named Prefix in the SQL 
        /// statement. 
        /// </summary>
        /// <remarks>
        /// The OLE DB/ODBC .NET Provider does not support named parameters for 
        /// passing parameters to an SQL Statement or a stored procedure called 
        /// by an IDbCommand when CommandType is set to Text.
        /// 
        /// For example, SqlClient requires select * from simple where simple_id = @simple_id
        /// If this is false, like with the OleDb or Obdc provider, then it is assumed that 
        /// the ? can be a placeholder for the parameter in the SQL statement when CommandType 
        /// is set to Text.		
        /// </remarks>
        [XmlAttribute("useParameterPrefixInSql")]
        public bool UseParameterPrefixInSql { get => this.useParameterPrefixInSql; set => this.useParameterPrefixInSql = value; }

        /// <summary>
        /// Does this ConnectionProvider require the use of the Named Prefix when trying
        /// to reference the Parameter in the Command's Parameter collection. 
        /// </summary>
        /// <remarks>
        /// This is really only useful when the UseParameterPrefixInSql = true. 
        /// When this is true the code will look like IDbParameter param = cmd.Parameters["@paramName"], 
        /// if this is false the code will be IDbParameter param = cmd.Parameters["paramName"] - ie - Oracle.
        /// </remarks>
        [XmlAttribute("useParameterPrefixInParameter")]
        public bool UseParameterPrefixInParameter { get => this.useParameterPrefixInParameter; set => this.useParameterPrefixInParameter = value; }

        /// <summary>
        /// The OLE DB/OBDC .NET Provider uses positional parameters that are marked with a 
        /// question mark (?) instead of named parameters.
        /// </summary>
        [XmlAttribute("usePositionalParameters")]
        public bool UsePositionalParameters { get => this.usePositionalParameters; set => this.usePositionalParameters = value; }

        /// <summary>
        /// Used to indicate whether or not the provider 
        /// supports parameter size.
        /// </summary>
        /// <remarks>
        /// See JIRA-49 about SQLite.Net provider not supporting parameter size.
        /// </remarks>
        [XmlAttribute("setDbParameterSize")]
        public bool SetDbParameterSize { get => this.setDbParameterSize; set => this.setDbParameterSize = value; }

        /// <summary>
        /// Used to indicate whether or not the provider 
        /// supports parameter precision.
        /// </summary>
        /// <remarks>
        /// See JIRA-49 about SQLite.Net provider not supporting parameter precision.
        /// </remarks>
        [XmlAttribute("setDbParameterPrecision")]
        public bool SetDbParameterPrecision { get => this.setDbParameterPrecision; set => this.setDbParameterPrecision = value; }

        /// <summary>
        /// Used to indicate whether or not the provider 
        /// supports a parameter scale.
        /// </summary>
        /// <remarks>
        /// See JIRA-49 about SQLite.Net provider not supporting parameter scale.
        /// </remarks>
        [XmlAttribute("setDbParameterScale")]
        public bool SetDbParameterScale { get => this.setDbParameterScale; set => this.setDbParameterScale = value; }

        /// <summary>
        /// Used to indicate whether or not the provider 
        /// supports DeriveParameters method for procedure.
        /// </summary>
        [XmlAttribute("useDeriveParameters")]
        public bool UseDeriveParameters { get => this.useDeriveParameters; set => this.useDeriveParameters = value; }

        /// <summary>
        /// The command class name to use.
        /// </summary>
        /// <example>
        /// "System.Data.SqlClient.SqlCommand"
        /// </example>
        [XmlAttribute("commandClass")]
        public string DbCommandClass
        {
            get { return this.commandClass; }
            set
            {
                this.CheckPropertyString("DbCommandClass", value);
                this.commandClass = value;
            }
        }

        /// <summary>
        /// The ParameterDbType class name to use.
        /// </summary>			
        /// <example>
        /// "System.Data.SqlDbType"
        /// </example>
        [XmlAttribute("parameterDbTypeClass")]
        public string ParameterDbTypeClass
        {
            get { return this.parameterDbTypeClass; }
            set
            {
                this.CheckPropertyString("ParameterDbTypeClass", value);
                this.parameterDbTypeClass = value;
            }
        }

        /// <summary>
        /// The ParameterDbTypeProperty class name to use.
        /// </summary>
        /// <example >
        /// SqlDbType in SqlParamater.SqlDbType, 
        /// OracleType in OracleParameter.OracleType.
        /// </example>
        [XmlAttribute("parameterDbTypeProperty")]
        public string ParameterDbTypeProperty
        {
            get { return this.parameterDbTypeProperty; }
            set
            {
                this.CheckPropertyString("ParameterDbTypeProperty", value);
                this.parameterDbTypeProperty = value;
            }
        }

        /// <summary>
        /// The dataAdapter class name to use.
        /// </summary>
        /// <example >
        /// "System.Data.SqlDbType"
        /// </example>
        [XmlAttribute("dataAdapterClass")]
        public string DataAdapterClass
        {
            get { return this.dataAdapterClass; }
            set
            {
                this.CheckPropertyString("DataAdapterClass", value);
                this.dataAdapterClass = value;
            }
        }

        /// <summary>
        /// The commandBuilder class name to use.
        /// </summary>
        /// <example >
        /// "System.Data.OleDb.OleDbCommandBuilder", 
        /// "System.Data.SqlClient.SqlCommandBuilder", 
        /// "Microsoft.Data.Odbc.OdbcCommandBuilder"
        /// </example>
        [XmlAttribute("commandBuilderClass")]
        public string CommandBuilderClass
        {
            get { return this.commandBuilderClass; }
            set
            {
                this.CheckPropertyString("CommandBuilderClass", value);
                this.commandBuilderClass = value;
            }
        }

        /// <summary>
        /// Parameter prefix use in store procedure.
        /// </summary>
        /// <example> @ for Sql Server.</example>
        [XmlAttribute("parameterPrefix")]
        public string ParameterPrefix
        {
            get { return this.parameterPrefix; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    this.parameterPrefix = "";
                }
                else
                {
                    this.parameterPrefix = value;
                }
            }
        }

        /// <summary>
        /// Check if this provider is Odbc ?
        /// </summary>
        [XmlIgnore]
        public bool IsObdc => (this.connectionClass.IndexOf(".Odbc.") > 0);

        /// <summary>
        /// Get the CommandBuilder Type for this provider.
        /// </summary>
        /// <returns>An object.</returns>
        [XmlIgnore]
        public Type CommandBuilderType => this.commandBuilderType;

        /// <summary>
        /// Get the ParameterDb Type for this provider.
        /// </summary>
        /// <returns>An object.</returns>
        [XmlIgnore]
        public Type ParameterDbType => this.parameterDbType;

        #endregion


        #region Constructor(s)/Destructor
        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public DbProvider()
        {
        }
        #endregion


        #region Methods

        /// <summary>
        /// Init the provider.
        /// </summary>
        public void Initialize()
        {
            Assembly _assembly;
            Type? _type = null;

            try
            {
                // Assembly.Load(this.assemblyName)方法需要在应用程序中添加名称为this.assemblyName的程序集的引用
                //_assembly = Assembly.Load(this.assemblyName);

                //在当前应用程序根目录下查找并加载名称为this.assemblyName的程序集
                string _assemblyPath = this.assemblyName.Split(',')[0] + ".dll";
                _assembly = Assembly.LoadFrom(_assemblyPath);

                // Build the DataAdapter template 
                _type = _assembly.GetType(this.dataAdapterClass, true);
                this.templateDataAdapter = (IDbDataAdapter)_type.GetConstructor(Type.EmptyTypes).Invoke(null);


                // Build the connection template
                _type = _assembly.GetType(this.connectionClass, true);
                this.CheckPropertyType(this.connectionClass, typeof(IDbConnection), _type);
                this.templateConnection = (IDbConnection)_type.GetConstructor(Type.EmptyTypes).Invoke(null);

                // Get the CommandBuilder Type
                this.commandBuilderType = _assembly.GetType(this.commandBuilderClass, true);

                // Get the DbType Type
                if (this.parameterDbTypeClass.IndexOf(',') > 0)
                {
                    this.parameterDbType = TypeUtils.ResolveType(this.parameterDbTypeClass);
                }
                else
                {
                    this.parameterDbType = _assembly.GetType(this.parameterDbTypeClass, true);
                }

                this.templateConnectionIsICloneable = this.templateConnection is ICloneable;
                this.templateDataAdapterIsICloneable = this.templateDataAdapter is ICloneable;

            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"无法配置驱动程序，无法加载或未找到名为{this.name}的驱动程序，失败原因: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create a command object for this provider.
        /// </summary>
        /// <returns>An 'IDbCommand' object.</returns>
        public virtual IDbCommand CreateCommand()
        {
            return this.templateConnection.CreateCommand();
        }

        /// <summary>
        /// Create a connection object for this provider.
        /// </summary>
        /// <returns>An 'IDbConnection' object.</returns>
        public virtual IDbConnection CreateConnection()
        {
            if (this.templateConnectionIsICloneable)
            {
                return (IDbConnection)((ICloneable)this.templateConnection).Clone();
            }
            else
            {
                return (IDbConnection)Activator.CreateInstance(this.templateConnection.GetType());
            }
        }

        /// <summary>
        /// Create a dataAdapter object for this provider.
        /// </summary>
        /// <returns>An 'IDbDataAdapter' object.</returns>
        public virtual IDbDataAdapter CreateDataAdapter()
        {
            if (this.templateDataAdapterIsICloneable)
            {
                return (IDbDataAdapter)((ICloneable)this.templateDataAdapter).Clone();
            }
            else
            {
                return (IDbDataAdapter)Activator.CreateInstance(this.templateDataAdapter.GetType());
            }
        }

        /// <summary>
        /// Create a IDbDataParameter object for this provider.
        /// </summary>
        /// <returns>An 'IDbDataParameter' object.</returns>
        public virtual IDbDataParameter CreateDataParameter()
        {
            return this.templateConnection.CreateCommand().CreateParameter();
        }

        /// <summary>
        /// Changes the parameterName into the correct format for an IDbParameter for the Driver.
        /// </summary>
        /// <remarks>
        /// For SqlServerConnectionProvider it will change <c>id</c> to <c>@id</c>
        /// </remarks>
        /// <param name="parameterName">The unformatted name of the parameter</param>
        /// <returns>A parameter formatted for an IDbParameter.</returns>
        public virtual string FormatNameForParameter(string parameterName)
        {
            return this.useParameterPrefixInParameter ? (this.parameterPrefix + parameterName) : parameterName;
        }


        /// <summary>
        /// Change the parameterName into the correct format IDbCommand.CommandText for the ConnectionProvider
        /// </summary>
        /// <param name="parameterName">The unformatted name of the parameter</param>
        /// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
        public virtual string FormatNameForSql(string parameterName)
        {
            return this.useParameterPrefixInSql ? (this.parameterPrefix + parameterName) : SQLPARAMETER;
        }

        private void CheckPropertyString(string propertyName, string value)
        {
            if (value == null || value.Trim().Length == 0)
            {
                throw new ArgumentException($"属性({propertyName})不能为空。", propertyName);
            }
        }

        private void CheckPropertyType(string propertyName, Type expectedType, Type value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(propertyName, $"属性({propertyName})不能为空。");
            }
        }


        /// <summary>
		/// Equals implemantation.
		/// </summary>
		/// <param name="obj">The test object.</param>
		/// <returns>A boolean.</returns>
		public override bool Equals(object? obj)
        {
            if ((obj != null) && (obj is IDbProvider))
            {
                IDbProvider that = (IDbProvider)obj;
                return ((this.name == that.Name) &&
                    (this.assemblyName == that.AssemblyName) &&
                    (this.connectionClass == that.DbConnectionClass));
            }
            return false;
        }

        /// <summary>
        /// A hashcode for the provider.
        /// </summary>
        /// <returns>An integer.</returns>
        public override int GetHashCode()
        {
            return (this.name.GetHashCode() ^ this.assemblyName.GetHashCode() ^ this.connectionClass.GetHashCode());
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns>A string that describes the provider.</returns>
        public override string ToString()
        {
            return "Provider " + this.name;
        }


        #endregion

    }
}
