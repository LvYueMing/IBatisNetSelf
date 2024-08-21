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
    /// Summary description for IsGreaterThanDeSerializer.
    /// </summary>
    public sealed class IsGreaterThanDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsGreaterThanDeSerializer(ConfigurationScope configScope)
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
            IsGreaterThan isGreaterThan = new IsGreaterThan(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isGreaterThan.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isGreaterThan.Property = XmlNodeUtils.GetStringAttribute(prop, "property");
            isGreaterThan.CompareProperty = XmlNodeUtils.GetStringAttribute(prop, "compareProperty");
            isGreaterThan.CompareValue = XmlNodeUtils.GetStringAttribute(prop, "compareValue");

            return isGreaterThan;
        }

        #endregion
    }
}
