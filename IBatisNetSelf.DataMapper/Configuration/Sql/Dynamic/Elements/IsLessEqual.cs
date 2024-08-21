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
    /// Represent an isLessEqual sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isLessEqual", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsLessEqual : Conditional
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsLessEqual"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsLessEqual(AccessorFactory accessorFactory)
        {
            this.Handler = new IsLessEqualTagHandler(accessorFactory);
        }
    }
}
