using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common
{
    /// <summary>
    /// IDataSource
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// DataSource Name.
        /// </summary>
        string Name { set; get; }
        /// <summary>
        /// Connection string used to create connections.
        /// </summary>
        string ConnectionString { set; get; }

        /// <summary>
        /// The data provider.
        /// </summary>
        IDbProvider DbProvider { set; get; }
    }
}
