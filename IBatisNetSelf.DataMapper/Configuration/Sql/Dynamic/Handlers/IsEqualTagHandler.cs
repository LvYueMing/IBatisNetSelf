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
    /// Summary description for IsEqualTagHandler.
    /// </summary>
    public class IsEqualTagHandler : ConditionalTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEqualTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsEqualTagHandler(AccessorFactory accessorFactory): base(accessorFactory)
        {
        }

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aTag"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        public override bool IsCondition(SqlTagContext aContext, SqlTag aTag, object aParameterObject)
        {
            return (this.Compare(aContext, aTag, aParameterObject) == 0);
        }
        #endregion

    }
}
