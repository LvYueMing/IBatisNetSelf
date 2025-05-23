using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy;
using IBatisNetSelf.DataMapper.Proxy;
using IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy;
using IBatisNetSelf.DataMapper.Scope;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// Summary description for ResultProperty.
    /// </summary>
    [Serializable]
    [XmlRoot("result", Namespace = "http://ibatis.apache.org/mapping")]
    public class ResultProperty
    {
        #region Const
        
        public const int UNKNOWN_COLUMN_INDEX = -999999;

        #endregion

        #region Fields
        [NonSerialized]
        private ISetAccessor setAccessor = null;
        [NonSerialized]
        private string nullValue = null;
        [NonSerialized]
        private string propertyName = string.Empty;
        [NonSerialized]
        private string columnName = string.Empty;
        [NonSerialized]
        private int columnIndex = UNKNOWN_COLUMN_INDEX;
        [NonSerialized]
        private string select = string.Empty;
        [NonSerialized]
        private string nestedResultMapName = string.Empty;
        [NonSerialized]
        private IResultMap nestedResultMap = null;
        [NonSerialized]
        private string dbType = null;
        [NonSerialized]
        private string clrType = string.Empty;
        [NonSerialized]
        private bool isLazyLoad = false;
        [NonSerialized]
        private ITypeHandler typeHandler = null;
        [NonSerialized]
        private string callBackName = string.Empty;
        [NonSerialized]
        private bool isComplexMemberName = false;
        [NonSerialized]
        private IPropertyStrategy propertyStrategy = null;
        [NonSerialized]
        private ILazyFactory lazyFactory = null;
        [NonSerialized]
        private bool isIList = false;
        [NonSerialized]
        private bool isGenericIList = false;
        [NonSerialized]
        private IFactory listFactory = null;
        [NonSerialized]
        private static readonly IFactory arrayListFactory = new ArrayListFactory();

        #endregion

        #region Properties

        /// <summary>
        /// Tell us if the member type implement generic Ilist interface.
        /// </summary>
        [XmlIgnore]
        public bool IsGenericIList
        {
            get { return isGenericIList; }
        }

        /// <summary>
        /// Tell us if the member type implement Ilist interface.
        /// </summary>
        [XmlIgnore]
        public bool IsIList
        {
            get { return isIList; }
        }

        /// <summary>
        /// List factory for <see cref="IList"/> property
        /// </summary>
        /// <remarks>Used by N+1 Select solution</remarks>
        [XmlIgnore]
        public IFactory ListFactory
        {
            get { return listFactory; }
        }

        /// <summary>
        /// The lazy loader factory
        /// </summary>
        [XmlIgnore]
        public ILazyFactory LazyFactory
        {
            get { return lazyFactory; }
        }

        /// <summary>
        /// Sets or gets the <see cref="IArgumentStrategy"/> used to fill the object property.
        /// </summary>
        [XmlIgnore]
        public virtual IArgumentStrategy ArgumentStrategy
        {
            set { throw new NotImplementedException("Valid on ArgumentProperty"); }
            get { throw new NotImplementedException("Valid on ArgumentProperty"); }
        }

        /// <summary>
        /// Sets or gets the <see cref="IPropertyStrategy"/> used to fill the object property.
        /// </summary>
        [XmlIgnore]
        public IPropertyStrategy PropertyStrategy
        {
            set { propertyStrategy = value; }
            get { return propertyStrategy; }
        }

        /// <summary>
        /// Specify the custom type handlers to used.
        /// </summary>
        /// <remarks>Will be an alias to a class wchic implement ITypeHandlerCallback</remarks>
        [XmlAttribute("typeHandler")]
        public string CallBackName
        {
            get { return callBackName; }
            set { callBackName = value; }
        }

        /// <summary>
        /// Tell us if we must lazy load this property..
        /// </summary>
        [XmlAttribute("lazyLoad")]
        public virtual bool IsLazyLoad
        {
            get { return isLazyLoad; }
            set { isLazyLoad = value; }
        }

        /// <summary>
        /// The typeHandler used to work with the result property.
        /// </summary>
        [XmlIgnore]
        public ITypeHandler TypeHandler
        {
            get
            {
                if (this.typeHandler == null)
                {
                    throw new DataMapperException(
                        String.Format("Error on Result property {0}, type handler for {1} is not registered.", PropertyName, MemberType.Name));
                }
                return this.typeHandler;
            }
            set { this.typeHandler = value; }
        }

        /// <summary>
        /// Give an entry in the 'DbType' enumeration
        /// </summary>
        /// <example >
        /// For Sql Server, give an entry of SqlDbType : Bit, Decimal, Money...
        /// <br/>
        /// For Oracle, give an OracleType Enumeration : Byte, Int16, Number...
        /// </example>
        [XmlAttribute("dbType")]
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }


        /// <summary>
        /// Specify the CLR type of the result.
        /// </summary>
        /// <remarks>
        /// The type attribute is used to explicitly specify the property type of the property to be set.
        /// Normally this can be derived from a property through reflection, but certain mappings such as
        /// HashTable cannot provide the type to the framework.
        /// </remarks>
        [XmlAttribute("type")]
        public string CLRType
        {
            get { return clrType; }
            set { clrType = value; }
        }


        /// <summary>
        /// select 属性的作用是指定一个嵌套查询(一个 SQL 映射语句的 ID)，用于获取关联对象。，这个查询会在需要填充当前属性时被执行。
        /// 它常用于实现 关联对象的延迟加载 或者 复杂对象图的构建
        /// </summary>
        [XmlAttribute("select")]
        public string Select
        {
            get { return select; }
            set { select = value; }
        }

        /// <summary>
        /// The name of a nested ResultMap to set the property
        /// </summary>
        [XmlAttribute("resultMapping")]
        public string NestedResultMapName
        {
            get { return nestedResultMapName; }
            set { nestedResultMapName = value; }
        }

        /// <summary>
        /// The property name used to identify the property amongst the others.
        /// </summary>
        [XmlAttribute("property")]
        public string PropertyName
        {
            get { return propertyName; }
            set
            {
                propertyName = value;
                if (propertyName.IndexOf('.') < 0)
                {
                    isComplexMemberName = false;
                }
                else // complex member name FavouriteLineItem.Id
                {
                    isComplexMemberName = true;
                }
            }
        }

        /// <summary>
        /// Defines a field/property <see cref="ISetAccessor"/>
        /// </summary>
        [XmlIgnore]
        public ISetAccessor SetAccessor
        {
            get { return setAccessor; }
        }

        /// <summary>
        /// Get the field/property type
        /// </summary>
        [XmlIgnore]
        public virtual Type MemberType
        {
            get
            {
                if (setAccessor != null)
                {
                    return setAccessor.MemberType;
                }
                else if (nestedResultMap != null)
                {
                    return nestedResultMap.Class;
                }
                else
                {
                    throw new IBatisNetSelfException(
                        String.Format(CultureInfo.InvariantCulture,
                                      "Could not resolve member type for result property '{0}'. Neither nested result map nor typed setter was provided.",
                                      propertyName));
                }
            }
        }

        /// <summary>
        /// Tell if a nullValue is defined.
        /// </summary>
        [XmlIgnore]
        public bool HasNullValue
        {
            get { return (nullValue != null); }
        }

        /// <summary>
        /// Null value replacement.
        /// </summary>
        /// <example>"no_email@provided.com"</example>
        [XmlAttribute("nullValue")]
        public string NullValue
        {
            get { return nullValue; }
            set { nullValue = value; }
        }

        /// <summary>
        /// A nested ResultMap use to set a property
        /// </summary>
        [XmlIgnore]
        public IResultMap NestedResultMap
        {
            get { return nestedResultMap; }
            set { nestedResultMap = value; }
        }

        /// <summary>
        /// Indicate if we have a complex member name as [FavouriteLineItem.Id]
        /// </summary>
        public bool IsComplexMemberName
        {
            get { return isComplexMemberName; }
        }

        /// <summary>
        /// Column Index
        /// </summary>
        [XmlAttribute("columnIndex")]
        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        /// <summary>
        /// Column Name
        /// </summary>
        [XmlAttribute("column")]
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化结果属性的 PropertyInfo。
        /// </summary>
        /// <param name="aResultClass">结果对象的类型</param>
        /// <param name="configScope">当前配置作用域</param>
        public void Initialize(ConfigurationScope configScope, Type aResultClass)
        {
            // 如果属性名非空，且不等于"value"，且目标类型不是 IDictionary
            if (propertyName.Length > 0 && propertyName != "value" &&
                !typeof(IDictionary).IsAssignableFrom(aResultClass))
            {
                // 如果是复合成员名（如 "FavouriteLineItem.Id"）
                if (isComplexMemberName)
                {
                    // 使用 ObjectProbe 获取属性名对应的成员信息（用于 setter）
                    MemberInfo propertyInfo = ObjectProbe.GetMemberInfoForSetter(aResultClass, propertyName);
                    // 提取最后一个“.”后的属性名（如 "Id"）
                    string memberName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
                    // 创建 setter 访问器，用于后续给属性赋值
                    this.setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(propertyInfo.ReflectedType, memberName);                    
                }
                else 
                {
                    // 普通属性名，直接用类型和属性名创建 setter 访问器
                    this.setAccessor = configScope.DataExchangeFactory.AccessorFactory.SetAccessorFactory.CreateSetAccessor(aResultClass, propertyName);
                }

                isIList = typeof(IList).IsAssignableFrom(MemberType);

                // set the list factory

                if (isIList)
                {
                    if (MemberType.IsArray)
                    {
                        this.listFactory = arrayListFactory;
                    }
                    else
                    {
                        if (MemberType == typeof(IList))
                        {
                            this.listFactory = arrayListFactory;
                        }
                        else // custom collection
                        {
                            this.listFactory = configScope.DataExchangeFactory.ObjectFactory.CreateFactory(MemberType, Type.EmptyTypes);
                        }
                    }
                }
            }

            if (CallBackName != null && CallBackName.Length > 0)
            {
                configScope.ErrorContext.MoreInfo = "Result property '" + propertyName + "' check the typeHandler attribute '" + CallBackName + "' (must be a ITypeHandlerCallback implementation).";
                try
                {
                    Type type = configScope.SqlMapper.TypeHandlerFactory.GetType(CallBackName);
                    ITypeHandlerCallback typeHandlerCallback = (ITypeHandlerCallback)Activator.CreateInstance(type);
                    this.typeHandler = new CustomTypeHandler(typeHandlerCallback);
                }
                catch (Exception e)
                {
                    throw new IBatisConfigException("Error occurred during custom type handler configuration.  Cause: " + e.Message, e);
                }
            }
            else
            {
                configScope.ErrorContext.MoreInfo = "Result property '" + propertyName + "' set the typeHandler attribute.";
                this.typeHandler = configScope.ResolveTypeHandler(aResultClass, propertyName, clrType, dbType, true);
            }

            if (IsLazyLoad)
            {
                lazyFactory = new LazyFactoryBuilder().GetLazyFactory(setAccessor.MemberType);
            }
        }

        /// <summary>
        /// Initialize a the result property
        /// for AutoMapper
        /// </summary>
        /// <param name="setAccessor">An <see cref="ISetAccessor"/>.</param>
        /// <param name="typeHandlerFactory"></param>
        internal void Initialize(TypeHandlerFactory typeHandlerFactory, ISetAccessor setAccessor)
        {
            this.setAccessor = setAccessor;
            this.typeHandler = typeHandlerFactory.GetTypeHandler(setAccessor.MemberType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public object GetDataBaseValue(IDataReader dataReader)
        {
            object value = null;

            if (columnIndex == UNKNOWN_COLUMN_INDEX)
            {
                value = this.typeHandler.GetValueByName(this, dataReader);
            }
            else
            {
                value = this.typeHandler.GetValueByIndex(this, dataReader);
            }

            bool wasNull = (value == DBNull.Value);
            if (wasNull)
            {
                if (HasNullValue)
                {
                    if (setAccessor != null)
                    {
                        value = this.typeHandler.ValueOf(setAccessor.MemberType, nullValue);
                    }
                    else
                    {
                        value = this.typeHandler.ValueOf(null, nullValue);
                    }
                }
                else
                {
                    value = this.typeHandler.NullValue;
                }
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object TranslateValue(object value)
        {
            if (value == null)
            {
                return TypeHandler.NullValue;
            }
            else
            {
                return value;
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>An <see cref="ResultProperty"/></returns>
        public ResultProperty Clone()
        {
            ResultProperty resultProperty = new ResultProperty();

            resultProperty.CLRType = CLRType;
            resultProperty.CallBackName = CallBackName;
            resultProperty.ColumnIndex = ColumnIndex;
            resultProperty.ColumnName = ColumnName;
            resultProperty.DbType = DbType;
            resultProperty.IsLazyLoad = IsLazyLoad;
            resultProperty.NestedResultMapName = NestedResultMapName;
            resultProperty.NullValue = NullValue;
            resultProperty.PropertyName = PropertyName;
            resultProperty.Select = Select;

            return resultProperty;
        }

        #endregion

        /// <summary>
        /// <see cref="IFactory"/> that constructs <see cref="ArrayList"/> instance
        /// </summary>
        private class ArrayListFactory : IFactory
        {
            #region IFactory Members

            /// <summary>
            /// Create a new instance with the specified parameters
            /// </summary>
            /// <param name="parameters">An array of values that matches the number, order and type
            /// of the parameters for this constructor.</param>
            /// <returns>A new instance</returns>
            /// <remarks>
            /// If you call a constructor with no parameters, pass null.
            /// Anyway, what you pass will be ignore.
            /// </remarks>
            public object CreateInstance(object[] parameters)
            {
                return new ArrayList();
            }

            #endregion
        }
    }
}
