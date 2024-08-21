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
    /// Represent an isNotNull sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isNotNull", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsNotNull : BaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotNull"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsNotNull(AccessorFactory accessorFactory)
        {
            this.Handler = new IsNotNullTagHandler(accessorFactory);
        }
    }
}
