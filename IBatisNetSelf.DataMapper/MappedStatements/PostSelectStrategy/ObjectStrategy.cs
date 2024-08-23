using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// <see cref="IPostSelectStrategy"/> implementation to exceute a query for object.
    /// </summary>
    public sealed class ObjectStrategy : IPostSelectStrategy
    {
        #region IPostSelectStrategy Members

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            object value = postSelect.Statement.ExecuteQueryForObject(request.Session, postSelect.Keys);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, value);
        }

        #endregion
    }
}
