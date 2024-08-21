using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy
{
    /// <summary>
    /// Factory to get <see cref="IArgumentStrategy"/> implementation.
    /// </summary>
    public sealed class ArgumentStrategyFactory
    {
        private static IArgumentStrategy _defaultStrategy = null;
        private static IArgumentStrategy _resultMapStrategy = null;
        private static IArgumentStrategy _selectArrayStrategy = null;
        private static IArgumentStrategy _selectGenericListStrategy = null;
        private static IArgumentStrategy _selectListStrategy = null;
        private static IArgumentStrategy _selectObjectStrategy = null;

        /// <summary>
        /// Initializes the <see cref="ArgumentStrategyFactory"/> class.
        /// </summary>
        static ArgumentStrategyFactory()
        {
            _defaultStrategy = new DefaultStrategy();
            _resultMapStrategy = new ResultMapStrategy();

            _selectArrayStrategy = new SelectArrayStrategy();
            _selectListStrategy = new SelectListStrategy();
            _selectObjectStrategy = new SelectObjectStrategy();
        }

        /// <summary>
        /// Finds the <see cref="IArgumentStrategy"/>.
        /// </summary>
        /// <param name="mapping">The <see cref="ArgumentProperty"/>.</param>
        /// <returns>The <see cref="IArgumentStrategy"/></returns>
        public static IArgumentStrategy Get(ArgumentProperty mapping)
        {
            // no 'select' or 'resultMap' attributes
            if (mapping.Select.Length == 0 && mapping.NestedResultMap == null)
            {
                // We have a 'normal' ResultMap
                return _defaultStrategy;
            }
            else if (mapping.NestedResultMap != null) // 'resultMap' attribut
            {
                return _resultMapStrategy;
            }
            else //'select' ResultProperty 
            {
                return new SelectStrategy(mapping,
                    _selectArrayStrategy,
                    _selectGenericListStrategy,
                    _selectListStrategy,
                    _selectObjectStrategy);
            }
        }
    }
}
