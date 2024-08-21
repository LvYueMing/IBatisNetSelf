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
    /// Represent an isNotEmpty sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isNotEmpty", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsNotEmpty : BaseTag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotEmpty"/> class.
        /// </summary>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public IsNotEmpty(AccessorFactory aAccessorFactory)
        {
            this.Handler = new IsNotEmptyTagHandler(aAccessorFactory);
        }
    }
}
