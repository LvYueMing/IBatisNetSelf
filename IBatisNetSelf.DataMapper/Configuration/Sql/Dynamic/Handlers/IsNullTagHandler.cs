using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Summary description for IsNullTagHandler.
    /// </summary>
    public class IsNullTagHandler : ConditionalTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsNullTagHandler(AccessorFactory accessorFactory)
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
            if (parameterObject == null)
            {
                return true;
            }
            else
            {
                string propertyName = ((BaseTag)tag).Property;
                object value;
                if (propertyName != null && propertyName.Length > 0)
                {
                    value = ObjectProbe.GetMemberValue(parameterObject, propertyName, this.AccessorFactory);
                }
                else
                {
                    value = parameterObject;
                }
                return (value == null);
            }
        }
    }
}
