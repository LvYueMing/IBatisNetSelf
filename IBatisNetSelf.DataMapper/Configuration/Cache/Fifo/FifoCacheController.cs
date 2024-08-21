using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Cache.Fifo
{
    /// <summary>
    /// Summary description for FifoCacheController.
    /// </summary>
    public class FifoCacheController : ICacheController
    {
        #region Fields 
        private int _cacheSize = 0;
        private Hashtable _cache = null;
        private IList _keyList = null;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// 
        /// </summary>
        public FifoCacheController()
        {
            _cacheSize = 100;
            _cache = Hashtable.Synchronized(new Hashtable());
            _keyList = ArrayList.Synchronized(new ArrayList());
        }
        #endregion

        #region ICacheController Members

        /// <summary>
        /// Remove an object from a cache model
        /// </summary>
        /// <param name="key">the key to the object</param>
        /// <returns>the removed object(?)</returns>
        public object Remove(object key)
        {
            object o = this[key];

            _keyList.Remove(key);
            _cache.Remove(key);
            return o;
        }

        /// <summary>
        /// Clears all elements from the cache.
        /// </summary>
        public void Flush()
        {
            _cache.Clear();
            _keyList.Clear();
        }


        /// <summary>
        /// Adds an item with the specified key and value into cached data.
        /// Gets a cached object with the specified key.
        /// </summary>
        /// <value>The cached object or <c>null</c></value>
        public object this[object key]
        {
            get
            {
                return _cache[key];
            }
            set
            {
                _cache[key] = value;
                _keyList.Add(key);
                if (_keyList.Count > _cacheSize)
                {
                    object oldestKey = _keyList[0];
                    _keyList.Remove(0);
                    _cache.Remove(oldestKey);
                }
            }
        }


        /// <summary>
        /// Configures the cache
        /// </summary>
        public void Configure(IDictionary properties)
        {
            string size = (string)properties["CacheSize"]; ;
            if (size != null)
            {
                _cacheSize = Convert.ToInt32(size);
            }
        }

        #endregion

    }
}
