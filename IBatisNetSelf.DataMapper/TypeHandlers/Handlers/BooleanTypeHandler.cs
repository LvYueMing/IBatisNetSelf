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
    /// Boolean TypeHandler.
    /// </summary>
    public sealed class BooleanTypeHandler : BaseTypeHandler
    {
        /// <summary>
        /// Gets a value indicating whether this instance is simple type.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is simple type; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSimpleType => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aMapping"></param>
        /// <param name="aDataReader"></param>
        /// <returns></returns>
        public override object GetValueByName(ResultProperty aMapping, IDataReader aDataReader)
        {
            int _index = aDataReader.GetOrdinal(aMapping.ColumnName);

            if (aDataReader.IsDBNull(_index) == true)
            {
                return System.DBNull.Value;
            }
            else
            {
                return Convert.ToBoolean(aDataReader.GetValue(_index));
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
                return Convert.ToBoolean(dataReader.GetValue(mapping.ColumnIndex));
            }
        }


        /// <summary>
        /// Retrieve ouput database value of an output parameter
        /// </summary>
        /// <param name="outputValue">ouput database value</param>
        /// <param name="parameterType">type used in EnumTypeHandler</param>
        /// <returns></returns>
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Convert.ToBoolean(outputValue);
        }


        /// <summary>
        /// Converts the String to the type that this handler deals with
        /// </summary>
        /// <param name="type">the tyepe of the property (used only for enum conversion)</param>
        /// <param name="s">the String value</param>
        /// <returns>the converted value</returns>
        public override object ValueOf(Type type, string s)
        {
            return Convert.ToBoolean(s);
        }

        //public override object NullValue
        //{
        //    get { throw new InvalidCastException("BooleanTypeHandler could not cast a null value in bool field."); }
        //}
    }
}
