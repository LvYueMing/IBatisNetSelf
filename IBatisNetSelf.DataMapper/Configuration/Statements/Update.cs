using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for Update.
    /// </summary>
    [Serializable]
    [XmlRoot("update", Namespace = "http://ibatis.apache.org/mapping")]
    public class Update : Statement
    {

        #region Fields
        [NonSerialized]
        private Generate generate = null;
        #endregion

        /// <summary>
        /// The Generate tag used by a generated update statement.
        /// (CRUD operation)
        /// </summary>
        [XmlElement("generate", typeof(Generate))]
        public Generate Generate
        {
            get { return generate; }
            set { generate = value; }
        }

        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public Update() : base()
        { }

    }
}
