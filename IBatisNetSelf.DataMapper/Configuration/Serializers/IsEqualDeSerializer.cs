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
    /// Summary description for IsEqualDeSerializer.
    /// </summary>
    public sealed class IsEqualDeSerializer : IDeSerializer
    {
        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsEqualDeSerializer(ConfigurationScope configScope)
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
            IsEqual _isEqual = new IsEqual(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            _isEqual.Prepend = XmlNodeUtils.GetStringAttribute(_prop, "prepend");
            _isEqual.Property = XmlNodeUtils.GetStringAttribute(_prop, "property");
            _isEqual.CompareProperty = XmlNodeUtils.GetStringAttribute(_prop, "compareProperty");
            _isEqual.CompareValue = XmlNodeUtils.GetStringAttribute(_prop, "compareValue");

            return _isEqual;
        }

        #endregion
    }
}
