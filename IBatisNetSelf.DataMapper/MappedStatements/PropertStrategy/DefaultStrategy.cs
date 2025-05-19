using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers.Handlers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// 当 <see cref="ResultProperty"/> 上未配置 'select' 或 'resultMap' 属性时，
    /// 使用的默认 <see cref="IPropertyStrategy"/> 实现。
    /// </summary>
    public sealed class DefaultStrategy : IPropertyStrategy
    {
        #region IPropertyStrategy members

        /// <summary>
        /// 当未指定 select 或 resultMap 属性时，将数据库读取的值设置到目标对象的属性上。
        /// </summary>
        /// <param name="request">当前请求上下文。</param>
        /// <param name="resultMap">当前映射的 ResultMap。</param>
        /// <param name="mapping">映射的属性信息（ResultProperty）。</param>
        /// <param name="target">目标对象，值将被设置到此对象上。</param>
        /// <param name="reader">用于读取数据库数据的 IDataReader。</param>
        /// <param name="keys">主键（可以用于缓存或识别对象）。</param>
        public void Set(RequestScope request, IResultMap resultMap,ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            // 从 reader 中获取字段值
            object obj = Get(request, resultMap, mapping, ref target, reader);
            // 将字段值设置到目标对象的属性中
            resultMap.SetValueOfProperty(ref target, mapping, obj);
        }

        /// <summary>
        /// 从 IDataReader 中获取指定属性的值（字段值），并返回。
        /// </summary>
        /// <param name="request">请求上下文。</param>
        /// <param name="resultMap">当前的 ResultMap。</param>
        /// <param name="mapping">字段与属性的映射描述。</param>
        /// <param name="reader">数据库读取器。</param>
        /// <param name="target">当前的目标对象。</param>
        /// <returns>从数据库中读取的值。</returns>
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            // 如果尚未为此属性配置 TypeHandler（类型转换器），则动态决定
            if (mapping.TypeHandler == null || mapping.TypeHandler is UnknownTypeHandler) // Find the TypeHandler
            {
                // 多线程下加锁，避免重复初始化
                lock (mapping)
                {
                    if (mapping.TypeHandler == null || mapping.TypeHandler is UnknownTypeHandler)
                    {
                        int _columnIndex = 0;
                        // 如果字段索引未知，通过列名查找索引
                        if (mapping.ColumnIndex == ResultProperty.UNKNOWN_COLUMN_INDEX)
                        {
                            _columnIndex = reader.GetOrdinal(mapping.ColumnName);
                        }
                        else// 否则使用已知索引
                        {
                            _columnIndex = mapping.ColumnIndex;
                        }
                        // 获取字段在数据库中的类型（如 System.String, System.Int32 等）
                        Type systemType = ((IDataRecord)reader).GetFieldType(_columnIndex);
                        // 通过 TypeHandlerFactory 获取匹配的类型处理器（TypeHandler）
                        mapping.TypeHandler = request.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(systemType);
                    }
                }
            }

            // 使用 mapping 封装的方法，从 reader 中读取字段值
            object dataBaseValue = mapping.GetDataBaseValue(reader);
            // 如果数据库字段值不为 null，则设置标记，表示读取到有效数据
            request.IsRowDataFound = request.IsRowDataFound || (dataBaseValue != null);
            return dataBaseValue;
        }

        #endregion
    }
}
