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
    /// TypeAlias.
    /// </summary>
    [Serializable]
    [XmlRoot("typeAlias", Namespace = "http://ibatis.apache.org/dataMapper")]
    public class TypeAlias
    {

        #region Fields
        [NonSerialized]
        private string name = string.Empty;
        [NonSerialized]
        private string className = string.Empty;
        [NonSerialized]
        private Type @class = null;
        #endregion

        #region Properties
        /// <summary>
        /// Name used to identify the typeAlias amongst the others.
        /// </summary>
        /// <example> Account</example>
        [XmlAttribute("alias")]
        public string Name
        {
            get { return name; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The name attribute is mandatory in the typeAlias ");
                }
                name = value;
            }
        }


        /// <summary>
        /// The type class for the typeAlias
        /// </summary>
        [XmlIgnore]
        public Type Class
        {
            get { return @class; }
        }


        /// <summary>
        /// The class name to identify the typeAlias.
        /// </summary>
        /// <example>Com.Site.Domain.Product</example>
        [XmlAttribute("type")]
        public string ClassName
        {
            get { return className; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The class attribute is mandatory in the typeAlias " + name);
                }
                className = value;
            }
        }
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public TypeAlias()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">a type.</param>
        public TypeAlias(Type type)
        {
            @class = type;
        }
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
