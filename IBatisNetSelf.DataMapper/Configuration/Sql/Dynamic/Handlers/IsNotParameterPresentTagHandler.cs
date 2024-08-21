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
    /// Summary description for IsNotParameterPresentTagHandler.
    /// </summary>
    public sealed class IsNotParameterPresentTagHandler : IsParameterPresentTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotParameterPresentTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsNotParameterPresentTagHandler(AccessorFactory accessorFactory)
            : base(accessorFactory)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public override bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            return !base.IsCondition(ctx, tag, parameterObject);
        }
    }
}
