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
    /// Represent an isPropertyAvailable sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isPropertyAvailable", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsPropertyAvailable : BaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsPropertyAvailable"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsPropertyAvailable(AccessorFactory accessorFactory)
        {
            this.Handler = new IsPropertyAvailableTagHandler(accessorFactory);
        }
    }
}
