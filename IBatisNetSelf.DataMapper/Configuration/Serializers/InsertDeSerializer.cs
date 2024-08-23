using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects.Members;
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
    /// Summary description for InsertDeSerializer.
    /// </summary>
    public sealed class InsertDeSerializer
    {
        /// <summary>
        /// Deserialize a TypeHandler object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Insert Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Insert insert = new Insert();
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            insert.CacheModelName = XmlNodeUtils.GetStringAttribute(prop, "cacheModel");
            insert.ExtendStatement = XmlNodeUtils.GetStringAttribute(prop, "extends");
            insert.Id = XmlNodeUtils.GetStringAttribute(prop, "id");
            insert.ParameterClassName = XmlNodeUtils.GetStringAttribute(prop, "parameterClass");
            insert.ParameterMapName = XmlNodeUtils.GetStringAttribute(prop, "parameterMap");
            insert.ResultClassName = XmlNodeUtils.GetStringAttribute(prop, "resultClass");
            insert.ResultMapName = XmlNodeUtils.GetStringAttribute(prop, "resultMap");
            insert.AllowRemapping = XmlNodeUtils.GetBooleanAttribute(prop, "remapResults", false);

            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "generate")
                {
                    Generate generate = new Generate();
                    NameValueCollection props = XmlNodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);

                    generate.By = XmlNodeUtils.GetStringAttribute(props, "by");
                    generate.Table = XmlNodeUtils.GetStringAttribute(props, "table");

                    insert.Generate = generate;
                }
                else if (node.ChildNodes[i].LocalName == "selectKey")
                {
                    SelectKey selectKey = new SelectKey();
                    NameValueCollection props = XmlNodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);

                    selectKey.PropertyName = XmlNodeUtils.GetStringAttribute(props, "property");
                    selectKey.SelectKeyType = InsertDeSerializer.ReadSelectKeyType(props["type"]);
                    selectKey.ResultClassName = XmlNodeUtils.GetStringAttribute(props, "resultClass");

                    insert.SelectKey = selectKey;
                }
            }
            return insert;
        }

        private static SelectKeyType ReadSelectKeyType(string s)
        {
            switch (s)
            {
                case @"pre": return IBatisNetSelf.DataMapper.SelectKeyType.@pre;
                case @"post": return IBatisNetSelf.DataMapper.SelectKeyType.@post;
                default: throw new ConfigurationException("Unknown selectKey type : '" + s + "'");
            }
        }
    }
}
