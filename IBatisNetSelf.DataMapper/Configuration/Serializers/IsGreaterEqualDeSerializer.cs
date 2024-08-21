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
    /// Summary description for IsGreaterEqualDeSerializer.
    /// </summary>
    public sealed class IsGreaterEqualDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsGreaterEqualDeSerializer(ConfigurationScope configScope)
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
            IsGreaterEqual isGreaterEqual = new IsGreaterEqual(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isGreaterEqual.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isGreaterEqual.Property = XmlNodeUtils.GetStringAttribute(prop, "property");
            isGreaterEqual.CompareProperty = XmlNodeUtils.GetStringAttribute(prop, "compareProperty");
            isGreaterEqual.CompareValue = XmlNodeUtils.GetStringAttribute(prop, "compareValue");

            return isGreaterEqual;
        }

        #endregion
    }
}
