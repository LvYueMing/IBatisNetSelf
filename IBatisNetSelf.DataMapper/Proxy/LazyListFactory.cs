using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.MappedStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Proxy
{
    /// <summary>
    /// Implementation of <see cref="ILazyFactory"/> to create proxy for an <see cref="IList"/> element.
    /// </summary>
    public class LazyListFactory : ILazyFactory
    {
        #region ILazyFactory Members

        /// <summary>
        /// Create a new proxy instance.
        /// </summary>
        /// <param name="mappedStatement">The mapped statement.</param>
        /// <param name="param">The param.</param>
        /// <param name="target">The target.</param>
        /// <param name="setAccessor">The set accessor.</param>
        /// <returns>Returns a new proxy.</returns>
        public object CreateProxy(IMappedStatement mappedStatement, object param,
            object target, ISetAccessor setAccessor)
        {
            return new LazyList(mappedStatement, param, target, setAccessor);
        }

        #endregion
    }
}
