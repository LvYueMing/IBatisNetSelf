using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// Summary description for DynamicParent.
    /// </summary>
    public interface IDynamicParent
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        void AddChild(ISqlChild child);

    }
}
