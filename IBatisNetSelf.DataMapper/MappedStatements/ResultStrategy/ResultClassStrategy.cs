using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// 当在映射配置中使用了 resultClass 属性时，对应的 <see cref="IResultStrategy"/> 实现类。
    /// 根据结果类的类型选择适当的映射策略（如简单类型、字典、列表或自动映射）。
    /// </summary>
    public sealed class ResultClassStrategy : IResultStrategy
    {
        // 简单类型（int, string, DateTime 等）映射策略
        private static IResultStrategy simpleTypeStrategy = null;
        // 字典类型（IDictionary）映射策略
        private static IResultStrategy dictionaryStrategy = null;
        // 列表类型（IList）映射策略
        private static IResultStrategy listStrategy = null;
        // 自动映射策略（将列自动映射到对象属性）
        private static IResultStrategy autoMapStrategy = null;

        /// <summary>
        /// 构造函数，初始化各种子策略对象。
        /// </summary>
        public ResultClassStrategy()
        {
            simpleTypeStrategy = new SimpleTypeStrategy();
            dictionaryStrategy = new DictionaryStrategy();
            listStrategy = new ListStrategy();
            autoMapStrategy = new AutoMapStrategy();
        }

        #region IResultStrategy Members

        /// <summary>
        /// 对一条数据记录进行映射处理，将其转换为 resultClass 指定的对象。
        /// </summary>
        /// <param name="aRequest">当前请求作用域（包含语句、映射信息）</param>
        /// <param name="aReader">数据读取器（IDataReader）</param>
        /// <param name="aResultObject">用于接收结果的对象（通常为 null）</param>
        /// <returns>映射完成的结果对象</returns>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {
            // 如果目标类是简单类型（如 int、string、bool），使用 SimpleTypeStrategy
            if (aRequest.DataExchangeFactory.TypeHandlerFactory.IsSimpleType(aRequest.CurrentResultMap.Class))
            {
                return simpleTypeStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            // 如果目标类是 IDictionary 类型（如 Hashtable、Dictionary<string, object>）
            else if (typeof(IDictionary).IsAssignableFrom(aRequest.CurrentResultMap.Class))
            {
                return dictionaryStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            // 如果目标类是 IList 类型（如 ArrayList、List<T>）
            else if (typeof(IList).IsAssignableFrom(aRequest.CurrentResultMap.Class))
            {
                return listStrategy.Process(aRequest, ref aReader, aResultObject);
            }
            // 默认情况：使用 AutoMapStrategy 将列名映射到对象属性
            else
            {
                return autoMapStrategy.Process(aRequest, ref aReader, aResultObject);
            }
        }

        #endregion
    } 
}
