using IBatisNetSelf.Common.Exceptions;
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
    /// Description  ByteArrayTypeHandler.
    /// </summary>
    public sealed class ByteArrayTypeHandler : BaseTypeHandler
    {
        /// <summary>
        /// Tell us if ot is a 'primitive' type
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public override bool IsSimpleType => true;

        /// <summary>
        /// Gets a column value by the name
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            int index = dataReader.GetOrdinal(mapping.ColumnName);

            if (dataReader.IsDBNull(index) || dataReader.GetBytes(index, 0, null, 0, 0) == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return GetValueByIndex(index, dataReader);
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
            if (dataReader.IsDBNull(mapping.ColumnIndex) || dataReader.GetBytes(mapping.ColumnIndex, 0, null, 0, 0) == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return GetValueByIndex(mapping.ColumnIndex, dataReader);
            }
        }


        /// <summary>
        /// Gets the index of the value by.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        private byte[] GetValueByIndex(int columnIndex, IDataReader dataReader)
        {
            // determine the buffer size
            int bufferLength = (int)dataReader.GetBytes(columnIndex, 0, null, 0, 0);

            // initialize it
            byte[] byteArray = new byte[bufferLength];

            // fill it
            dataReader.GetBytes(columnIndex, 0, byteArray, 0, bufferLength);

            return byteArray;
        }


        /// <summary>
        /// Converts the String to the type that this handler deals with
        /// </summary>
        /// <param name="type">the tyepe of the property (used only for enum conversion)</param>
        /// <param name="s">the String value</param>
        /// <returns>the converted value</returns>
        public override object ValueOf(Type type, string s)
        {
            return System.Text.Encoding.Default.GetBytes(s);
        }

        /// <summary>
        /// Retrieve ouput database value of an output parameter
        /// </summary>
        /// <param name="outputValue">ouput database value</param>
        /// <param name="parameterType">type used in EnumTypeHandler</param>
        /// <returns></returns>
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            throw new DataMapperException("NotSupportedException");
        }



        //public override object NullValue
        //{
        //    get { return null; }
        //}
    }
}
