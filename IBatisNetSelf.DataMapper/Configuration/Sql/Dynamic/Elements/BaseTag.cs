using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// Summary description for BaseTag.
    /// </summary>
    [Serializable]
    public abstract class BaseTag : SqlTag
    {
        #region Fields

        [NonSerialized]
        private string property = string.Empty;

        #endregion

        /// <summary>
        /// Property attribute
        /// </summary>
        [XmlAttribute("property")]
        public string Property
        {
            get=>property;
            set=>property = value;
        }
    }
}
