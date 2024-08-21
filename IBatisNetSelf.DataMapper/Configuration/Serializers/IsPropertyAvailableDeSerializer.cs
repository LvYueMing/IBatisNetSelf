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
    /// Summary description for IsPropertyAvailableDeSerializer.
    /// </summary>
    public sealed class IsPropertyAvailableDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsPropertyAvailableDeSerializer(ConfigurationScope configScope)
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
            IsPropertyAvailable isPropertyAvailable = new IsPropertyAvailable(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isPropertyAvailable.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isPropertyAvailable.Property = XmlNodeUtils.GetStringAttribute(prop, "property");

            return isPropertyAvailable;
        }

        #endregion
    }
}
