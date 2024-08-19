using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// <see cref="IPropertyStrategy"/> contract to set value object on <see cref="ResultProperty"/>.
    /// </summary>
    public interface IPropertyStrategy
    {
        /// <summary>
        /// Sets value of the specified <see cref="ResultProperty"/> on the target object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The ResultProperty.</param>
        /// <param name="target">The target.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="keys">The keys</param>
        void Set(RequestScope request, IResultMap resultMap,
                 ResultProperty mapping, ref object target,
                 IDataReader reader, object keys);


        /// <summary>
        /// Gets the value of the specified <see cref="ResultProperty"/> that must be set on the target object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="target">The target.</param>
        object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader);
    }
}
