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
    /// 【字节类型处理器】- 用于处理byte类型与数据库字段的相互转换
    /// </summary>
    /// <remarks>
    /// 密封类设计，确保字节转换逻辑的稳定性
    /// </remarks>
    public sealed class ByteTypeHandler : BaseTypeHandler
    {
        /// <summary>
        /// 标识是否为简单类型（基本数据类型）
        /// </summary>
        /// <value>
        /// 始终返回<c>true</c>，因为byte属于基础值类型
        /// </value>
        public override bool IsSimpleType => true;


        /// <summary>
        /// 通过列名获取字节类型数据
        /// </summary>
        /// <param name="mapping">结果映射属性（包含列元数据）</param>
        /// <param name="dataReader">数据读取器</param>
        /// <returns>
        /// <para>DBNull.Value - 当数据库字段为NULL时</para>
        /// <para>byte - 转换成功的字节值</para>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">当列名不存在时</exception>
        public override object GetValueByName(ResultProperty mapping, IDataReader dataReader)
        {
            // 根据列名获取数据读取器的索引位置
            int index = dataReader.GetOrdinal(mapping.ColumnName);

            // 处理数据库NULL值情况
            if (dataReader.IsDBNull(index) == true)
            {
                return System.DBNull.Value;
            }
            else
            {
                // 安全转换数值类型（支持多种数值类型到byte的转换）
                return Convert.ToByte(dataReader.GetValue(index));
            }
        }

        /// <summary>
        /// 通过列索引获取字节类型数据
        /// </summary>
        /// <param name="mapping">结果映射属性（包含列索引）</param>
        /// <param name="dataReader">数据读取器</param>
        /// <returns>
        /// <para>DBNull.Value - 当数据库字段为NULL时</para>
        /// <para>byte - 转换成功的字节值</para>
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">当索引超出范围时</exception>
        public override object GetValueByIndex(ResultProperty mapping, IDataReader dataReader)
        {
            if (dataReader.IsDBNull(mapping.ColumnIndex) == true)
            {
                return System.DBNull.Value;
            }
            else
            {
                return Convert.ToByte(dataReader.GetValue(mapping.ColumnIndex));
            }
        }

        /// <summary>
        /// 将字符串转换为byte类型
        /// </summary>
        /// <param name="type">目标类型（用于枚举处理的扩展点）</param>
        /// <param name="s">待转换的字符串</param>
        /// <returns>转换后的byte值</returns>
        /// <exception cref="FormatException">当字符串格式无效时</exception>
        /// <exception cref="OverflowException">当值超出byte范围时</exception>
        public override object ValueOf(Type type, string s)
        {
            return Convert.ToByte(s);
        }

        /// <summary>
        /// 处理输出参数的数据库值转换
        /// </summary>
        /// <param name="outputValue">数据库输出参数值</param>
        /// <param name="parameterType">参数类型（兼容枚举处理）</param>
        /// <returns>byte类型的输出值</returns>
        /// <remarks>
        /// 主要用于存储过程输出参数的后期处理
        /// </remarks>
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            return Convert.ToByte(outputValue);
        }

    }
}
