using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.ParameterMapping
{
    /// <summary>
    /// Summary description for ParameterProperty.
    /// </summary>
    [Serializable]
    [XmlRoot("parameter", Namespace = "http://ibatis.apache.org/mapping")]
    public class ParameterProperty
    {

        #region Fields
        [NonSerialized]
        private string nullValue = null;//string.Empty;//null;
        [NonSerialized]
        private string propertyName = string.Empty;
        [NonSerialized]
        private ParameterDirection direction = ParameterDirection.Input;
        [NonSerialized]
        private string directionAttribute = string.Empty;
        [NonSerialized]
        private string dbType = null;
        [NonSerialized]
        private int size = -1;
        [NonSerialized]
        private byte scale = 0;
        [NonSerialized]
        private byte precision = 0;
        [NonSerialized]
        private string columnName = string.Empty; // used only for store procedure
        [NonSerialized]
        private ITypeHandler typeHandler = null;
        [NonSerialized]
        private string clrType = string.Empty;
        [NonSerialized]
        private string callBackName = string.Empty;
        [NonSerialized]
        private IGetAccessor getAccessor = null;
        [NonSerialized]
        private bool isComplexMemberName = false;

        #endregion

        #region Properties

        /// <summary>
        /// Indicate if we have a complex member name as [avouriteLineItem.Id]
        /// </summary>
        public bool IsComplexMemberName
        {
            get { return isComplexMemberName; }
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
        /// Specify the CLR type of the parameter.
        /// </summary>
        /// <remarks>
        /// The type attribute is used to explicitly specify the property type to be read.
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
        /// The typeHandler used to work with the parameter.
        /// </summary>
        [XmlIgnore]
        public ITypeHandler TypeHandler
        {
            get { return typeHandler; }
            set { typeHandler = value; }
        }

        /// <summary>
        /// Column Name for output parameter 
        /// in store proccedure.
        /// </summary>
        [XmlAttribute("column")]
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        /// <summary>
        /// Column size.
        /// </summary>
        [XmlAttribute("size")]
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Column Scale.
        /// </summary>
        [XmlAttribute("scale")]
        public byte Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Column Precision.
        /// </summary>
        [XmlAttribute("precision")]
        public byte Precision
        {
            get { return precision; }
            set { precision = value; }
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
        /// The direction attribute of the XML parameter.
        /// </summary>
        /// <example> Input, Output, InputOutput</example>
        [XmlAttribute("direction")]
        public string DirectionAttribute
        {
            get { return directionAttribute; }
            set { directionAttribute = value; }
        }

        /// <summary>
        /// Indicate the direction of the parameter.
        /// </summary>
        /// <example> Input, Output, InputOutput</example>
        [XmlIgnore]
        public ParameterDirection Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                directionAttribute = direction.ToString();
            }
        }

        /// <summary>
        /// Property name used to identify the property amongst the others.
        /// </summary>
        /// <example>EmailAddress</example>
        [XmlAttribute("property")]
        public string PropertyName
        {
            get { return propertyName; }
            set
            {
                if ((value == null) || (value.Length < 1))
                    throw new ArgumentNullException("The property attribute is mandatory in a paremeter property.");

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
        /// Tell if a nullValue is defined._nullValue!=null
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
        /// Defines a field/property get accessor
        /// </summary>
        [XmlIgnore]
        public IGetAccessor GetAccessor
        {
            get { return getAccessor; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the parameter property
        /// </summary>
        /// <param name="aConfigScope">The scope.</param>
        /// <param name="aParameterClass">The parameter class.</param>
        public void Initialize(IScope aConfigScope, Type aParameterClass)
        {

            if (this.directionAttribute.Length > 0)
            {
                this.direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), directionAttribute, true);
            }

            if (!typeof(IDictionary).IsAssignableFrom(aParameterClass) // Hashtable parameter map
                && aParameterClass != null // value property
                && !aConfigScope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(aParameterClass)) // value property
            {
                if (!isComplexMemberName)
                {
                    IGetAccessorFactory _getAccessorFactory = aConfigScope.DataExchangeFactory.AccessorFactory.GetAccessorFactory;
                    this.getAccessor = _getAccessorFactory.CreateGetAccessor(aParameterClass, this.propertyName);
                }
                else // complex member name FavouriteLineItem.Id
                {
                    string _memberName = this.propertyName.Substring(this.propertyName.LastIndexOf('.') + 1);
                    string _parentName = this.propertyName.Substring(0, this.propertyName.LastIndexOf('.'));
                    Type _parentType = ObjectProbe.GetMemberTypeForGetter(aParameterClass, _parentName);

                    IGetAccessorFactory _getAccessorFactory = aConfigScope.DataExchangeFactory.AccessorFactory.GetAccessorFactory;
                    this.getAccessor = _getAccessorFactory.CreateGetAccessor(_parentType, _memberName);
                }
            }

            aConfigScope.ErrorContext.MoreInfo = $"Check the parameter mapping typeHandler attribute {this.CallBackName}(must be a ITypeHandlerCallback implementation).";
            if (this.CallBackName.Length > 0)
            {
                try
                {
                    Type _type = aConfigScope.DataExchangeFactory.TypeHandlerFactory.GetType(this.CallBackName);
                    ITypeHandlerCallback _typeHandlerCallback = (ITypeHandlerCallback)Activator.CreateInstance(_type);
                    this.typeHandler = new CustomTypeHandler(_typeHandlerCallback);
                }
                catch (Exception e)
                {
                    throw new ConfigurationException("Error occurred during custom type handler configuration.  Cause: " + e.Message, e);
                }
            }
            else
            {
                if (this.CLRType.Length == 0)  // Unknown
                {
                    if (this.getAccessor != null &&
                        aConfigScope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(this.getAccessor.MemberType))
                    {
                        // Primitive
                        this.typeHandler = aConfigScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(this.getAccessor.MemberType, this.dbType);
                    }
                    else
                    {
                        this.typeHandler = aConfigScope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                    }
                }
                else // If we specify a CLR type, use it
                {
                    Type _type = TypeUtils.ResolveType(this.CLRType);

                    if (aConfigScope.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(_type))
                    {
                        // Primitive
                        this.typeHandler = aConfigScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(_type, dbType);
                    }
                    else
                    {
                        // .NET object
                        _type = ObjectProbe.GetMemberTypeForGetter(_type, this.PropertyName);
                        this.typeHandler = aConfigScope.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(_type, this.dbType);
                    }
                }
            }
        }




        /// <summary>
        /// Determines whether the specified <see cref="System.Object"></see> is equal to the current <see cref="System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"></see> to compare with the current <see cref="System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="System.Object"></see> is equal to the current <see cref="System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;
            ParameterProperty p = (ParameterProperty)obj;
            return (this.PropertyName == p.PropertyName);
        }


        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return propertyName.GetHashCode();
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>An <see cref="ParameterProperty"/></returns>
        public ParameterProperty Clone()
        {
            ParameterProperty property = new ParameterProperty();

            property.CallBackName = this.CallBackName;
            property.CLRType = this.CLRType;
            property.ColumnName = this.ColumnName;
            property.DbType = this.DbType;
            property.DirectionAttribute = this.DirectionAttribute;
            property.NullValue = this.NullValue;
            property.PropertyName = this.PropertyName;
            property.Precision = this.Precision;
            property.Scale = this.Scale;
            property.Size = this.Size;

            return property;
        }
        #endregion

    }
}
