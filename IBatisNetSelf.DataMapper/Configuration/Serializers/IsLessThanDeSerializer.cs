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
    /// Summary description for IsLessThanDeSerializer.
    /// </summary>
    public sealed class IsLessThanDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsLessThanDeSerializer(ConfigurationScope configScope)
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
            IsLessThan isLessThan = new IsLessThan(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isLessThan.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isLessThan.Property = XmlNodeUtils.GetStringAttribute(prop, "property");
            isLessThan.CompareProperty = XmlNodeUtils.GetStringAttribute(prop, "compareProperty");
            isLessThan.CompareValue = XmlNodeUtils.GetStringAttribute(prop, "compareValue");

            return isLessThan;
        }

        #endregion
    }
}
