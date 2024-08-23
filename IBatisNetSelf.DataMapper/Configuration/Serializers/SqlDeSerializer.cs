using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Xml;
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
    public sealed class SqlDeSerializer
    {
        /// <summary>
        /// Deserialize a sql tag
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);

            string id = XmlNodeUtils.GetStringAttribute(prop, "id");

            if (configScope.UseStatementNamespaces)
            {
                id = configScope.ApplyNamespace(id);
            }
            if (configScope.SqlIncludes.Contains(id))
            {
                throw new ConfigurationException("Duplicate <sql>-include '" + id + "' found.");
            }
            else
            {
                configScope.SqlIncludes.Add(id, node);
            }
        }
    }
}
