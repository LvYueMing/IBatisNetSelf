using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// 用于获取 <see cref="IResultStrategy"/> 实例的工厂类。
    /// </summary>
    public sealed class ResultStrategyFactory
    {
        // 结果是 ResultClass 时的策略（通常是自动映射到 POCO）
        private static IResultStrategy resultClassStrategy = null;
        // 结果是 ResultMap 时的策略（用户在 XML 中自定义 <resultMap>）
        private static IResultStrategy mapStrategy = null;
        // 结果 没有映射配置（ResultClass、ResultMap） 时的策略（直接返回对象，比如 int/string 等）
        private static IResultStrategy objectStrategy = null;

        /// <summary>
        /// 静态构造函数（只执行一次），初始化所有策略实例。
        /// </summary>
        static ResultStrategyFactory()
        {
            // 初始化“MapStrategy”：使用 <resultMap> 自定义的字段映射方式
            mapStrategy = new MapStrategy();
            // 初始化“ResultClassStrategy”：通过反射自动映射到 POCO 类的属性
            resultClassStrategy = new ResultClassStrategy();
            // 初始化“ObjectStrategy”：直接返回简单值类型（如 int、string、DateTime 等）
            objectStrategy = new ObjectStrategy();
        }

        /// <summary>
        /// 根据 <see cref="IStatement"/> 的配置，返回对应的结果映射策略。
        /// </summary>
        /// <param name="statement">语句定义对象</param>
        /// <returns>对应的 <see cref="IResultStrategy"/> 实例</returns>
        public static IResultStrategy Get(IStatement statement)
        {
            // 如果定义了结果映射（ResultMap 或 AutoResultMap(resultClass 标记自动生成的)）
            if (statement.ResultsMap.Count > 0)
            {
                // 如果是显式配置的 <resultMap>，使用 MapStrategy 策略
                if (statement.ResultsMap[0] is ResultMap)
                {
                    return mapStrategy;
                }
                else // 否则是自动生成的 AutoResultMap，使用 ResultClassStrategy 策略
                {
                    return resultClassStrategy;
                }
            }
            else
            {
                return objectStrategy;
            }
        }
    }

}
