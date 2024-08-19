using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers.Handlers
{
    /// <summary>
    ///  Implementation of TypeHandler for dealing with unknown types
    /// </summary>
    public sealed class UnknownTypeHandler : BaseTypeHandler
    {

        private TypeHandlerFactory factory = null;

        /// <summary>
        /// Gets a value indicating whether this instance is simple type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is simple type; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSimpleType=>true;


        /// <summary>
        /// Constructor to create via a factory
        /// </summary>
        /// <param name="factory">the factory to associate this with</param>
        public UnknownTypeHandler(TypeHandlerFactory factory)
        {
            this.factory = factory;
        }
        /// <summary>
        /// Performs processing on a value before it is used to set
        /// the parameter of a IDbCommand.
        /// </summary>
        /// <param name="aDataParameter"></param>
        /// <param name="aParameterValue">The value to be set</param>
        /// <param name="aDbType">Data base type</param>
        public override void SetParameter(IDataParameter aDataParameter, object aParameterValue, string aDbType)
        {
            if (aParameterValue != null)
            {
                ITypeHandler _handler = this.factory.GetTypeHandler(aParameterValue.GetType(), aDbType);
                _handler.SetParameter(aDataParameter, aParameterValue, aDbType);
            }
            else
            {
                // When sending a null parameter value to the server,
                // the user must specify DBNull, not null. 
                aDataParameter.Value = System.DBNull.Value;
            }
        }

        /// <summary>
        /// Gets a column value by the name
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int _index = dataReader.GetOrdinal(mapping.ColumnName);

            if (dataReader.IsDBNull(_index) == true)
            {
                return System.DBNull.Value;
            }
            else
            {
                return dataReader.GetValue(_index);
            }
        }

        /// <summary>
        /// Gets a column value by the index
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex) == true)
            {
                return System.DBNull.Value;
            }
            else
            {
                return dataReader.GetValue(mapping.ColumnIndex);
            }
        }

        /// <summary>
        /// Converts the String to the type that this handler deals with
        /// </summary>
        /// <param name="type">the tyepe of the property (used only for enum conversion)</param>
        /// <param name="s">the String value</param>
        /// <returns>the converted value</returns>
        public override object ValueOf(Type type, string s)
        {
            return s;
        }

        /// <summary>
        /// Retrieve ouput database value of an output parameter
        /// </summary>
        /// <param name="outputValue">ouput database value</param>
        /// <param name="parameterType">type used in EnumTypeHandler</param>
        /// <returns></returns>
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return outputValue;
        }



        //public override object NullValue
        //{
        //    get { throw new InvalidCastException("UnknownTypeHandler could not cast a null value in unknown type field."); }
        //}

        /// <summary>
        ///  Compares two values (that this handler deals with) for equality
        /// </summary>
        /// <param name="obj">one of the objects</param>
        /// <param name="str">the other object as a String</param>
        /// <returns>true if they are equal</returns>
        public override bool Equals(object obj, string str)
        {
            if (obj == null || str == null)
            {
                return (string)obj == str;
            }
            else
            {
                ITypeHandler handler = this.factory.GetTypeHandler(obj.GetType());
                object castedObject = handler.ValueOf(obj.GetType(), str);
                return obj.Equals(castedObject);
            }
        }
    }
}
