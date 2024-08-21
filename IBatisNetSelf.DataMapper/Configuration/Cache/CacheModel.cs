using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Cache
{
    /// <summary>
    /// Summary description for CacheModel.
    /// </summary>
    [Serializable]
    [XmlRoot("cacheModel", Namespace = "http://ibatis.apache.org/mapping")]
    public class CacheModel
    {
        #region Fields

        private static IDictionary lockMap = new HybridDictionary();

        [NonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// This is used to represent null objects that are returned from the cache so 
        /// that they can be cached, too.
        /// </summary>
        [NonSerialized]
        public readonly static object NULL_OBJECT = new Object();

        /// <summary>
        /// Constant to turn off periodic cache flushes
        /// </summary>
        [NonSerialized]
        public const long NO_FLUSH_INTERVAL = -99999;

        [NonSerialized]
        private object statLock = new Object();
        [NonSerialized]
        private int requests = 0;
        [NonSerialized]
        private int hits = 0;
        [NonSerialized]
        private string id = string.Empty;
        [NonSerialized]
        private ICacheController controller = null;
        [NonSerialized]
        private FlushInterval flushInterval = null;
        [NonSerialized]
        private long lastFlush = 0;
        [NonSerialized]
        private HybridDictionary properties = new HybridDictionary();
        [NonSerialized]
        private string implementation = string.Empty;
        [NonSerialized]
        private bool isReadOnly = true;
        [NonSerialized]
        private bool isSerializable = false;

        #endregion

        #region Properties
        /// <summary>
        /// Identifier used to identify the CacheModel amongst the others.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if ((value == null) || (value.Length < 1))
                    throw new ArgumentNullException("The id attribute is mandatory in a cacheModel tag.");

                id = value;
            }
        }

        /// <summary>
        /// Cache controller implementation name.
        /// </summary>
        [XmlAttribute("implementation")]
        public string Implementation
        {
            get { return implementation; }
            set
            {
                if ((value == null) || (value.Length < 1))
                    throw new ArgumentNullException("The implementation attribute is mandatory in a cacheModel tag.");

                implementation = value;
            }
        }

        /// <summary>
        /// Set the cache controller
        /// </summary>
        public ICacheController CacheController
        {
            set { controller = value; }
        }

        /// <summary>
        /// Set or get the flushInterval (in Ticks)
        /// </summary>
        [XmlElement("flushInterval", typeof(FlushInterval))]
        public FlushInterval FlushInterval
        {
            get { return flushInterval; }
            set { flushInterval = value; }
        }

        /// <summary>
        /// Specifie how the cache content should be returned.
        /// If true a deep copy is returned.
        /// </summary>
        /// <remarks>
        /// Combinaison
        /// IsReadOnly=true/IsSerializable=false : Returned instance of cached object
        /// IsReadOnly=false/IsSerializable=true : Returned coopy of cached object
        /// </remarks>
        public bool IsSerializable
        {
            get { return isSerializable; }
            set { isSerializable = value; }
        }

        /// <summary>
        /// Determines if the cache will be used as a read-only cache.
        /// Tells the cache model that is allowed to pass back a reference to the object
        /// existing in the cache.
        /// </summary>
        /// <remarks>
        /// The IsReadOnly properties works in conjonction with the IsSerializable propertie.
        /// </remarks>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; }
        }
        #endregion

        #region Constructor (s) / Destructor


        /// <summary>
        /// Constructor
        /// </summary>
        public CacheModel()
        {
            lastFlush = DateTime.Now.Ticks;
        }
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            // Initialize FlushInterval
            flushInterval.Initialize();

            try
            {
                if (implementation == null)
                {
                    throw new DataMapperException("Error instantiating cache controller for cache named '" + id + "'. Cause: The class for name '" + implementation + "' could not be found.");
                }

                // Build the CacheController
                Type type = TypeUtils.ResolveType(implementation);
                object[] arguments = new object[0];

                controller = (ICacheController)Activator.CreateInstance(type, arguments);
            }
            catch (Exception e)
            {
                throw new ConfigurationException("Error instantiating cache controller for cache named '" + id + ". Cause: " + e.Message, e);
            }

            //------------ configure Controller---------------------
            try
            {
                controller.Configure(properties);
            }
            catch (Exception e)
            {
                throw new ConfigurationException("Error configuring controller named '" + id + "'. Cause: " + e.Message, e);
            }
        }


        /// <summary>
        /// Event listener
        /// </summary>
        /// <param name="mappedStatement">A MappedStatement on which we listen ExecuteEventArgs event.</param>
        public void RegisterTriggerStatement(IMappedStatement mappedStatement)
        {
            mappedStatement.Execute += new ExecuteEventHandler(FlushHandler);
        }


        /// <summary>
        /// FlushHandler which clear the cache 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlushHandler(object sender, ExecuteEventArgs e)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug("Flush cacheModel named " + id + " for statement '" + e.StatementName + "'");
            }

            Flush();
        }


        /// <summary>
        /// Clears all elements from the cache.
        /// </summary>
        public void Flush()
        {
            lastFlush = DateTime.Now.Ticks;
            controller.Flush();
        }


        /// <summary>
        /// Adds an item with the specified key and value into cached data.
        /// Gets a cached object with the specified key.
        /// </summary>
        /// <value>The cached object or <c>null</c></value>
        /// <remarks>
        /// A side effect of this method is that is may clear the cache
        /// if it has not been cleared in the flushInterval.
        /// </remarks> 
        public object this[CacheKey key]
        {
            get
            {
                lock (this)
                {
                    if (lastFlush != NO_FLUSH_INTERVAL
                        && (DateTime.Now.Ticks - lastFlush > flushInterval.Interval))
                    {
                        Flush();
                    }
                }

                object value = null;
                lock (GetLock(key))
                {
                    value = controller[key];
                }

                if (isSerializable && !isReadOnly &&
                    (value != NULL_OBJECT && value != null))
                {
                    try
                    {
                        MemoryStream stream = new MemoryStream((byte[])value);
                        stream.Position = 0;
                        BinaryFormatter formatter = new BinaryFormatter();
                        value = formatter.Deserialize(stream);
                    }
                    catch (Exception ex)
                    {
                        throw new IBatisNetSelfException("Error caching serializable object.  Be sure you're not attempting to use " +
                            "a serialized cache for an object that may be taking advantage of lazy loading.  Cause: " + ex.Message, ex);
                    }
                }

                lock (statLock)
                {
                    requests++;
                    if (value != null)
                    {
                        hits++;
                    }
                }

                if (logger.IsDebugEnabled)
                {
                    if (value != null)
                    {
                        logger.Debug(String.Format("Retrieved cached object '{0}' using key '{1}' ", value, key));
                    }
                    else
                    {
                        logger.Debug(String.Format("Cache miss using key '{0}' ", key));
                    }
                }
                return value;
            }
            set
            {
                if (null == value) { value = NULL_OBJECT; }
                if (isSerializable && !isReadOnly && value != NULL_OBJECT)
                {
                    try
                    {
                        MemoryStream stream = new MemoryStream();
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, value);
                        value = stream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        throw new IBatisNetSelfException("Error caching serializable object. Cause: " + ex.Message, ex);
                    }
                }
                controller[key] = value;
                if (logger.IsDebugEnabled)
                {
                    logger.Debug(String.Format("Cache object '{0}' using key '{1}' ", value, key));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double HitRatio
        {
            get
            {
                if (requests != 0)
                {
                    return (double)hits / (double)requests;
                }
                else
                {
                    return 0;
                }
            }
        }


        /// <summary>
        /// Add a property
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        public void AddProperty(string name, string value)
        {
            properties.Add(name, value);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object GetLock(CacheKey key)
        {
            int controllerId = HashCodeProvider.GetIdentityHashCode(controller);
            int keyHash = key.GetHashCode();
            int lockKey = 29 * controllerId + keyHash;
            object lok = lockMap[lockKey];
            if (lok == null)
            {
                lok = lockKey; //might as well use the same object
                lockMap[lockKey] = lok;
            }
            return lok;
        }
    }
}
