using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Cache
{
    /// <summary>
    /// Summary description for ICacheController.
    /// </summary>
    public interface ICacheController
    {
        #region Properties
        /// <summary>
        /// Adds an item with the specified key and value into cached data.
        /// Gets a cached object with the specified key.
        /// </summary>
        /// <value>The cached object or <c>null</c></value>
        object this[object key]
        {
            get;
            set;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Remove an object from a cache model
        /// </summary>
        /// <param name="key">the key to the object</param>
        /// <returns>the removed object(?)</returns>
        object Remove(object key);

        /// <summary>
        /// Clears all elements from the cache.
        /// </summary>
        void Flush();

        /// <summary>
        /// Configures the CacheController
        /// </summary>
        /// <param name="properties"></param>
        void Configure(IDictionary properties);
        #endregion

    }
}
