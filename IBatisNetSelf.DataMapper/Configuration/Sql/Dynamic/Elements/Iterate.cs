using IBatisNetSelf.Common.Utilities.Objects.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// Represent an iterate sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("iterate", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class Iterate : BaseTag
    {

        #region Fields

        [NonSerialized]
        private string _open = string.Empty;
        [NonSerialized]
        private string _close = string.Empty;
        [NonSerialized]
        private string _conjunction = string.Empty;

        #endregion


        /// <summary>
        /// Conjonction attribute
        /// </summary>
        [XmlAttribute("conjunction")]
        public string Conjunction
        {
            get
            {
                return _conjunction;
            }
            set
            {
                _conjunction = value;
            }
        }


        /// <summary>
        /// Close attribute
        /// </summary>
        [XmlAttribute("close")]
        public string Close
        {
            get
            {
                return _close;
            }
            set
            {
                _close = value;
            }
        }


        /// <summary>
        /// Open attribute
        /// </summary>
        [XmlAttribute("open")]
        public string Open
        {
            get
            {
                return _open;
            }
            set
            {
                _open = value;
            }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="Iterate"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public Iterate(AccessorFactory accessorFactory)
        {
            this.Handler = new IterateTagHandler(accessorFactory);
        }

    }
}
