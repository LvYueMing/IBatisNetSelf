using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for delete.
    /// </summary>
    [Serializable]
    [XmlRoot("delete", Namespace = "http://ibatis.apache.org/mapping")]
    public class Delete : Statement
    {

        #region Fields
        [NonSerialized]
        private Generate _generate = null;
        #endregion

        /// <summary>
        /// The Generate tag used by a generated delete statement.
        /// (CRUD operation)
        /// </summary>
        [XmlElement("generate", typeof(Generate))]
        public Generate Generate
        {
            get { return _generate; }
            set { _generate = value; }
        }

        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public Delete() : base()
        { }

    }
}
