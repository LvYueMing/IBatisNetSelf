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
    /// Represent an isGreaterEqual sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isGreaterEqual", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsGreaterEqual : Conditional
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsGreaterEqual"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsGreaterEqual(AccessorFactory accessorFactory)
        {
            this.Handler = new IsGreaterEqualTagHandler(accessorFactory);
        }
    }
}
