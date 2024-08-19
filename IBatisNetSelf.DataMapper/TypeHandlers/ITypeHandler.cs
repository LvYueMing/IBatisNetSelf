using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// Summary description for ITypeHandler.
    /// </summary>
    public interface ITypeHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsSimpleType { get; }

        /// <summary>
        /// The null value for this type
        /// </summary>
        object NullValue { get; }

        /// <summary>
        /// Gets a column value by the name
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        object GetValueByName(ResultProperty mapping, IDataReader dataReader);

        /// <summary>
        /// Gets a column value by the index
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        object GetValueByIndex(ResultProperty mapping, IDataReader dataReader);

        /// <summary>
        /// Retrieve ouput database value of an output parameter
        /// </summary>
        /// <param name="outputValue">ouput database value</param>
        /// <param name="parameterType">type used in EnumTypeHandler</param>
        /// <returns></returns>
        object GetDataBaseValue(object outputValue, Type parameterType);

        /// <summary>
        ///  Sets a parameter on a IDbCommand
        /// </summary>
        /// <param name="dataParameter">the parameter</param>
        /// <param name="parameterValue">the parameter value</param>
        /// <param name="dbType">the dbType of the parameter</param>
        void SetParameter(IDataParameter dataParameter, object parameterValue, string dbType);

        /// <summary>
        /// Converts the String to the type that this handler deals with
        /// </summary>
        /// <param name="type">the tyepe of the property (used only for enum conversion)</param>
        /// <param name="s">the String value</param>
        /// <returns>the converted value</returns>
        object ValueOf(Type type, string s);

        /// <summary>
        ///  Compares two values (that this handler deals with) for equality
        /// </summary>
        /// <param name="obj">one of the objects</param>
        /// <param name="str">the other object as a String</param>
        /// <returns>true if they are equal</returns>
        bool Equals(object obj, string str);


    }
}
