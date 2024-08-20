using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic
{
    /// <summary>
    /// Summary description for SqlText.
    /// </summary>
    public sealed class SqlText : ISqlChild
    {

        #region Fields

        private string text = string.Empty;
        private bool isWhiteSpace = false;
        private ParameterProperty[] parameters = null;

        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                isWhiteSpace = text.Trim().Length == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWhiteSpace => isWhiteSpace;

        /// <summary>
        /// 
        /// </summary>
        public ParameterProperty[] Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }
        #endregion

    }
}
