using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// BaseStrategy.
    /// </summary>
    public abstract class BaseStrategy
    {
        /// <summary>
        /// 用于 N+1 Select（懒加载或子对象批量加载）解决方案中的占位符对象。
        /// </summary>
        public static object SKIP = new object();

        /// 用作唯一键的分隔符（使用 \u0002，为了避免与数据库常用字符冲突）。
        private const string KEY_SEPARATOR = "\002";


        /// <summary>
        /// 计算唯一键，用于标识当前 <see cref="IResultMap"/> 所构建的结果对象。
        /// 主要用于缓存或判断某个对象是否已加载（如解决 N+1 问题）。
        /// </summary>
        /// <param name="resultMap">当前使用的结果映射</param>
        /// <param name="request">请求作用域对象，封装上下文数据</param>
        /// <param name="reader">数据库读取器</param>
        /// <returns>唯一标识当前结果对象的键字符串</returns>
        protected string GetUniqueKey(IResultMap resultMap, RequestScope request, IDataReader reader)
        {
            // 如果配置了 GroupBy 属性（用于指定组合键的字段）
            if (resultMap.GroupByProperties.Count > 0)
            {
                StringBuilder keyBuffer = new StringBuilder();
                // 遍历每个分组字段，取值并拼接
                for (int i = 0; i < resultMap.GroupByProperties.Count; i++)
                {
                    ResultProperty resultProperty = resultMap.GroupByProperties[i];
                    // 从数据库读取当前字段值
                    keyBuffer.Append(resultProperty.GetDataBaseValue(reader));
                    keyBuffer.Append('-');// 用 '-' 分隔各字段值
                }

                // 如果没有拼接内容，说明未配置字段，返回 null
                if (keyBuffer.Length < 1)
                {
                    // 正常情况下不会进入此分支
                    return null;
                }
                else
                {
                    // 添加一个极不可能出现在数据库中的分隔符作为结尾标识
                    keyBuffer.Append(KEY_SEPARATOR);
                    return keyBuffer.ToString();
                }
            }
            else
            {
                // 未配置 GroupBy 属性，返回 null（也是不推荐的情况）
                return null;
            }
        }

        /// <summary>
        /// 根据指定的 ResultMap 和数据读取器，将数据填充到结果对象中。
        /// </summary>
        /// <param name="request">当前请求作用域</param>
        /// <param name="reader">数据读取器，用于读取每一列</param>
        /// <param name="resultMap">对象的映射配置</param>
        /// <param name="resultObject">目标对象（用于接收填充的字段值）</param>
        /// <returns>如果成功读取到数据，返回 true；否则返回 false</returns>
        protected bool FillObjectWithReaderAndResultMap(RequestScope request, IDataReader reader,
                                                        IResultMap resultMap, ref object resultObject)
        {
            bool dataFound = false;
            // 如果当前映射包含属性字段（映射项）
            if (resultMap.Properties.Count > 0)
            {
                // 遍历所有映射的属性项
                for (int index = 0; index < resultMap.Properties.Count; index++)
                {
                    // 每次重置本轮读取的标记
                    request.IsRowDataFound = false;
                    // 获取当前属性
                    ResultProperty property = resultMap.Properties[index];
                    // 使用该属性的策略（如简单值或嵌套对象）设置对象值
                    property.PropertyStrategy.Set(request, resultMap, property, ref resultObject, reader, null);
                    dataFound = dataFound || request.IsRowDataFound;
                }

                request.IsRowDataFound = dataFound;
                return dataFound;
            }
            else
            {
                return true;
            }
        }

    }
}
