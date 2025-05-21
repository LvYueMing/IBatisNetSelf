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
        private string aliasName = string.Empty;
        [NonSerialized]
        private string typeName = string.Empty;
        [NonSerialized]
        private Type type = null;
        #endregion

        #region Properties
        /// <summary>
        /// 用于唯一标识typeAlias别名的名称
        /// </summary>
        /// <example> Account</example>
        [XmlAttribute("alias")]
        public string AliasName
        {
            get { return aliasName; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The name attribute is mandatory in the typeAlias ");
                }
                aliasName = value;
            }
        }


        /// <summary>
        /// The type class for the typeAlias
        /// </summary>
        [XmlIgnore]
        public Type Type
        {
            get { return type; }
        }


        /// <summary>
        /// typeAlias 对应的类型类
        /// </summary>
        /// <example>Com.Site.Domain.Product</example>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set
            {
                if ((value == null) || (value.Length < 1))
                {
                    throw new ArgumentNullException("The class attribute is mandatory in the typeAlias " + aliasName);
                }
                typeName = value;
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
            this.type = type;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the object, 
        /// try to idenfify the .Net type class from the corresponding name.
        /// </summary>
        public void Initialize()
        {
            type = TypeUtils.ResolveType(typeName);
        }
        #endregion

    }
}
