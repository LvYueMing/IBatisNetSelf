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
    /// Summary description for ResultPropertyDeSerializer.
    /// </summary>
    public sealed class ResultPropertyDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultProperty object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static ResultProperty Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            ResultProperty resultProperty = new ResultProperty();

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            resultProperty.CLRType = XmlNodeUtils.GetStringAttribute(prop, "type");
            resultProperty.CallBackName = XmlNodeUtils.GetStringAttribute(prop, "typeHandler");
            resultProperty.ColumnIndex = XmlNodeUtils.GetIntAttribute(prop, "columnIndex", ResultProperty.UNKNOWN_COLUMN_INDEX);
            resultProperty.ColumnName = XmlNodeUtils.GetStringAttribute(prop, "column");
            resultProperty.DbType = XmlNodeUtils.GetStringAttribute(prop, "dbType");
            resultProperty.IsLazyLoad = XmlNodeUtils.GetBooleanAttribute(prop, "lazyLoad", false);
            resultProperty.NestedResultMapName = XmlNodeUtils.GetStringAttribute(prop, "resultMapping");
            resultProperty.NullValue = prop["nullValue"];
            resultProperty.PropertyName = XmlNodeUtils.GetStringAttribute(prop, "property");
            resultProperty.Select = XmlNodeUtils.GetStringAttribute(prop, "select");

            return resultProperty;
        }
    }
}
