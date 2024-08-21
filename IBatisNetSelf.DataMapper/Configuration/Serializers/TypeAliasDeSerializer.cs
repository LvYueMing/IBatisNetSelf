using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Alias;
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
    /// Summary description for TypeAliasDeSerializer.
    /// </summary>
    public sealed class TypeAliasDeSerializer
    {
        /// <summary>
        /// Deserialize a TypeAlias object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            TypeAlias typeAlias = new TypeAlias();
            configScope.ErrorContext.MoreInfo = "loading type alias";

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            typeAlias.Name = XmlNodeUtils.GetStringAttribute(prop, "alias");
            typeAlias.ClassName = XmlNodeUtils.GetStringAttribute(prop, "type");

            configScope.ErrorContext.ObjectId = typeAlias.ClassName;
            configScope.ErrorContext.MoreInfo = "initialize type alias";

            typeAlias.Initialize();

            configScope.SqlMapper.TypeHandlerFactory.AddTypeAlias(typeAlias.Name, typeAlias);
        }
    }
}
