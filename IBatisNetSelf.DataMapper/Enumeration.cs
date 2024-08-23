using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// Indicate if the generated key by a selectKey statement
    ///  concern a pre or post-generated key(预生成键、后生成键).
    /// </summary>
    public enum SelectKeyType : int
    {
        /// <summary>
        /// 
        /// </summary>
        pre = 1,
        /// <summary>
        /// 
        /// </summary>
        post = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CacheKeyType : int
    {
        /// <summary>
        /// 
        /// </summary>
        Object = 1,
        /// <summary>
        /// 
        /// </summary>
        List = 2,
        /// <summary>
        /// 
        /// </summary>
        Map = 3
    }
}
