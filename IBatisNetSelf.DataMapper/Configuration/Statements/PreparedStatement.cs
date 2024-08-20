using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Construct the list of IDataParameters for the statement and prepare the sql
    /// </summary>
    public class PreparedStatement
    {
        #region Fields

        private string preparedSql = string.Empty;
        private StringCollection dbParametersName = new StringCollection();
        private IDbDataParameter[] dbParameters = null;

        #endregion

        #region Properties


        /// <summary>
        /// The list of IDataParameter name used by the PreparedSql.
        /// </summary>
        public StringCollection DbParametersName => dbParametersName;

        /// <summary>
        /// The list of IDataParameter to use for the PreparedSql.
        /// </summary>
        public IDbDataParameter[] DbParameters
        {
            get => dbParameters;
            set => dbParameters = value;
        }

        /// <summary>
        /// The prepared statement.
        /// </summary>
        public string PreparedSql
        {
            get => preparedSql;
            set => preparedSql = value;
        }

        #endregion
    }
}
