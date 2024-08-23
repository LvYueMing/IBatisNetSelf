using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Represent a generate tag element.
    /// The generation would happen at the point where the 
    /// SqlMapClient instance is built.
    /// </summary>
    [Serializable]
    [XmlRoot("generate", Namespace = "http://ibatis.apache.org/mapping")]
    public class Generate : Statement
    {
        #region Fields

        [NonSerialized]
        private string table = string.Empty;
        [NonSerialized]
        private string by = string.Empty;

        #endregion

        /// <summary>
        /// The table name used to build the SQL query. 
        /// </summary>
        /// <remarks>
        /// Will be used to get the metadata to build the SQL if needed.
        /// </remarks>
        [XmlAttribute("table")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// The by attribute is used to generate the where clause.
        /// </summary>
        /// <remarks>The by="" attribute can support multiple colums.</remarks>
        /// <example> 
        ///		&lt; delete ...&gt;
        ///			&lt;generate table="EMPLOYEE" by="EMPLOYEE_ID, LAST_MOD_DATE" /&gt;
        ///		&lt;/delete&gt;
        /// </example>
        [XmlAttribute("by")]
        public string By
        {
            get { return by; }
            set { by = value; }
        }

        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public Generate() : base() { }


    }
}
