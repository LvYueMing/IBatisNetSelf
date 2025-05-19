using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// Summary description for ExecuteEventArgs.
    /// </summary>
    public class ExecuteEventArgs : EventArgs
    {
        private string statementName = string.Empty;

        /// <summary>
        /// Set or get the statement name
        /// </summary>
        public string StatementName
        {
            get
            {
                return statementName;
            }
            set
            {
                statementName = value;
            }
        }
    }
}
