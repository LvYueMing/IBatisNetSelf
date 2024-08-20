using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// Accessor factory
    /// </summary>
    public class AccessorFactory
    {
        private ISetAccessorFactory setAccessorFactory = null;
        private IGetAccessorFactory getAccessorFactory = null;

        /// <summary>
        /// The factory which build <see cref="ISetAccessor"/>
        /// </summary>
        public ISetAccessorFactory SetAccessorFactory => this.setAccessorFactory;


        /// <summary>
        /// The factory which build <see cref="IGetAccessor"/>
        /// </summary>
        public IGetAccessorFactory GetAccessorFactory => this.getAccessorFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="AccessorFactory"/> class.
        /// </summary>
        /// <param name="setAccessorFactory">The set accessor factory.</param>
        /// <param name="getAccessorFactory">The get accessor factory.</param>
        public AccessorFactory(ISetAccessorFactory setAccessorFactory, IGetAccessorFactory getAccessorFactory)
        {
            this.setAccessorFactory = setAccessorFactory;
            this.getAccessorFactory = getAccessorFactory;
        }
    }
}
