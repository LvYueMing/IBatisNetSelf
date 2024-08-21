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
    /// Summary description for IsLessEqualDeSerializer.
    /// </summary>
    public sealed class IsLessEqualDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsLessEqualDeSerializer(ConfigurationScope configScope)
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
            IsLessEqual isLessEqual = new IsLessEqual(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isLessEqual.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isLessEqual.Property = XmlNodeUtils.GetStringAttribute(prop, "property");
            isLessEqual.CompareProperty = XmlNodeUtils.GetStringAttribute(prop, "compareProperty");
            isLessEqual.CompareValue = XmlNodeUtils.GetStringAttribute(prop, "compareValue");

            return isLessEqual;
        }

        #endregion
    }
}
