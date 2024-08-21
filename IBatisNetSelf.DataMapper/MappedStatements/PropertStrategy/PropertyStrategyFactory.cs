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
    /// Factory to get <see cref="IPropertyStrategy"/> implementation.
    /// </summary>
    public sealed class PropertyStrategyFactory
    {
        private static IPropertyStrategy defaultStrategy = null;
        private static IPropertyStrategy resultMapStrategy = null;
        private static IPropertyStrategy groupByStrategy = null;

        private static IPropertyStrategy selectArrayStrategy = null;
        private static IPropertyStrategy selectGenericListStrategy = null;
        private static IPropertyStrategy selectListStrategy = null;
        private static IPropertyStrategy selectObjectStrategy = null;

        /// <summary>
        /// Initializes the <see cref="PropertyStrategyFactory"/> class.
        /// </summary>
        static PropertyStrategyFactory()
        {
            defaultStrategy = new DefaultStrategy();
            resultMapStrategy = new ResultMapStrategy();
            groupByStrategy = new GroupByStrategy();

            selectArrayStrategy = new SelectArrayStrategy();
            selectListStrategy = new SelectListStrategy();
            selectObjectStrategy = new SelectObjectStrategy();
        }

        /// <summary>
        /// Finds the <see cref="IPropertyStrategy"/>.
        /// </summary>
        /// <param name="mapping">The <see cref="ResultProperty"/>.</param>
        /// <returns>The <see cref="IPropertyStrategy"/></returns>
        public static IPropertyStrategy Get(ResultProperty mapping)
        {
            // no 'select' or 'resultMap' attributes
            if (mapping.Select.Length == 0 && mapping.NestedResultMap == null)
            {
                // We have a 'normal' ResultMap
                return defaultStrategy;
            }
            else if (mapping.NestedResultMap != null) // 'resultMap' attribute
            {
                if (mapping.NestedResultMap.GroupByPropertyNames.Count > 0)
                {
                    return groupByStrategy;
                }
                else
                {
                    if (typeof(IList).IsAssignableFrom(mapping.MemberType))
                    {
                        return groupByStrategy;
                    }
                    else
                    {
                        return resultMapStrategy;
                    }
                }
            }
            else //'select' ResultProperty 
            {
                return new SelectStrategy(mapping,
                    selectArrayStrategy,
                    selectGenericListStrategy,
                    selectListStrategy,
                    selectObjectStrategy);
            }
        }
    }
}
