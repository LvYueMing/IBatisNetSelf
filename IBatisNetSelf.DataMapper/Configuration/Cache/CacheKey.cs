using IBatisNetSelf.Common.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Cache
{
    /// <summary>
    ///  Hash value generator for cache keys
    /// </summary>
    public class CacheKey
    {
        private const int DEFAULT_MULTIPLYER = 37;
        private const int DEFAULT_HASHCODE = 17;

        private int multiplier = DEFAULT_MULTIPLYER;
        private int hashCode = DEFAULT_HASHCODE;
        private long checksum = long.MinValue;
        private int count = 0;
        private IList paramList = new ArrayList();

        /// <summary>
        /// Default constructor
        /// </summary>
        public CacheKey()
        {
            this.hashCode = DEFAULT_HASHCODE;
            this.multiplier = DEFAULT_MULTIPLYER;
            this.count = 0;
        }

        /// <summary>
        /// Constructor that supplies an initial hashcode
        /// </summary>
        /// <param name="initialNonZeroOddNumber">the hashcode to use</param>
        public CacheKey(int initialNonZeroOddNumber)
        {
            this.hashCode = initialNonZeroOddNumber;
            this.multiplier = DEFAULT_MULTIPLYER;
            this.count = 0;
        }

        /// <summary>
        /// Constructor that supplies an initial hashcode and multiplier
        /// </summary>
        /// <param name="initialNonZeroOddNumber">the hashcode to use</param>
        /// <param name="multiplierNonZeroOddNumber">the multiplier to use</param>
        public CacheKey(int initialNonZeroOddNumber, int multiplierNonZeroOddNumber)
        {
            this.hashCode = initialNonZeroOddNumber;
            this.multiplier = multiplierNonZeroOddNumber;
            this.count = 0;
        }

        /// <summary>
        /// Updates this object with new information based on an object
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>the cachekey</returns>
        public CacheKey Update(object obj)
        {
            int baseHashCode = HashCodeProvider.GetIdentityHashCode(obj);

            count++;
            checksum += baseHashCode;
            baseHashCode *= count;

            hashCode = multiplier * hashCode + baseHashCode;

            paramList.Add(obj);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (!(obj is CacheKey)) return false;

            CacheKey cacheKey = (CacheKey)obj;

            if (hashCode != cacheKey.hashCode) return false;
            if (checksum != cacheKey.checksum) return false;
            if (this.count != cacheKey.count) return false;

            int count = paramList.Count;
            for (int i = 0; i < count; i++)
            {
                object thisParam = paramList[i];
                object thatParam = cacheKey.paramList[i];
                if (thisParam == null)
                {
                    if (thatParam != null) return false;
                }
                else
                {
                    if (!thisParam.Equals(thatParam)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the HashCode for this CacheKey
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return hashCode;
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns>A string that give the CacheKey HashCode.</returns>
        public override string ToString()
        {
            return new StringBuilder().Append(hashCode).Append('|').Append(checksum).ToString();
        }

    }
}
