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
    /// Represent an isNotEqual sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isNotEqual", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsNotEqual : Conditional
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEqual"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsNotEqual(AccessorFactory accessorFactory)
        {
            this.Handler = new IsNotEqualTagHandler(accessorFactory);
        }
    }
}
