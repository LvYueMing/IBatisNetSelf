using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Summary description for IsGreaterEqualTagHandler.
    /// </summary>
    public sealed class IsGreaterEqualTagHandler : ConditionalTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterEqualTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsGreaterEqualTagHandler(AccessorFactory accessorFactory)
            : base(accessorFactory)
        {
        }

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            long x = this.Compare(ctx, tag, parameterObject);
            return ((x >= 0) && (x != ConditionalTagHandler.NOT_COMPARABLE));
        }
        #endregion
    }
}
