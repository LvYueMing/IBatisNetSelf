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
    /// Represent an isNull sql tag element.
    /// </summary>
    [Serializable]
    [XmlRoot("isNull", Namespace = "http://ibatis.apache.org/mapping")]
    public sealed class IsNull : BaseTag
    {
        /// <summary>
        /// 
        /// </summary>
        public IsNull(AccessorFactory accessorFactory)
        {
            this.Handler = new IsNullTagHandler(accessorFactory);
        }
    }
}
