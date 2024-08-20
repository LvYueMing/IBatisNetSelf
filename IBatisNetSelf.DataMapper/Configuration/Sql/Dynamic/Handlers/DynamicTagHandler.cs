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
    /// Summary description for DynamicTagHandler.
    /// </summary>
    public sealed class DynamicTagHandler : BaseTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public DynamicTagHandler(AccessorFactory accessorFactory)
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
        public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, Object parameterObject)
        {
            ctx.FirstNonDynamicTagWithPrepend = null;
            if (tag.IsPrependAvailable)
            {
                ctx.IsOverridePrepend = true;
            }
            return BaseTagHandler.INCLUDE_BODY;
        }
        #endregion

    }
}
