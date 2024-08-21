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
    /// Summary description for IsNotEmptyTagHandler.
    /// </summary>
    public sealed class IsNotEmptyTagHandler : IsEmptyTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEmptyTagHandler"/> class.
        /// </summary>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public IsNotEmptyTagHandler(AccessorFactory aAccessorFactory)
            : base(aAccessorFactory)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aTag"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        public override bool IsCondition(SqlTagContext aContext, SqlTag aTag, object aParameterObject)
        {
            return !base.IsCondition(aContext, aTag, aParameterObject);
        }
    }
}
