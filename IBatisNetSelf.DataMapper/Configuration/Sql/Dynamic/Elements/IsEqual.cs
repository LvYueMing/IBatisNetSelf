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
    /// Represent an isEqual sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isEqual", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsEqual : Conditional
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsEqual"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsEqual(AccessorFactory accessorFactory)
        {
            this.Handler = new IsEqualTagHandler(accessorFactory);
        }
    }
}
