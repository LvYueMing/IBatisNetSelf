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
    /// Summary description for ProcedureDeSerializer.
    /// </summary>
    public sealed class ProcedureDeSerializer
    {
        /// <summary>
        /// Deserialize a Procedure object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Procedure Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Procedure _procedure = new Procedure();
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            _procedure.CacheModelName = XmlNodeUtils.GetStringAttribute(_prop, "cacheModel");
            _procedure.Id = XmlNodeUtils.GetStringAttribute(_prop, "id");
            _procedure.ListClassName = XmlNodeUtils.GetStringAttribute(_prop, "listClass");
            _procedure.ParameterMapName = XmlNodeUtils.GetStringAttribute(_prop, "parameterMap");
            _procedure.ResultClassName = XmlNodeUtils.GetStringAttribute(_prop, "resultClass");
            _procedure.ResultMapName = XmlNodeUtils.GetStringAttribute(_prop, "resultMap");
            _procedure.ListClassName = XmlNodeUtils.GetStringAttribute(_prop, "listClass");

            return _procedure;
        }
    }
}
