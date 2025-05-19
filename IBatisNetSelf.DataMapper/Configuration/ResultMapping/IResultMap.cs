using IBatisNetSelf.DataMapper.DataExchange;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// This is a grouping of ResultMapping objects used to map results back to objects
    /// </summary>
    public interface IResultMap
    {
        /// <summary>
        /// The collection of constructor parameters.
        /// </summary>
        [XmlIgnore]
        ResultPropertyCollection ConstructorParams { get; }

        /// <summary>
        /// The collection of ResultProperty.
        /// </summary>
        [XmlIgnore]
        ResultPropertyCollection Properties { get; }

        /// <summary>
        /// The GroupBy Properties.
        /// </summary>
        [XmlIgnore]
        ResultPropertyCollection GroupByProperties { get; }

        /// <summary>
        /// Identifier used to identify the resultMap amongst the others.
        /// </summary>
        /// <example>GetProduct</example>
        [XmlAttribute("id")]
        string Id { get; }

        /// <summary>
        /// The GroupBy Properties name.
        /// </summary>
        [XmlIgnore]
        StringCollection GroupByPropertyNames { get; }

        /// <summary>
        /// The output type class of the resultMap.
        /// </summary>
        [XmlIgnore]
        Type Class { get; }

        /// <summary>
        /// Sets the IDataExchange
        /// </summary>
        [XmlIgnore]
        IDataExchange DataExchange { set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initalized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initalized; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        bool IsInitalized { get; set; }


        /// <summary>
        /// Create an instance Of result.
        /// </summary>
        /// <param name="parameters">
        /// An array of values that matches the number, order and type 
        /// of the parameters for this constructor. 
        /// </param>
        /// <returns>An object.</returns>
        object CreateInstanceOfResult(object[] parameters);

        /// <summary>
        /// Set the value of an object property.
        /// </summary>
        /// <param name="target">The object to set the property.</param>
        /// <param name="property">The result property to use.</param>
        /// <param name="dataBaseValue">The database value to set.</param>
        void SetValueOfProperty(ref object target, ResultProperty property, object dataBaseValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        IResultMap ResolveSubMap(IDataReader dataReader);
    }
}
