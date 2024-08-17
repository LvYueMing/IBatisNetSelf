using IBatisNetSelf.Common.Logging.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// Used in an application's configuration file (App.Config or Web.Config) to configure the logging subsystem.
    /// </summary>
    /// <remarks>
    /// <example>
    /// An example configuration section that writes IBatisNet messages to the Console using the built-in Console Logger.
    /// <code lang="XML" escaped="true">
    /// <configuration>
    ///     <!-- 配置节处理程序的声明区。 -->
    ///		<configSections>
    ///			<sectionGroup name="IBatisNetSelf">
    ///				<section name="Logging" type="IBatisNetSelf.Common.Logging.ConfigurationSectionHandler, IBatisNetSelf.Common" />
    ///			</sectionGroup>
    ///			<!-- 其他的 <section> 和 <sectionGroup> 元素。 -->
    ///		</configSections>
    ///		<!-- 配置节的设定区。 -->
    ///		<IBatisNetSelf>
    ///			<Logging>
    ///				<LogFactoryAdapter type="IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNetSelf.Common">
    ///					<arg key="showLogName" value="true" />
    ///					<arg key="showDataTime" value="true" />
    ///					<arg key="level" value="ALL" />
    ///					<arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:fff" />
    ///					<!--All,Debug,Info,Warn,Error,Fatal,Off-->
    ///			        <arg key = "level" value="Info" />
    ///				</logFactoryAdapter>
    ///			</Logging>
    ///		</iBATIS>
    /// </IBatisNetSelf>
    /// </code> 
    /// </example>
    /// <para>
    /// The following aliases are recognized for the type attribute of logFactoryAdapter: 
    /// </para>
    /// <list type="table">
    /// <item><term>CONSOLE</term><description>Alias for IBatisNetSelf.Common.Logging.Impl.ConsoleOutLoggerFA, IBatisNetSelf.Common</description></item>
    /// <item><term>TRACE</term><description>Alias for IBatisNetSelf.Common.Logging.Impl.TraceLoggerFA, IBatisNetSelf.Common</description></item>
    /// <item><term>NOOP</term><description>Alias IBatisNetSelf.Common.Logging.Impl.NoOpLoggerFA, IBatisNetSelf.Common</description></item>
    /// </list>
    /// </remarks>
    public class ConfigurationSectionHandler : ConfigurationSection
    {
        #region Fields		
        private static readonly string LOGFACTORYADAPTER_ELEMENT = "LogFactoryAdapter";
        private static readonly string LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB = "type";
        private static readonly string ARGUMENT_ELEMENT = "arg";
        private static readonly string ARGUMENT_ELEMENT_KEY_ATTRIB = "key";
        private static readonly string ARGUMENT_ELEMENT_VALUE_ATTRIB = "value";

        #endregion

        #region Property	

        [ConfigurationProperty("LogFactoryAdapter")]
        private LogFactoryAdapterEelement LogFactoryAdapter
        {
            get { return (LogFactoryAdapterEelement)base["LogFactoryAdapter"]; }
            set { base["LogFactoryAdapter"] = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigurationSectionHandler()
        {
        }

        /// <summary>
        /// Retrieves the <see cref="Type" /> of the logger the use by looking at the logFactoryAdapter element
        /// of the logging configuration element.
        /// </summary>
        /// <param name="aSection"></param>
        /// <returns>
        /// A <see cref="LogSetting" /> object containing the specified type that implements 
        /// <see cref="ILoggerFactoryAdapter" /> along with zero or more properties that will be 
        /// passed to the logger factory adapter's constructor as an <see cref="IDictionary" />.
        /// </returns>
        private LogSetting ReadConfiguration()
        {
            string _factoryTypeString = this.LogFactoryAdapter.Type;
            if (_factoryTypeString == string.Empty)
            {
                throw new System.Configuration.ConfigurationErrorsException
                    ("Required Attribute '"
                    + LOGFACTORYADAPTER_ELEMENT_TYPE_ATTRIB
                    + "' not found in element '"
                    + LOGFACTORYADAPTER_ELEMENT
                    + "'"
                    );
            }

            Type? _factoryType = null;
            try
            {
                if (String.Compare(_factoryTypeString, "CONSOLE", true) == 0)
                {
                    _factoryType = typeof(ConsoleOutLoggerFactory);
                }
                else if (String.Compare(_factoryTypeString, "TRACE", true) == 0)
                {
                    _factoryType = typeof(TraceLoggerFactory);
                }
                else if (String.Compare(_factoryTypeString, "NOOP", true) == 0)
                {
                    _factoryType = typeof(NoOpLoggerFactory);
                }
                else if (String.Compare(_factoryTypeString, "FILE", true) == 0)
                {
                    _factoryType = typeof(FileTxtLoggerFactory);
                }
                else
                {
                    _factoryType = Type.GetType(_factoryTypeString, true, false);
                }
            }
            catch (Exception e)
            {
                throw new System.Configuration.ConfigurationErrorsException
                    ("Unable to create type '" + _factoryTypeString + "'", e);
            }


            NameValueCollection _properties = new NameValueCollection(StringComparer.OrdinalIgnoreCase);


            foreach (ArgElement arg in this.LogFactoryAdapter.ArgElements)
            {
                string key = arg.Key;
                string itsValue = arg.Value;

                if (string.IsNullOrEmpty(key))
                {
                    throw new System.Configuration.ConfigurationErrorsException
                        ("Required Attribute '"
                          + ARGUMENT_ELEMENT_KEY_ATTRIB
                          + "' not found in element '"
                          + ARGUMENT_ELEMENT
                          + "'"
                        );
                }

                _properties.Add(key, itsValue);
            }

            return new LogSetting(_factoryType, _properties);
        }


        #region ConfigurationSection Members

        protected override object? GetRuntimeObject()
        {
            if (this.LogFactoryAdapter != null)
            {
                return ReadConfiguration();
            }
            else
            {
                return null;
            }
        }

        #endregion


        #region Internal Class
        private sealed class LogFactoryAdapterEelement : ConfigurationElement
        {
            [ConfigurationProperty("type", IsRequired = true)]
            public string Type
            {
                get { return (string)base["type"]; }
                set { base["type"] = value; }
            }

            //没有对应的xml元素，所有集合NAME=""
            [ConfigurationProperty("", IsDefaultCollection = true)]
            public ArgElementCollection ArgElements
            {
                get { return (ArgElementCollection)base[""]; }
            }
        }


        private sealed class ArgElementCollection : ConfigurationElementCollection
        {
            //ConfigurationElementCollection的抽象方法，必须实现
            protected override ConfigurationElement CreateNewElement()
            {
                return new ArgElement();
            }

            //ConfigurationElementCollection的抽象方法，必须实现
            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((ArgElement)element).Key;
            }

            //指定集合类型
            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }

            //定义集合元素对应的xml元素
            protected override string ElementName
            {
                get
                {
                    return "arg";
                }
            }

            //获取集合元素
            public ArgElement this[int index]
            {
                get
                {
                    return (ArgElement)BaseGet(index);
                }
                set
                {
                    if (BaseGet(index) != null)
                    {
                        BaseRemoveAt(index);
                    }
                    BaseAdd(index, value);
                }
            }
        }

        private sealed class ArgElement : ConfigurationElement
        {
            [ConfigurationProperty("key", IsKey = true)]
            public string Key
            {
                get { return (string)base["key"]; }
                set { base["key"] = value; }
            }

            [ConfigurationProperty("value")]
            public string Value
            {
                get { return (string)base["value"]; }
                set { base["value"] = value; }
            }
        }

        #endregion
    }
}
