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
    /// 处理未知类型的TypeHandler实现
    /// </summary>
    /// <remarks>
    /// 工厂委托机制：这个类借助工厂来获取实际类型的处理器，以此处理具体的转换工作。
    /// </remarks>
    public sealed class UnknownTypeHandler : BaseTypeHandler
    {
        // 类型处理器工厂，用于获取特定类型的处理器
        private TypeHandlerFactory factory = null;

        /// <summary>
        /// 获取一个值，该值指示此实例是否为简单类型。
        /// </summary>
        /// <value>
        /// 	<c>true</c> 表示此实例是简单类型；否则为 <c>false</c>。
        /// </value>
        public override bool IsSimpleType=>true;


        /// <summary>
        /// 通过工厂创建的构造函数
        /// </summary>
        /// <param name="factory">与之关联的工厂</param>
        public UnknownTypeHandler(TypeHandlerFactory factory)
        {
            this.factory = factory;
        }

 
        /// <summary>
        /// 在将值用于设置IDbCommand的参数之前对其进行处理。
        /// </summary>
        /// <param name="aDataParameter">数据参数</param>
        /// <param name="aParameterValue">要设置的值</param>
        /// <param name="aDbType">数据库类型</param>
        public override void SetParameter(IDataParameter aDataParameter, object aParameterValue, string aDbType)
        {
            if (aParameterValue != null)
            {
                // 获取参数值实际类型的处理器并委托处理
                ITypeHandler _handler = this.factory.GetTypeHandler(aParameterValue.GetType(), aDbType);
                _handler.SetParameter(aDataParameter, aParameterValue, aDbType);
            }
            else
            {
                // 当向服务器发送空参数值时，用户必须指定DBNull，而不是null。
                aDataParameter.Value = System.DBNull.Value;
            }
        }

        /// <summary>
        /// 通过名称获取列值
        /// </summary>
        /// <param name="mapping">结果属性映射</param>
        /// <param name="dataReader">数据读取器</param>
        /// <returns>列值</returns>
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
        /// 通过索引获取列值
        /// </summary>
        /// <param name="mapping">结果属性映射</param>
        /// <param name="dataReader">数据读取器</param>
        /// <returns>列值</returns>
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
        /// 将字符串转换为此处理器处理的类型
        /// </summary>
        /// <param name="type">属性的类型（仅用于枚举转换）</param>
        /// <param name="s">字符串值</param>
        /// <returns>转换后的值</returns>
        public override object ValueOf(Type type, string s)
        {
            // 默认直接返回字符串，不进行转换
            return s;
        }

        /// <summary>
        /// 检索输出参数的数据库输出值
        /// </summary>
        /// <param name="outputValue">数据库输出值</param>
        /// <param name="parameterType">枚举类型处理器中使用的类型</param>
        /// <returns>处理后的输出值</returns>
        public override object GetDataBaseValue(object outputValue, Type parameterType)
        {
            // 默认直接返回输出值，不进行转换
            return outputValue;
        }



        /// <summary>
        /// 比较两个值（此处理器处理的值）是否相等
        /// </summary>
        /// <param name="obj">其中一个对象</param>
        /// <param name="str">另一个对象的字符串表示</param>
        /// <returns>如果相等则为true</returns>
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
