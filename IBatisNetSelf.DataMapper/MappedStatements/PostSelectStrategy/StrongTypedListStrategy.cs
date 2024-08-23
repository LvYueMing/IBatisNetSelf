using IBatisNetSelf.Common.Utilities.Objects;
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
    /// <see cref="IPostSelectStrategy"/> implementation to exceute a query for 
    /// strong typed list.
    /// </summary>
    public sealed class StrongTypedListStrategy : IPostSelectStrategy
    {
        #region IPostSelectStrategy Members

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        public void Execute(PostBindind postSelect, RequestScope request)
        {
            IFactory factory = request.DataExchangeFactory.ObjectFactory.CreateFactory(postSelect.ResultProperty.SetAccessor.MemberType, Type.EmptyTypes);
            object values = factory.CreateInstance(null);
            postSelect.Statement.ExecuteQueryForList(request.Session, postSelect.Keys, (IList)values);
            postSelect.ResultProperty.SetAccessor.Set(postSelect.Target, values);
        }

        #endregion
    }
}
