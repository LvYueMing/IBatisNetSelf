using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Xml;
using IBatisNetSelf.DataMapper.Configuration.Alias;

namespace IBatisNetSelf.DataMapper.Configuration.Serializers
{
    /// <summary>
    /// Summary description for TypeHandlerDeSerializer.
    /// </summary>
    public sealed class TypeHandlerDeSerializer
    {
        /// <summary>
        /// Deserialize a TypeHandler object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configScope"></param>
        /// <returns></returns>
        public static void Deserialize(XmlNode node, ConfigurationScope configScope)
        {
            TypeHandler handler = new TypeHandler();

            NameValueCollection prop = XmlNodeUtils.ParseAttributes(node, configScope.Properties);
            handler.CallBackName = XmlNodeUtils.GetStringAttribute(prop, "callback");
            handler.ClassName = XmlNodeUtils.GetStringAttribute(prop, "type");
            handler.DbType = XmlNodeUtils.GetStringAttribute(prop, "dbType");

            handler.Initialize();

            configScope.ErrorContext.MoreInfo = "Check the callback attribute '" + handler.CallBackName + "' (must be a classname).";
            ITypeHandler typeHandler = null;
            Type type = configScope.SqlMapper.TypeHandlerFactory.GetType(handler.CallBackName);
            object impl = Activator.CreateInstance(type);
            if (impl is ITypeHandlerCallback)
            {
                typeHandler = new CustomTypeHandler((ITypeHandlerCallback)impl);
            }
            else if (impl is ITypeHandler)
            {
                typeHandler = (ITypeHandler)impl;
            }
            else
            {
                throw new IBatisConfigException("The callBack type is not a valid implementation of ITypeHandler or ITypeHandlerCallback");
            }

            // 
            configScope.ErrorContext.MoreInfo = "Check the type attribute '" + handler.ClassName + "' (must be a class name) or the dbType '" + handler.DbType + "' (must be a DbType type name).";
            if (handler.DbType != null && handler.DbType.Length > 0)
            {
                configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(handler.ClassName), handler.DbType, typeHandler);
            }
            else
            {
                configScope.DataExchangeFactory.TypeHandlerFactory.Register(TypeUtils.ResolveType(handler.ClassName), typeHandler);
            }
        }
    }
}
