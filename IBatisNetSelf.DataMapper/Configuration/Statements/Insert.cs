using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Represent an insert statement.
    /// </summary>
    [Serializable]
    [XmlRoot("insert", Namespace = "http://ibatis.apache.org/mapping")]
    public class Insert : Statement
    {
        #region Fields
        [NonSerialized]
        private SelectKey _selectKey = null;
        [NonSerialized]
        private Generate _generate = null;
        #endregion

        #region Properties
        /// <summary>
        /// Extend statement attribute
        /// </summary>
        [XmlIgnore]
        public override string ExtendStatement
        {
            get { return string.Empty; }
            set { }
        }

        /// <summary>
        /// The selectKey statement used by an insert statement.
        /// </summary>
        [XmlElement("selectKey", typeof(SelectKey))]
        public SelectKey SelectKey
        {
            get { return _selectKey; }
            set { _selectKey = value; }
        }

        /// <summary>
        /// The Generate tag used by a generated insert statement.
        /// (CRUD operation)
        /// </summary>
        [XmlElement("generate", typeof(Generate))]
        public Generate Generate
        {
            get { return _generate; }
            set { _generate = value; }
        }
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public Insert() : base()
        {
        }
        #endregion


    }
}
