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
    /// Summary description for SubMapDeSerializer.
    /// </summary>
    public sealed class SubMapDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultMap object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static SubMap Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            string discriminatorValue = XmlNodeUtils.GetStringAttribute(prop, "value");
            string resultMapName = configScope.ApplyNamespace(XmlNodeUtils.GetStringAttribute(prop, "resultMapping"));

            return new SubMap(discriminatorValue, resultMapName);
        }
    }
}
