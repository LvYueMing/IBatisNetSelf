using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Logging
{
    /// <summary>
    /// Container used to hold configuration information from config file.
    /// </summary>
    internal class LogSetting
    {

        #region Fields

        private Type? factoryAdapterType = null;
        private NameValueCollection? properties = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aFactoryAdapterType">
        /// The <see cref="ILoggerFactoryAdapter" /> type 
        /// that will be used for creating <see cref="ILog" />
        /// </param>
        /// <param name="aProperties">
        /// Additional user supplied properties that are passed to the 
        /// <paramref name="aFactoryAdapterType" />'s constructor.
        /// </param>
        public LogSetting(Type aFactoryAdapterType, NameValueCollection aProperties)
        {
            this.factoryAdapterType = aFactoryAdapterType;
            this.properties = aProperties;
        }

        /// <summary>
        /// The <see cref="ILoggerFactoryAdapter" /> type that will be used for creating <see cref="ILog" /> instances.
        /// </summary>
        public Type? FactoryAdapterType
        {
            get { return this.factoryAdapterType; }
        }

        /// <summary>
        /// Additional user supplied properties that are passed to the <see cref="FactoryAdapterType" />'s constructor.
        /// </summary>
        public NameValueCollection? Properties
        {
            get { return this.properties; }
        }
    }
}
