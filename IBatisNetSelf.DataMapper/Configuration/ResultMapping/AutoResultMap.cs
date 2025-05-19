using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.DataExchange;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// Implementation of <see cref="IResultMap"/> interface for auto mapping
    /// </summary>
    public class AutoResultMap : IResultMap
    {
        [NonSerialized]
        private bool isInitalized = false;
        [NonSerialized]
        private Type resultClass = null;
        [NonSerialized]
        private IFactory resultClassFactory = null;
        [NonSerialized]
        private ResultPropertyCollection properties = new ResultPropertyCollection();

        [NonSerialized]
        private IDataExchange dataExchange = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoResultMap"/> class.
        /// </summary>
        /// <param name="aResultClass">The result class.</param>
        /// <param name="aResultClassFactory">The result class factory.</param>
        /// <param name="aDataExchange">The data exchange.</param>
        public AutoResultMap(Type aResultClass, IFactory aResultClassFactory, IDataExchange aDataExchange)
        {
            this.resultClass = aResultClass;
            this.resultClassFactory = aResultClassFactory;
            this.dataExchange = aDataExchange;
        }

        #region IResultMap Members

        /// <summary>
        /// The GroupBy Properties.
        /// </summary>
        [XmlIgnore]
        public StringCollection GroupByPropertyNames
        {
            get { throw new NotImplementedException("The property 'GroupByPropertyNames' is not implemented."); }
        }

        /// <summary>
        /// The collection of ResultProperty.
        /// </summary>
        [XmlIgnore]
        public ResultPropertyCollection Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// The GroupBy Properties.
        /// </summary>
        /// <value></value>
        public ResultPropertyCollection GroupByProperties
        {
            get { throw new NotImplementedException("The property 'GroupByProperties' is not implemented."); }
        }

        /// <summary>
        /// The collection of constructor parameters.
        /// </summary>
        [XmlIgnore]
        public ResultPropertyCollection ConstructorParams
        {
            get { throw new NotImplementedException("The property 'Parameters' is not implemented."); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initalized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initalized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitalized
        {
            get { return isInitalized; }
            set { isInitalized = value; }
        }

        /// <summary>
        /// Identifier used to identify the resultMap amongst the others.
        /// </summary>
        /// <value></value>
        /// <example>GetProduct</example>
        public string Id
        {
            get { return resultClass.Name; }
        }


        /// <summary>
        /// The output type class of the resultMap.
        /// </summary>
        /// <value></value>
        public Type Class
        {
            get { return resultClass; }
        }


        /// <summary>
        /// Sets the IDataExchange
        /// </summary>
        /// <value></value>
        public IDataExchange DataExchange
        {
            set { dataExchange = value; }
        }


        /// <summary>
        /// Create an instance Of result.
        /// </summary>
        /// <param name="parameters">An array of values that matches the number, order and type
        /// of the parameters for this constructor.</param>
        /// <returns>An object.</returns>
        public object CreateInstanceOfResult(object[] parameters)
        {
            return CreateInstanceOfResultClass();
        }

        /// <summary>
        /// Set the value of an object property.
        /// </summary>
        /// <param name="target">The object to set the property.</param>
        /// <param name="property">The result property to use.</param>
        /// <param name="dataBaseValue">The database value to set.</param>
        public void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue)
        {
            this.dataExchange.SetData(ref target, property, dataBaseValue);
        }

        /// <summary>
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public IResultMap ResolveSubMap(IDataReader dataReader)
        {
            return this;
        }

        #endregion

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public AutoResultMap Clone()
        {
            return new AutoResultMap(resultClass, resultClassFactory, dataExchange);
        }

        /// <summary>
        /// Create an instance of result class.
        /// </summary>
        /// <returns>An object.</returns>
        public object CreateInstanceOfResultClass()
        {
            if (this.resultClass.IsPrimitive || this.resultClass == typeof(string))
            {
                TypeCode _typeCode = Type.GetTypeCode(this.resultClass);
                return TypeUtils.InstantiatePrimitiveType(_typeCode);
            }
            else
            {
                if (this.resultClass.IsValueType)
                {
                    if (this.resultClass == typeof(DateTime))
                    {
                        return new DateTime();
                    }
                    else if (this.resultClass == typeof(Decimal))
                    {
                        return new Decimal();
                    }
                    else if (this.resultClass == typeof(Guid))
                    {
                        return Guid.Empty;
                    }
                    else if (this.resultClass == typeof(TimeSpan))
                    {
                        return new TimeSpan(0);
                    }
                    else
                    {
                        throw new NotImplementedException("Unable to instanciate value type");
                    }
                }
                else
                {
                    return this.resultClassFactory.CreateInstance(null);
                }
            }
        }

    }
}
