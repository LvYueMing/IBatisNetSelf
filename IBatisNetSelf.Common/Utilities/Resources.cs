using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Xml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// A class to simplify access to resources.
    /// 
    /// The file can be loaded from the application root directory 
    /// (use the resource attribute) 
    /// or from any valid URL (use the url attribute). 
    /// For example,to load a fixed path file, use:
    /// <properties url= file:///c:/config/my.properties? />
    /// </summary>
    public class Resources
    {
        #region Fields
        private static string applicationBase = AppDomain.CurrentDomain.BaseDirectory;
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //private static CachedTypeResolver _cachedTypeResolver = null;

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Properties
        /// <summary>
        /// The name of the directory containing the application
        /// </summary>
        public static string ApplicationBase => applicationBase;


        /// <summary>
        /// The name of the directory used to probe the assemblies.
        /// </summary>
        public static string BaseDirectory => baseDirectory;



        #endregion

        #region Constructor (s) / Destructor
        static Resources()
        {

        }
        #endregion

        #region Methods

        /// <summary>
        /// Protocole separator
        /// </summary>
        public const string PROTOCOL_SEPARATOR = "://";

        /// <summary>
        /// Strips protocol name from the resource name
        /// </summary>
        /// <param name="filePath">Name of the resource</param>
        /// <returns>Name of the resource without protocol name</returns>
        public static string GetFileSystemResourceWithoutProtocol(string filePath)
        {
            int pos = filePath.IndexOf(PROTOCOL_SEPARATOR);
            if (pos == -1)
            {
                return filePath;
            }
            else
            {
                // skip forward slashes after protocol name, if any
                if (filePath.Length > pos + PROTOCOL_SEPARATOR.Length)
                {
                    while (filePath[++pos] == '/')
                    {
                        ;
                    }
                }
                return filePath.Substring(pos);
            }
        }

        /// <summary>
        /// Get config file
        /// </summary>
        /// <param name="resourcePath">
        /// A config resource path.
        /// </param>
        /// <returns>An XmlDocument representation of the config file</returns>
        public static XmlDocument GetConfigAsXmlDocument(string resourcePath)
        {
            XmlDocument _configXmlDoc = new XmlDocument();
            XmlTextReader _reader = null;
            //去掉 "://"
            string _resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);

            if (!Resources.FileExists(_resourcePath))
            {
                _resourcePath = Path.Combine(baseDirectory, _resourcePath);
            }

            try
            {
                _reader = new XmlTextReader(_resourcePath);
                _configXmlDoc.Load(_reader);
            }
            catch (Exception e)
            {
                throw new ConfigurationException($"Unable to load config file \"{resourcePath}\". Cause : {e.Message}");
            }
            finally
            {
                if (_reader != null)
                {
                    _reader.Close();
                }
            }
            return _configXmlDoc;

        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="filePath">The file to check.</param>
        /// <returns>
        /// true if the caller has the required permissions and path contains the name of an existing file
        /// false if the caller has the required permissions and path doesn't contain the name of an existing file
        /// else exception
        /// </returns>
        public static bool FileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                // true if the caller has the required permissions and path contains the name of an existing file; 
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Load an XML resource from a location specify by the node.
        /// </summary>
        /// <param name="aNode">An location node</param>
        /// <param name="aProperties">the global properties</param>
        /// <returns>Return the Xml document load.</returns>
        public static XmlDocument GetAsXmlDocument(XmlNode aNode, NameValueCollection aProperties)
        {
            XmlDocument _xmlDocument = null;

            if (aNode.Attributes["resource"] != null)
            {
                string ressource = XmlNodeUtils.ParsePropertyTokens(aNode.Attributes["resource"].Value, aProperties);
                _xmlDocument = Resources.GetResourceAsXmlDocument(ressource);
            }
            else if (aNode.Attributes["url"] != null)
            {
                string url = XmlNodeUtils.ParsePropertyTokens(aNode.Attributes["url"].Value, aProperties);
                _xmlDocument = Resources.GetUrlAsXmlDocument(url);
            }
            else if (aNode.Attributes["embedded"] != null)
            {
                string embedded = XmlNodeUtils.ParsePropertyTokens(aNode.Attributes["embedded"].Value, aProperties);
                _xmlDocument = Resources.GetEmbeddedResourceAsXmlDocument(embedded);
            }

            return _xmlDocument;
        }


        /// <summary>
        /// Get the path resource of an url or resource location.
        /// </summary>
        /// <param name="node">The specification from where to load.</param>
        /// <param name="properties">the global properties</param>
        /// <returns></returns>
        public static string GetValueOfNodeResourceUrl(XmlNode node, NameValueCollection properties)
        {
            string path = null;

            if (node.Attributes["resource"] != null)
            {
                string ressource = XmlNodeUtils.ParsePropertyTokens(node.Attributes["resource"].Value, properties);
                path = Path.Combine(applicationBase, ressource);
            }
            else if (node.Attributes["url"] != null)
            {
                string url = XmlNodeUtils.ParsePropertyTokens(node.Attributes["url"].Value, properties);
                path = url;
            }

            return path;
        }

        /// <summary>
        /// Get XmlDocument from a stream resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static XmlDocument GetStreamAsXmlDocument(Stream resource)
        {
            XmlDocument _configXmlDoc = new XmlDocument();

            try
            {
                _configXmlDoc.Load(resource);
            }
            catch (Exception e)
            {
                throw new ConfigurationException($"Unable to load XmlDocument via stream. Cause : {e.Message}", e);
            }

            return _configXmlDoc;
        }

        /// <summary>
        /// Get XmlDocument from a FileInfo resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static XmlDocument GetFileInfoAsXmlDocument(FileInfo resource)
        {
            XmlDocument _configXmlDoc = new XmlDocument();

            try
            {
                _configXmlDoc.Load(resource.FullName);
            }
            catch (Exception e)
            {
                throw new ConfigurationException(
                    string.Format("Unable to load XmlDocument via FileInfo. Cause : {0}",
                    e.Message), e);
            }

            return _configXmlDoc;
        }

        /// <summary>
        /// Get XmlDocument from a Uri resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static XmlDocument GetUriAsXmlDocument(Uri resource)
        {
            XmlDocument _configXmlDoc = new XmlDocument();

            try
            {
                _configXmlDoc.Load(resource.AbsoluteUri);
            }
            catch (Exception e)
            {
                throw new ConfigurationException($"Unable to load XmlDocument via Uri. Cause : {e.Message}", e);
            }

            return _configXmlDoc;
        }

        /// <summary>
        /// Get XmlDocument from relative (from root directory of the application) path resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static XmlDocument GetResourceAsXmlDocument(string resource)
        {
            XmlDocument _configXmlDoc = new XmlDocument();

            try
            {
                _configXmlDoc.Load(Path.Combine(applicationBase, resource));
            }
            catch (Exception e)
            {
                throw new ConfigurationException($"Unable to load file via resource \"{resource}\" as resource. Cause : {e.Message}", e);
            }

            return _configXmlDoc;
        }


        /// <summary>
        /// Get XmlDocument from absolute path resource
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static XmlDocument GetUrlAsXmlDocument(string url)
        {
            XmlDocument _configXmlDoc = new XmlDocument();

            try
            {
                _configXmlDoc.Load(url);
            }
            catch (Exception e)
            {
                throw new ConfigurationException(
                    string.Format("Unable to load file via url \"{0}\" as url. Cause : {1}",
                    url,
                    e.Message), e);
            }

            return _configXmlDoc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aResource"></param>
        /// <returns></returns>
        public static XmlDocument GetEmbeddedResourceAsXmlDocument(string aResource)
        {
            XmlDocument _configXmlDoc = new XmlDocument();
            bool _isLoad = false;

            FileAssemblyInfo _fileInfo = new FileAssemblyInfo(aResource);
            if (_fileInfo.IsAssemblyQualified)
            {
                Assembly _assembly = null;
                _assembly = Assembly.LoadWithPartialName(_fileInfo.AssemblyName);
                Stream _stream = _assembly.GetManifestResourceStream(_fileInfo.ResourceFileName);
                // JIRA - IBATISNET-103 
                if (_stream == null)
                {
                    _stream = _assembly.GetManifestResourceStream(_fileInfo.FileName);
                }
                if (_stream != null)
                {
                    try
                    {
                        _configXmlDoc.Load(_stream);
                        _isLoad = true;
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationException(
                            string.Format("Unable to load file \"{0}\" in embedded resource. Cause : {1}",
                            aResource,
                            e.Message), e);
                    }
                }
            }
            else
            {
                // bare type name... loop thru all loaded assemblies
                Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly _assembly in _assemblies)
                {
                    Stream _stream = _assembly.GetManifestResourceStream(_fileInfo.FileName);
                    if (_stream != null)
                    {
                        try
                        {
                            _configXmlDoc.Load(_stream);
                            _isLoad = true;
                        }
                        catch (Exception e)
                        {
                            throw new ConfigurationException(
                                string.Format("Unable to load file \"{0}\" in embedded resource. Cause : ",
                                aResource,
                                e.Message), e);
                        }
                        break;
                    }
                }
            }

            //{
            //	_logger.Error("Could not load embedded resource from assembly");
            //	throw new ConfigurationException(
            //		string.Format("Unable to load embedded resource from assembly \"{0}\".",
            //		_fileInfo.OriginalFileName));
            //}

            return _configXmlDoc;
        }


        /// <summary>
        /// Load a file from a given resource path
        /// </summary>
        /// <param name="resourcePath">
        /// The resource path
        /// </param>
        /// <returns>return a FileInfo</returns>
        public static FileInfo GetFileInfo(string resourcePath)
        {
            FileInfo _fileInfo = null;
            resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);

            if (!Resources.FileExists(resourcePath))
            {
                resourcePath = Path.Combine(applicationBase, resourcePath);
            }

            try
            {
                //argument : The fully qualified name of the new file, or the relative file name. 
                _fileInfo = new FileInfo(resourcePath);
            }
            catch (Exception e)
            {
                throw new ConfigurationException(
                    string.Format("Unable to load file \"{0}\". Cause : \"{1}\"", resourcePath, e.Message), e);
            }
            return _fileInfo;

        }


        /// <summary>
        /// Resolves the supplied type name into a <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The (possibly partially assembly qualified) name of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the type cannot be resolved.
        /// </exception>
        [Obsolete("Use IBatisNet.Common.Utilities.TypeUtils")]
        public static Type TypeForName(string typeName)
        {
            return TypeUtils.ResolveType(typeName);
            //_cachedTypeResolver.Resolve(className);
        }

        #endregion

        #region Inner Class : FileAssemblyInfo
        /// <summary>
        /// Holds data about a <see cref="System.Type"/> and it's
        /// attendant <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        internal class FileAssemblyInfo
        {
            #region Constants
            /// <summary>
            /// The string that separates file name
            /// from their attendant <see cref="System.Reflection.Assembly"/>
            /// names in an assembly qualified type name.
            /// </summary>
            public const string FileAssemblySeparator = ",";
            #endregion

            #region Fields
            private string _unresolvedAssemblyName = string.Empty;
            private string _unresolvedFileName = string.Empty;
            private string _originalFileName = string.Empty;
            #endregion

            #region Properties

            /// <summary>
            /// The resource file name .
            /// </summary>
            public string ResourceFileName
            {
                get { return AssemblyName + "." + FileName; }
            }

            /// <summary>
            /// The original name.
            /// </summary>
            public string OriginalFileName
            {
                get { return _originalFileName; }
            }

            /// <summary>
            /// The file name portion.
            /// </summary>
            public string FileName
            {
                get { return _unresolvedFileName; }
            }

            /// <summary>
            /// The (unresolved, possibly partial) name of the attandant assembly.
            /// </summary>
            public string AssemblyName
            {
                get { return _unresolvedAssemblyName; }
            }

            /// <summary>
            /// Is the type name being resolved assembly qualified?
            /// </summary>
            public bool IsAssemblyQualified
            {
                get
                {
                    if (AssemblyName == null || AssemblyName.Trim().Length == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            #endregion

            #region Constructor (s) / Destructor
            /// <summary>
            /// Creates a new instance of the FileAssemblyInfo class.
            /// </summary>
            /// <param name="unresolvedFileName">
            /// The unresolved name of a <see cref="System.Type"/>.
            /// </param>
            public FileAssemblyInfo(string unresolvedFileName)
            {
                SplitFileAndAssemblyNames(unresolvedFileName);
            }
            #endregion

            #region Methods
            /// <summary>
            /// 
            /// </summary>
            /// <param name="originalFileName"></param>
            private void SplitFileAndAssemblyNames(string originalFileName)
            {
                _originalFileName = originalFileName;

                int separatorIndex = originalFileName.IndexOf(FileAssemblyInfo.FileAssemblySeparator);

                if (separatorIndex < 0)
                {
                    _unresolvedFileName = originalFileName.Trim();
                    _unresolvedAssemblyName = null; // IsAssemblyQualified will return false
                }
                else
                {
                    _unresolvedFileName = originalFileName.Substring(0, separatorIndex).Trim();
                    _unresolvedAssemblyName = originalFileName.Substring(separatorIndex + 1).Trim();
                }
            }
            #endregion

        }
        #endregion
    }
}
