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
    /// Summary description for IterateSerializer.
    /// </summary>
    public sealed class IterateSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IterateSerializer(ConfigurationScope configScope)
        {
            _configScope = configScope;
        }

        #region IDeSerializer Members

        /// <summary>
        /// Deserialize a Iterate object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SqlTag Deserialize(XmlNode node)
        {
            Iterate iterate = new Iterate(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            iterate.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            iterate.Property = XmlNodeUtils.GetStringAttribute(prop, "property");
            iterate.Close = XmlNodeUtils.GetStringAttribute(prop, "close");
            iterate.Conjunction = XmlNodeUtils.GetStringAttribute(prop, "conjunction");
            iterate.Open = XmlNodeUtils.GetStringAttribute(prop, "open");

            return iterate;
        }

        #endregion
    }
}
