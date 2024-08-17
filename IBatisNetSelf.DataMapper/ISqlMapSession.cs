using IBatisNetSelf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// SqlMap Session contract
    /// </summary>
    public interface ISqlMapSession : IDalSession
    {

        /// <summary>
        /// Gets the SQL mapper.
        /// </summary>
        /// <value>The SQL mapper.</value>
        ISqlMapper SqlMapper { get; }

        /// <summary>
        /// Create the connection
        /// </summary>
        void CreateConnection();

        /// <summary>
        /// Create the connection
        /// </summary>
        void CreateConnection(string connectionString);
    }
}
