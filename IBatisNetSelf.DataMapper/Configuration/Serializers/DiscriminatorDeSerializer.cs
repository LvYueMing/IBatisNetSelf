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
    /// Summary description for DiscriminatorDeSerializer.
    /// </summary>
    public sealed class DiscriminatorDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultMap object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Discriminator Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Discriminator discriminator = new Discriminator();

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            discriminator.CallBackName = XmlNodeUtils.GetStringAttribute(prop, "typeHandler");
            discriminator.CLRType = XmlNodeUtils.GetStringAttribute(prop, "type");
            discriminator.ColumnIndex = XmlNodeUtils.GetIntAttribute(prop, "columnIndex", ResultProperty.UNKNOWN_COLUMN_INDEX);
            discriminator.ColumnName = XmlNodeUtils.GetStringAttribute(prop, "column");
            discriminator.DbType = XmlNodeUtils.GetStringAttribute(prop, "dbType");
            discriminator.NullValue = prop["nullValue"];

            return discriminator;
        }
    }
}
