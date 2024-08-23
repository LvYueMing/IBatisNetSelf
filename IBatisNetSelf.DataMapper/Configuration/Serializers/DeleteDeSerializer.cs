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
    /// Summary description for DeleteDeSerializer.
    /// </summary>
    public sealed class DeleteDeSerializer
    {
        /// <summary>
        /// Deserialize a TypeHandler object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Delete Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Delete delete = new Delete();
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            delete.CacheModelName = XmlNodeUtils.GetStringAttribute(prop, "cacheModel");
            delete.ExtendStatement = XmlNodeUtils.GetStringAttribute(prop, "extends");
            delete.Id = XmlNodeUtils.GetStringAttribute(prop, "id");
            delete.ListClassName = XmlNodeUtils.GetStringAttribute(prop, "listClass");
            delete.ParameterClassName = XmlNodeUtils.GetStringAttribute(prop, "parameterClass");
            delete.ParameterMapName = XmlNodeUtils.GetStringAttribute(prop, "parameterMap");
            delete.ResultClassName = XmlNodeUtils.GetStringAttribute(prop, "resultClass");
            delete.ResultMapName = XmlNodeUtils.GetStringAttribute(prop, "resultMap");
            delete.AllowRemapping = XmlNodeUtils.GetBooleanAttribute(prop, "remapResults", false);

            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection props = XmlNodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);

                    generate.By = XmlNodeUtils.GetStringAttribute(props, "by");
                    generate.Table = XmlNodeUtils.GetStringAttribute(props, "table");

                    delete.Generate = generate;
                }
            }
            return delete;
        }
    }
}
