using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
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
    /// Summary description for ResultMapDeSerializer.
    /// </summary>
    public sealed class ResultMapDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultMap object
        /// </summary>
        /// <param name="aXmlNode"></param>
        /// <param name="aConfigScope"></param>
        /// <returns></returns>
        public static ResultMap Deserialize(XmlNode aXmlNode, ConfigurationScope aConfigScope)
        {
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aXmlNode, aConfigScope.Properties);
            ResultMap _resultMap = new ResultMap(aConfigScope, _prop["id"], _prop["class"], _prop["extends"], _prop["groupBy"]);

            aConfigScope.ErrorContext.MoreInfo = "initialize ResultMap";

            _resultMap.Initialize(aConfigScope);

            return _resultMap;
        }
    }
}
