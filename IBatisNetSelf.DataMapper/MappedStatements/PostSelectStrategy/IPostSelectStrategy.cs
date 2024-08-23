using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// <see cref="IPostSelectStrategy"/> contract to excute a 'select' <see cref="ResultProperty"/>
    /// after the process of the main <see cref="IDataReader"/>
    /// </summary>
    public interface IPostSelectStrategy
    {

        /// <summary>
        /// Executes the specified <see cref="PostBindind"/>.
        /// </summary>
        /// <param name="postSelect">The <see cref="PostBindind"/>.</param>
        /// <param name="request">The <see cref="RequestScope"/></param>
        void Execute(PostBindind postSelect, RequestScope request);
    }
}
