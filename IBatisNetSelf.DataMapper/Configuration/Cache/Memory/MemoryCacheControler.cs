using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Cache.Memory
{
    /// <summary>
    /// Summary description for MemoryCacheControler.
    /// </summary>
    public class MemoryCacheControler : ICacheController
    {
        #region Fields 
        private MemoryCacheLevel cacheLevel = MemoryCacheLevel.Weak;
        private Hashtable cache = null;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryCacheControler()
        {
            this.cache = Hashtable.Synchronized(new Hashtable());
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
            object value = null;
            object reference = this[key];
            this.cache.Remove(key);
            if (reference != null)
            {
                if (reference is StrongReference)
                {
                    value = ((StrongReference)reference).Target;
                }
                else if (reference is WeakReference)
                {
                    value = ((WeakReference)reference).Target;
                }
            }
            return value;
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
                object value = null;
                object reference = this.cache[key];
                if (reference != null)
                {
                    if (reference is StrongReference)
                    {
                        value = ((StrongReference)reference).Target;
                    }
                    else if (reference is WeakReference)
                    {
                        value = ((WeakReference)reference).Target;
                    }
                }
                return value;
            }
            set
            {
                object reference = null;
                if (cacheLevel.Equals(MemoryCacheLevel.Weak))
                {
                    reference = new WeakReference(value);
                }
                else if (cacheLevel.Equals(MemoryCacheLevel.Strong))
                {
                    reference = new StrongReference(value);
                }
                this.cache[key] = reference;

            }
        }


        /// <summary>
        /// Clears all elements from the cache.
        /// </summary>
        public void Flush()
        {
            lock (this)
            {
                this.cache.Clear();
            }
        }


        /// <summary>
        /// Configures the cache
        /// </summary>
        public void Configure(IDictionary properties)
        {
            string referenceType = (string)properties["Type"]; ;
            if (referenceType != null)
            {
                cacheLevel = MemoryCacheLevel.GetByRefenceType(referenceType.ToUpper());
            }
        }

        #endregion

        /// <summary>
        /// Class to implement a strong (permanent) reference.
        /// </summary>
        private class StrongReference
        {
            private object _target = null;

            public StrongReference(object obj)
            {
                _target = obj;
            }

            /// <summary>
            /// Gets the object (the target) referenced by this instance.
            /// </summary>
            public object Target
            {
                get
                {
                    return _target;
                }
            }
        }
    }
}
