using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// <see cref="IPostSelectStrategy"/> implementation to exceute a query for <see cref="Array"/>.
    /// </summary>
    public sealed class ArrayStrategy : IPostSelectStrategy
    {
        #region IPostSelectStrategy Members

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            IList values = postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys);
            Type elementType = postSelect.ResultProperty.SetAccessor.MemberType.GetElementType();

            Array array = Array.CreateInstance(elementType, values.Count);
            int count = values.Count;
            for (int i = 0; i < count; i++)
            {
                array.SetValue(values[i], i);
            }

            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, array);
        }

        #endregion
    }
}
