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
    /// Summary description for DynamicDeSerializer.
    /// </summary>
    public sealed class DynamicDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configScope"></param>
        public DynamicDeSerializer(ConfigurationScope configScope)
        {
            _configScope = configScope;
        }


        #region IDeSerializer Members

        /// <summary>
        /// Deserialize a Dynamic object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SqlTag Deserialize(XmlNode node)
        {
            Dynamic dynamic = new Dynamic(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            dynamic.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");

            return dynamic;
        }

        #endregion
    }
}
