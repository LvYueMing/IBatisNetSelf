using IBatisNetSelf.Common.Xml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.Common
{
    /// <summary>
    /// Summary description for ProviderDeSerializer.
    /// </summary>
    public sealed class ProviderDeSerializer
    {
        /// <summary>
        /// Deserializes the specified node in a <see cref="IDbProvider"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The <see cref="IDbProvider"/></returns>
        public static IDbProvider Deserialize(XmlNode node)
        {
            IDbProvider _provider = new DbProvider();
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(node);

            _provider.Name = _prop["name"] ?? "";
            _provider.Description = _prop["description"] ?? "";
            _provider.IsDefault = XmlNodeUtils.GetBooleanAttribute(_prop, "default", false);
            _provider.IsEnabled = XmlNodeUtils.GetBooleanAttribute(_prop, "enabled", true);
            _provider.AssemblyName = _prop["assemblyName"] ?? "";
            _provider.CommandBuilderClass = _prop["commandBuilderClass"] ?? "";
            _provider.DbCommandClass = _prop["commandClass"] ?? "";
            _provider.DbConnectionClass = _prop["connectionClass"] ?? "";
            _provider.DataAdapterClass = _prop["dataAdapterClass"] ?? "";
            _provider.ParameterDbTypeClass = _prop["parameterDbTypeClass"] ?? "";
            _provider.ParameterDbTypeProperty = _prop["parameterDbTypeProperty"] ?? "";
            _provider.ParameterPrefix = _prop["parameterPrefix"] ?? "";
            _provider.SetDbParameterPrecision = XmlNodeUtils.GetBooleanAttribute(_prop, "setDbParameterPrecision", true);
            _provider.SetDbParameterScale = XmlNodeUtils.GetBooleanAttribute(_prop, "setDbParameterScale", true);
            _provider.SetDbParameterSize = XmlNodeUtils.GetBooleanAttribute(_prop, "setDbParameterSize", true);
            _provider.UseDeriveParameters = XmlNodeUtils.GetBooleanAttribute(_prop, "useDeriveParameters", true);
            _provider.UseParameterPrefixInParameter = XmlNodeUtils.GetBooleanAttribute(_prop, "useParameterPrefixInParameter", true);
            _provider.UseParameterPrefixInSql = XmlNodeUtils.GetBooleanAttribute(_prop, "useParameterPrefixInSql", true);
            _provider.UsePositionalParameters = XmlNodeUtils.GetBooleanAttribute(_prop, "usePositionalParameters", false);
            _provider.AllowMARS = XmlNodeUtils.GetBooleanAttribute(_prop, "allowMARS", false);


            return _provider;
        }
    }
}
