using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements
{
    /// <summary>
    /// Summary description for DynamicTag.
    /// </summary>
    [Serializable]
    [XmlRoot("dynamic", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class Dynamic : SqlTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dynamic"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public Dynamic(AccessorFactory accessorFactory)
        {
            this.Handler = new DynamicTagHandler(accessorFactory);
        }

    }
}
