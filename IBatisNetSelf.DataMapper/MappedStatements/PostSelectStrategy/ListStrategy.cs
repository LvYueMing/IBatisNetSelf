using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// <see cref="IPostSelectStrategy"/> implementation to exceute a query for list.
    /// </summary>
    public sealed class ListStrategy : IPostSelectStrategy
    {
        #region IPostSelectStrategy Members

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            object values = postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, values);
        }

        #endregion
    }
}
