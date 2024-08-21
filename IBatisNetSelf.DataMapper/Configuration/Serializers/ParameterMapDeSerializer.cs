using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
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
    /// Summary description for ParameterMapDeSerializer.
    /// </summary>
    public sealed class ParameterMapDeSerializer
    {
        /// <summary>
        /// Deserialize a ParameterMap object
        /// </summary>
        /// <param name="aParameterMapNode"></param>
        /// <param name="aConfigScope"></param>
        /// <returns></returns>
        public static ParameterMap Deserialize(XmlNode aParameterMapNode, ConfigurationScope aConfigScope)
        {
            ParameterMap _parameterMap = new ParameterMap(aConfigScope.DataExchangeFactory);
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aParameterMapNode, aConfigScope.Properties);

            aConfigScope.ErrorContext.MoreInfo = "ParameterMap DeSerializer";

            _parameterMap.ExtendMap = XmlNodeUtils.GetStringAttribute(_prop, "extends");
            _parameterMap.Id = XmlNodeUtils.GetStringAttribute(_prop, "id");
            _parameterMap.ClassName = XmlNodeUtils.GetStringAttribute(_prop, "class");

            aConfigScope.ErrorContext.MoreInfo = "Initialize ParameterMap";
            aConfigScope.NodeContext = aParameterMapNode;
            _parameterMap.Initialize(aConfigScope.DataSource.DbProvider.UsePositionalParameters, aConfigScope);
            _parameterMap.BuildProperties(aConfigScope);
            aConfigScope.ErrorContext.MoreInfo = string.Empty;

            return _parameterMap;
        }
    }
}
