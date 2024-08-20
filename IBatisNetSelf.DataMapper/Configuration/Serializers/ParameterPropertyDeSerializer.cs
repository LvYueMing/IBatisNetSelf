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
    /// Summary description for ParameterPropertyDeSerializer.
    /// </summary>
    public sealed class ParameterPropertyDeSerializer
    {
        /// <summary>
        /// Deserialize a ResultMap object
        /// </summary>
        /// <param name="aParameterNode"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static ParameterProperty Deserialize(XmlNode aParameterNode, ConfigurationScope configScope)
        {
            ParameterProperty _property = new ParameterProperty();
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aParameterNode, configScope.Properties);

            configScope.ErrorContext.MoreInfo = "ParameterPropertyDeSerializer";

            _property.CallBackName = XmlNodeUtils.GetStringAttribute(_prop, "typeHandler");
            _property.CLRType = XmlNodeUtils.GetStringAttribute(_prop, "type");
            _property.ColumnName = XmlNodeUtils.GetStringAttribute(_prop, "column");
            _property.DbType = XmlNodeUtils.GetStringAttribute(_prop, "dbType", null);
            _property.DirectionAttribute = XmlNodeUtils.GetStringAttribute(_prop, "direction");
            _property.NullValue = _prop["nullValue"];
            _property.PropertyName = XmlNodeUtils.GetStringAttribute(_prop, "property");
            _property.Precision = XmlNodeUtils.GetByteAttribute(_prop, "precision", 0);
            _property.Scale = XmlNodeUtils.GetByteAttribute(_prop, "scale", 0);
            _property.Size = XmlNodeUtils.GetIntAttribute(_prop, "size", -1);

            return _property;
        }
    }
}
