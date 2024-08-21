using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.DataMapper.Configuration.Serializers
{
    /// <summary>
    /// Summary description for IsNotPropertyAvailableDeSerializer.
    /// </summary>
    public sealed class IsNotPropertyAvailableDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsNotPropertyAvailableDeSerializer(ConfigurationScope configScope)
        {
            _configScope = configScope;
        }

        #region IDeSerializer Members

        /// <summary>
        /// Deserialize a IsNotEmpty object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SqlTag Deserialize(XmlNode node)
        {
            IsNotPropertyAvailable isNotPropertyAvailable = new IsNotPropertyAvailable(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isNotPropertyAvailable.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isNotPropertyAvailable.Property = XmlNodeUtils.GetStringAttribute(prop, "property");

            return isNotPropertyAvailable;
        }

        #endregion
    }
}
