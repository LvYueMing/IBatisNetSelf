using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Cache;
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
    /// Summary description for CacheModelDeSerializer.
    /// </summary>
    public sealed class CacheModelDeSerializer
    {
        /// <summary>
        /// Deserialize a CacheModel object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static CacheModel Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            CacheModel model = new CacheModel();

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            model.Id = XmlNodeUtils.GetStringAttribute(prop, "id");
            model.Implementation = XmlNodeUtils.GetStringAttribute(prop, "implementation");
            model.Implementation = configScope.SqlMapper.TypeHandlerFactory.GetTypeAlias(model.Implementation).Type.AssemblyQualifiedName;
            model.IsReadOnly = XmlNodeUtils.GetBooleanAttribute(prop, "readOnly", true);
            model.IsSerializable = XmlNodeUtils.GetBooleanAttribute(prop, "serialize", false);

            int count = node.ChildNodes.Count;
            for (int i = 0; i < count; i++)
            {
                if (node.ChildNodes[i].LocalName == "flushInterval")
                {
                    FlushInterval flush = new FlushInterval();
                    NameValueCollection props = XmlNodeUtils.ParseAttributes(node.ChildNodes[i], configScope.Properties);
                    flush.Hours = XmlNodeUtils.GetIntAttribute(props, "hours", 0);
                    flush.Milliseconds = XmlNodeUtils.GetIntAttribute(props, "milliseconds", 0);
                    flush.Minutes = XmlNodeUtils.GetIntAttribute(props, "minutes", 0);
                    flush.Seconds = XmlNodeUtils.GetIntAttribute(props, "seconds", 0);

                    model.FlushInterval = flush;
                }
            }

            return model;
        }
    }
}
