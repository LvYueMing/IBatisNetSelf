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
    /// Summary description for IsNotEmptyDeSerializer.
    /// </summary>
    public sealed class IsNotEmptyDeSerializer : IDeSerializer
    {
        private ConfigurationScope configScope = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aConfigScope"></param>
        public IsNotEmptyDeSerializer(ConfigurationScope aConfigScope)
        {
            this.configScope = aConfigScope;
        }

        #region IDeSerializer Members

        /// <summary>
        /// Deserialize a IsNotEmpty object
        /// </summary>
        /// <param name="aXmlNode"></param>
        /// <returns></returns>
        public SqlTag Deserialize(XmlNode aXmlNode)
        {
            IsNotEmpty _isNotEmpty = new IsNotEmpty(this.configScope.DataExchangeFactory.AccessorFactory);

            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aXmlNode, configScope.Properties);
            _isNotEmpty.Prepend = XmlNodeUtils.GetStringAttribute(_prop, "prepend");
            _isNotEmpty.Property = XmlNodeUtils.GetStringAttribute(_prop, "property");

            return _isNotEmpty;
        }

        #endregion
    }
}
