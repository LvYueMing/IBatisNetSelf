using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// Summary description for Conditional.
    /// </summary>
    [Serializable]
    public abstract class Conditional : BaseTag
    {

        #region Fields

        [NonSerialized]
        private string compareValue = string.Empty;
        [NonSerialized]
        private string compareProperty = string.Empty;

        #endregion

        /// <summary>
        /// CompareProperty attribute
        /// </summary>
        [XmlAttribute("compareProperty")]
        public string CompareProperty
        {
            get=>compareProperty;
            set=>compareProperty = value;
        }


        /// <summary>
        /// CompareValue attribute
        /// </summary>
        [XmlAttribute("compareValue")]
        public string CompareValue
        {
            get=>compareValue;
            set=>compareValue = value;
        }
    }
}
