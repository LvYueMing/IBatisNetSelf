using IBatisNetSelf.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Commands
{
    /// <summary>
    /// For <see cref="IDataReader"/> which don't support M.A.R.S, wraps the current <see cref="IDataReader"/>
    /// in an <see cref="InMemoryDataReader"/>.
    /// </summary>
    public sealed class DataReaderTransformer
    {

        /// <summary>
        ///  Creates a DataReaderAdapter from a <see cref="IDataReader" />
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader" /> which holds the records from the Database.</param>
        /// <param name="dbProvider">The databse provider <see cref="IDbProvider"/></param>
        public static IDataReader Transform(IDataReader reader, IDbProvider dbProvider)
        {
            if (!dbProvider.AllowMARS && !(reader is InMemoryDataReader))
            {
                // The underlying reader will be closed.
                return new InMemoryDataReader(reader);
            }
            else
            {
                return reader;
            }
        }
    }
}
