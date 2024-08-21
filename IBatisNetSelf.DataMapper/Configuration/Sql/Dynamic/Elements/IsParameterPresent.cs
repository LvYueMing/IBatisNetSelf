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
    /// Represent an isParameterPresent sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isParameterPresent", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsParameterPresent : SqlTag
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsParameterPresent"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IsParameterPresent(AccessorFactory accessorFactory)
        {
            this.Handler = new IsParameterPresentTagHandler(accessorFactory);
        }
    }
}
