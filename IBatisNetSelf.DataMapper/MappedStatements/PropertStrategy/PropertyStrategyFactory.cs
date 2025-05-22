using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// 【属性策略工厂】- 根据ResultProperty对象获取对应的属性处理策略
    /// </summary>
    /// <remarks>
    /// 采用静态初始化+单例模式保证线程安全
    /// </remarks>
    public sealed class PropertyStrategyFactory
    {
        // 各策略的静态实例（线程安全单例）
        private static IPropertyStrategy defaultStrategy = null;          // 默认基础策略
        private static IPropertyStrategy resultMapStrategy = null;       // 结果映射策略
        private static IPropertyStrategy groupByStrategy = null;         // 分组处理策略

        private static IPropertyStrategy selectArrayStrategy = null;     // 数组选择策略
        private static IPropertyStrategy selectGenericListStrategy = null; // 泛型列表选择策略
        private static IPropertyStrategy selectListStrategy = null;      // 列表选择策略
        private static IPropertyStrategy selectObjectStrategy = null;    // 对象选择策略

        /// <summary>
        /// 静态构造函数（类首次使用时自动执行）
        /// </summary>
        static PropertyStrategyFactory()
        {
            // 初始化所有策略实例（整个应用生命周期内只执行一次）
            defaultStrategy = new DefaultStrategy();           // 基础属性映射
            resultMapStrategy = new ResultMapStrategy();       // 嵌套结果映射
            groupByStrategy = new GroupByStrategy();           // 分组聚合操作

            selectArrayStrategy = new SelectArrayStrategy();   // 处理数组类型结果
            selectListStrategy = new SelectListStrategy();    // 处理非泛型集合
            selectObjectStrategy = new SelectObjectStrategy(); // 处理简单对象
        }

        /// <summary>
        /// 根据ResultProperty对象获取对应的处理策略
        /// </summary>
        /// <param name="mapping">ResultProperty对象</param>
        /// <returns>
        /// 适配当前场景的<see cref="IPropertyStrategy"/>实现
        /// </returns>
        /// <exception cref="ArgumentNullException">当mapping参数为null时</exception>
        public static IPropertyStrategy Get(ResultProperty mapping)
        {
            /* 策略选择逻辑优先级：
               1. 普通属性映射（无select/resultMap配置）
               2. 嵌套结果映射（resultMap属性）
                 2.1 分组聚合场景
                 2.2 集合类型处理
                 2.3 普通嵌套对象
               3. 关联查询映射（select属性）
            */

            // 场景1：基础属性映射（无select/resultMap配置）
            if (mapping.Select.Length == 0 && mapping.NestedResultMap == null)
            {
                return defaultStrategy; // 使用默认策略处理简单字段映射
            }
            // 场景2：嵌套结果映射配置（resultMap属性）
            else if (mapping.NestedResultMap != null)
            {
                // 场景2.1：存在分组配置
                if (mapping.NestedResultMap.GroupByPropertyNames.Count > 0)
                {
                    return groupByStrategy; // 启用分组处理逻辑
                }
                // 场景2.2：目标属性是集合类型
                else if (typeof(IList).IsAssignableFrom(mapping.MemberType))
                {
                    return groupByStrategy; // 集合类型也使用分组策略
                }
                // 场景2.3：普通嵌套对象
                else
                {
                    return resultMapStrategy; // 标准嵌套对象映射
                }
            }
            // 场景3：关联查询映射（select属性）
            else
            {
                /* 动态创建选择策略（根据属性类型自动适配）：
                   - 自动检测数组/泛型集合/普通集合/单对象等类型
                   - 传入预定义的策略实例避免重复创建
                */
                return new SelectStrategy(
                    mapping,                   // 当前属性配置
                    selectArrayStrategy,       // 数组处理器
                    selectGenericListStrategy, // 泛型集合处理器
                    selectListStrategy,       // 普通集合处理器
                    selectObjectStrategy      // 单对象处理器
                );
            }
        }
    }
}
