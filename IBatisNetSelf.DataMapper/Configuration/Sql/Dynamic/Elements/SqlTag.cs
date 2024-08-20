using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// SqlTag is a children element of dynamic Sql element.
    /// SqlTag represent any binary unary/conditional element (like isEmpty, isNull, iEquall...) 
    /// or other element as isParameterPresent, isNotParameterPresent, iterate.
    /// </summary>
    [Serializable]
    public abstract class SqlTag : ISqlChild, IDynamicParent
    {

        #region Fields

        [NonSerialized]
        private string prepend = string.Empty;
        [NonSerialized]
        private ISqlTagHandler handler = null;
        [NonSerialized]
        private SqlTag parent = null;
        [NonSerialized]
        private IList children = new ArrayList();

        #endregion

        /// <summary>
        /// Parent tag element
        /// </summary>
        [XmlIgnore]
        public SqlTag Parent
        {
            get => this.Parent;
            set =>this.parent = value;
        }


        /// <summary>
        /// Prepend attribute
        /// </summary>
        [XmlAttribute("prepend")]
        public string Prepend
        {
            get => this.prepend;
            set => this.prepend = value;
        }


        /// <summary>
        /// Handler for this sql tag
        /// </summary>
        [XmlIgnore]
        public ISqlTagHandler Handler
        {
            get => this.handler;
            set => this.handler = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPrependAvailable
        {
            get => (this.prepend != null && this.prepend.Length > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetChildrenEnumerator()
        {
            return this.children.GetEnumerator();
        }

        #region IDynamicParent Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aChild"></param>
        public void AddChild(ISqlChild aChild)
        {
            if (aChild is SqlTag)
            {
                ((SqlTag)aChild).Parent = this;
            }
            this.children.Add(aChild);
        }

        #endregion
    }
}
