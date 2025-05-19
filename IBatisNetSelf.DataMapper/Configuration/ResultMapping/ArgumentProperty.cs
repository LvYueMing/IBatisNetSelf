using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using IBatisNetSelf.DataMapper.TypeHandlers;
using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Configuration;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// Summary description for ArgumentProperty.
    /// </summary>
    [Serializable]
    [XmlRoot("argument", Namespace = "http://ibatis.apache.org/mapping")]
    public class ArgumentProperty : ResultProperty
    {

        #region Fields
        [NonSerialized]
        private string argumentName = string.Empty;
        [NonSerialized]
        private Type argumentType = null;
        [NonSerialized]
        private IArgumentStrategy argumentStrategy = null;
        #endregion

        #region Properties

        /// <summary>
        /// Sets or gets the <see cref="IArgumentStrategy"/> used to fill the object property.
        /// </summary>
        [XmlIgnore]
        public override IArgumentStrategy ArgumentStrategy
        {
            set { argumentStrategy = value; }
            get { return argumentStrategy; }
        }

        /// <summary>
        /// Specify the constructor argument name.
        /// </summary>
        [XmlAttribute("argumentName")]
        public string ArgumentName
        {
            get { return argumentName; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The name attribute is mandatory in a argument tag.");
                }
                argumentName = value;
            }
        }

        /// <summary>
        /// Tell us if we must lazy load this property..
        /// </summary>
        [XmlAttribute("lazyLoad")]
        public override bool IsLazyLoad
        {
            get { return false; }
            set { throw new InvalidOperationException("Argument property cannot be lazy load."); }
        }

        /// <summary>
        /// Get the argument type
        /// </summary>
        [XmlIgnore]
        public override Type MemberType
        {
            get { return argumentType; }
        }

        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public ArgumentProperty()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initialize the argument property.
        /// </summary>
        /// <param name="aConstructorInfo"></param>
        /// <param name="aConfigScope"></param>
        public void Initialize(ConfigurationScope aConfigScope, ConstructorInfo aConstructorInfo)
        {
            // Search argument by his name to set his type
            ParameterInfo[] _parameters = aConstructorInfo.GetParameters();

            bool _found = false;
            for (int i = 0; i < _parameters.Length; i++)
            {
                _found = (_parameters[i].Name == this.argumentName);
                if (_found)
                {
                    this.argumentType = _parameters[i].ParameterType;
                    break;
                }
            }
            if (this.CallBackName != null && this.CallBackName.Length > 0)
            {
                aConfigScope.ErrorContext.MoreInfo = "Argument property (" + argumentName + "), check the typeHandler attribute '" + this.CallBackName + "' (must be a ITypeHandlerCallback implementation).";
                try
                {
                    Type _type = aConfigScope.SqlMapper.TypeHandlerFactory.GetType(this.CallBackName);
                    ITypeHandlerCallback _typeHandlerCallback = (ITypeHandlerCallback)Activator.CreateInstance(_type);
                    this.TypeHandler = new CustomTypeHandler(_typeHandlerCallback);
                }
                catch (Exception e)
                {
                    throw new IBatisConfigException("Error occurred during custom type handler configuration.  Cause: " + e.Message, e);
                }
            }
            else
            {
                aConfigScope.ErrorContext.MoreInfo = "Argument property (" + argumentName + ") set the typeHandler attribute.";
                this.TypeHandler = this.ResolveTypeHandler(aConfigScope, argumentType, this.CLRType, this.DbType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configScope"></param>
        /// <param name="argumenType">The argument type</param>
        /// <param name="clrType"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ITypeHandler ResolveTypeHandler(ConfigurationScope configScope, Type argumenType, string clrType, string dbType)
        {
            ITypeHandler handler = null;
            if (argumenType == null)
            {
                handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
            }
            else if (typeof(IDictionary).IsAssignableFrom(argumenType))
            {
                // IDictionary
                if (clrType == null || clrType.Length == 0)
                {
                    handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    try
                    {
                        Type type = TypeUtils.ResolveType(clrType);
                        handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                    }
                    catch (Exception e)
                    {
                        throw new IBatisConfigException("Error. Could not set TypeHandler.  Cause: " + e.Message, e);
                    }
                }
            }
            else if (configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType) != null)
            {
                // Primitive
                handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(argumenType, dbType);
            }
            else
            {
                // .NET object
                if (clrType == null || clrType.Length == 0)
                {
                    handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    try
                    {
                        Type type = TypeUtils.ResolveType(clrType);
                        handler = configScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(type, dbType);
                    }
                    catch (Exception e)
                    {
                        throw new IBatisConfigException("Error. Could not set TypeHandler.  Cause: " + e.Message, e);
                    }
                }
            }

            return handler;
        }
        #endregion
    }
}
