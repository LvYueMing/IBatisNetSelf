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
    /// Represent an isEmpty sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isEmpty", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsEmpty : BaseTag
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="IsEmpty"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsEmpty(AccessorFactory accessorFactory)
        {
            this.Handler = new IsEmptyTagHandler(accessorFactory);
        }

    }
}
