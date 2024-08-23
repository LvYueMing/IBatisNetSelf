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
    /// Summary description for StatementDeSerializer.
    /// </summary>
    public sealed class StatementDeSerializer
    {
        /// <summary>
        /// Deserialize a Procedure object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static Statement Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            Statement statement = new Statement();
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            statement.CacheModelName = XmlNodeUtils.GetStringAttribute(prop, "cacheModel");
            statement.ExtendStatement = XmlNodeUtils.GetStringAttribute(prop, "extends");
            statement.Id = XmlNodeUtils.GetStringAttribute(prop, "id");
            statement.ListClassName = XmlNodeUtils.GetStringAttribute(prop, "listClass");
            statement.ParameterClassName = XmlNodeUtils.GetStringAttribute(prop, "parameterClass");
            statement.ParameterMapName = XmlNodeUtils.GetStringAttribute(prop, "parameterMap");
            statement.ResultClassName = XmlNodeUtils.GetStringAttribute(prop, "resultClass");
            statement.ResultMapName = XmlNodeUtils.GetStringAttribute(prop, "resultMap");
            statement.AllowRemapping = XmlNodeUtils.GetBooleanAttribute(prop, "remapResults", false);

            return statement;
        }
    }
}
