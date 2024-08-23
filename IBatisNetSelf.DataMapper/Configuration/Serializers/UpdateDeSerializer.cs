using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using Microsoft.VisualBasic;
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
    /// Summary description for UpdateDeSerializer.
    /// </summary>
    public sealed class UpdateDeSerializer
    {
        /// <summary>
        /// Deserialize a Procedure object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Update Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Update update = new Update();
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            update.CacheModelName = XmlNodeUtils.GetStringAttribute(prop, "cacheModel");
            update.ExtendStatement = XmlNodeUtils.GetStringAttribute(prop, "extends");
            update.Id = XmlNodeUtils.GetStringAttribute(prop, "id");
            update.ParameterClassName = XmlNodeUtils.GetStringAttribute(prop, "parameterClass");
            update.ParameterMapName = XmlNodeUtils.GetStringAttribute(prop, "parameterMap");
            update.AllowRemapping = XmlNodeUtils.GetBooleanAttribute(prop, "remapResults", false);

            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection props = XmlNodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);

                    generate.By = XmlNodeUtils.GetStringAttribute(props, "by");
                    generate.Table = XmlNodeUtils.GetStringAttribute(props, "table");

                    update.Generate = generate;
                }
            }
            return update;
        }
    }
}
