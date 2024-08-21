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
    /// Summary description for IsEmptyDeSerializer.
    /// </summary>
    public sealed class IsEmptyDeSerializer : IDeSerializer
    {

        private ConfigurationScope _configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        public IsEmptyDeSerializer(ConfigurationScope configScope)
        {
            _configScope = configScope;
        }

        #region IDeSerializer Members

        /// <summary>
        /// Deserialize a IsNotNull object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SqlTag Deserialize(XmlNode node)
        {
            IsEmpty isEmpty = new IsEmpty(_configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, _configScope.Properties);
            isEmpty.Prepend = XmlNodeUtils.GetStringAttribute(prop, "prepend");
            isEmpty.Property = XmlNodeUtils.GetStringAttribute(prop, "property");

            return isEmpty;
        }

        #endregion
    }
}
