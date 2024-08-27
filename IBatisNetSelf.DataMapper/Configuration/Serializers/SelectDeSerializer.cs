using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Statements;
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
    /// Summary description for SelectDeSerializer.
    /// </summary>
    public sealed class SelectDeSerializer
    {
        /// <summary>
        /// Deserialize a Procedure object
        /// </summary>
        /// <param name="aXmlNode"></param>
        /// <param name="aConfigScope"></param>
        /// <returns></returns>
        public static Select Deserialize(XmlNode aXmlNode, ConfigurationScope aConfigScope)
        {
            Select _select = new Select();
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aXmlNode, aConfigScope.Properties);

            _select.Id = XmlNodeUtils.GetStringAttribute(_prop, "id");
            _select.CacheModelName = XmlNodeUtils.GetStringAttribute(_prop, "cacheModel");
            _select.ExtendStatement = XmlNodeUtils.GetStringAttribute(_prop, "extends");
            _select.ListClassName = XmlNodeUtils.GetStringAttribute(_prop, "listClass");
            _select.ParameterClassName = XmlNodeUtils.GetStringAttribute(_prop, "parameterClass");
            _select.ParameterMapName = XmlNodeUtils.GetStringAttribute(_prop, "parameterMap");
            _select.ResultClassName = XmlNodeUtils.GetStringAttribute(_prop, "resultClass");
            _select.ResultMapName = XmlNodeUtils.GetStringAttribute(_prop, "resultMap");
            _select.AllowRemapping = XmlNodeUtils.GetBooleanAttribute(_prop, "remapResults", false);

            int _count = aXmlNode.ChildNodes.Count;
            for (int i = 0; i < _count; i++)
            {
                if (aXmlNode.ChildNodes[i].LocalName == "generate")
                {
                    Generate _generate = new Generate();
                    NameValueCollection _props = XmlNodeUtils.ParseAttributes(aXmlNode.ChildNodes[i], aConfigScope.Properties);

                    _generate.By = XmlNodeUtils.GetStringAttribute(_props, "by");
                    _generate.Table = XmlNodeUtils.GetStringAttribute(_props, "table");

                    _select.Generate = _generate;
                }
            }
            return _select;
        }
    }
}
