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
    /// Summary description for ArgumentPropertyDeSerializer.
    /// </summary>
    public sealed class ArgumentPropertyDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultProperty object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static ArgumentProperty Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            ArgumentProperty argumentProperty = new ArgumentProperty();

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            argumentProperty.CLRType = XmlNodeUtils.GetStringAttribute(prop, "type");
            argumentProperty.CallBackName = XmlNodeUtils.GetStringAttribute(prop, "typeHandler");
            argumentProperty.ColumnIndex = XmlNodeUtils.GetIntAttribute(prop, "columnIndex", ResultProperty.UNKNOWN_COLUMN_INDEX);
            argumentProperty.ColumnName = XmlNodeUtils.GetStringAttribute(prop, "column");
            argumentProperty.DbType = XmlNodeUtils.GetStringAttribute(prop, "dbType");
            argumentProperty.NestedResultMapName = XmlNodeUtils.GetStringAttribute(prop, "resultMapping");
            argumentProperty.NullValue = prop["nullValue"];
            argumentProperty.ArgumentName = XmlNodeUtils.GetStringAttribute(prop, "argumentName");
            argumentProperty.Select = XmlNodeUtils.GetStringAttribute(prop, "select");

            return argumentProperty;
        }
    }
}
