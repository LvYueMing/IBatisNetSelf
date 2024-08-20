using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Summary description for SqlTagContext.
    /// </summary>
    public sealed class SqlTagContext
    {
        #region Fields
        private Hashtable attributes = new Hashtable();
        private bool overridePrepend = false;
        private SqlTag firstNonDynamicTagWithPrepend = null;
        private ArrayList parameterMappings = new ArrayList();
        private StringBuilder bodyTextbuffer = new StringBuilder();
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public string BodyText => this.bodyTextbuffer.ToString().Trim();

        /// <summary>
        /// 
        /// </summary>
        public bool IsOverridePrepend
        {
            set => this.IsOverridePrepend = value;
            get => this.overridePrepend;
        }

        /// <summary>
        /// 
        /// </summary>
        public SqlTag FirstNonDynamicTagWithPrepend
        {
            get=>this.firstNonDynamicTagWithPrepend;
            set=>this.firstNonDynamicTagWithPrepend = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public SqlTagContext()
        {
            this.overridePrepend = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetWriter()
        {
            return this.bodyTextbuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddAttribute(object key, object value)
        {
            this.attributes.Add(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetAttribute(object key)
        {
            return this.attributes[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapping"></param>
        public void AddParameterMapping(ParameterProperty mapping)
        {
            this.parameterMappings.Add(mapping);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList GetParameterMappings()
        {
            return this.parameterMappings;
        }
    }
}
