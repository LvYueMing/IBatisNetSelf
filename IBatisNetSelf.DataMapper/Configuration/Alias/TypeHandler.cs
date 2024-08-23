using IBatisNetSelf.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Alias
{
    /// <summary>
    /// Summary description for TypeHandler.
    /// </summary>
    [Serializable]
    [XmlRoot("typeHandler", Namespace = "http://ibatis.apache.org/dataMapper")]
    public class TypeHandler
    {
        #region Fields
        [NonSerialized]
        private string className = string.Empty;
        [NonSerialized]
        private Type @class = null;
        [NonSerialized]
        private string dbType = string.Empty;
        [NonSerialized]
        private string callBackName = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// CLR type
        /// </summary>
        [XmlAttribute("type")]
        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        /// <summary>
        /// The type class for the TypeName
        /// </summary>
        [XmlIgnore]
        public Type Class
        {
            get { return @class; }
        }

        /// <summary>
        /// dbType name
        /// </summary>
        [XmlAttribute("dbType")]
        public string DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }


        /// <summary>
        /// callback alias name
        /// </summary>
        [XmlAttribute("callback")]
        public string CallBackName
        {
            get { return callBackName; }
            set { callBackName = value; }
        }


        #endregion

        #region Constructors

        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public TypeHandler()
        { }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the object, 
        /// try to idenfify the .Net type class from the corresponding name.
        /// </summary>
        public void Initialize()
        {
            @class = TypeUtils.ResolveType(className);
        }
        #endregion

    }
}
