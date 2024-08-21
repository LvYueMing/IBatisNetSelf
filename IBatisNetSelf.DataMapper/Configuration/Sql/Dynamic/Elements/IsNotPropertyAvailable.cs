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
    /// Represent an isEmpty sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isNotPropertyAvailable", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsNotPropertyAvailable : BaseTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotPropertyAvailable"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsNotPropertyAvailable(AccessorFactory accessorFactory)
        {
            this.Handler = new IsNotPropertyAvailableTagHandler(accessorFactory);
        }
    }
}
