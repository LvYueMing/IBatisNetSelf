using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// Collection of <see cref="IResultMap"/>
    /// </summary>
    public class ResultMapCollection
    {
        private const int DEFAULT_CAPACITY = 2;
        private const int CAPACITY_MULTIPLIER = 2;
        private int count = 0;
        private IResultMap[] innerList = null;


        /// <summary>
        /// Read-only property describing how many elements are in the Collection.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Length of the collection
        /// </summary>
        public int Length=> innerList.Length;


        /// <summary>
        /// Constructs a ResultMapCollection. The list is initially empty and has a capacity
        /// of zero. Upon adding the first element to the list the capacity is
        /// increased to 8, and then increased in multiples of two as required.
        /// </summary>
        public ResultMapCollection()
        {
            this.Clear();
        }

        /// <summary>
        ///  Constructs a ResultMapCollection with a given initial capacity. 
        ///  The list is initially empty, but will have room for the given number of elements
        ///  before any reallocations are required.
        /// </summary>
        /// <param name="capacity">The initial capacity of the list</param>
        public ResultMapCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity", "The size of the list must be >0.");
            }
            innerList = new IResultMap[capacity];
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            innerList = new IResultMap[DEFAULT_CAPACITY];
            count = 0;
        }


        /// <summary>
        /// Sets or Gets the ResultMap at the given index.
        /// </summary>
        public IResultMap this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return innerList[index];
            }
            set
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                innerList[index] = value;
            }
        }


        /// <summary>
        /// Add an ResultMap
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Index</returns>
        public int Add(IResultMap value)
        {
            Resize(count + 1);
            int index = count++;
            innerList[index] = value;

            return index;
        }


        /// <summary>
        /// Add a list of ResultMap to the collection
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(IResultMap[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Add(value[i]);
            }
        }


        /// <summary>
        /// Add a list of ResultMap to the collection
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(ResultMapCollection value)
        {
            for (int i = 0; i < value.Count; i++)
            {
                Add(value[i]);
            }
        }


        /// <summary>
        /// Indicate if a ResultMap is in the collection
        /// </summary>
        /// <param name="value">A ResultMap</param>
        /// <returns>True fi is in</returns>
        public bool Contains(IResultMap value)
        {
            for (int i = 0; i < count; i++)
            {
                if (innerList[i].Id == value.Id)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Insert a ResultMap in the collection.
        /// </summary>
        /// <param name="index">Index where to insert.</param>
        /// <param name="value">A ResultMap</param>
        public void Insert(int index, IResultMap value)
        {
            if (index < 0 || index > count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Resize(count + 1);
            Array.Copy(innerList, index, innerList, index + 1, count - index);
            innerList[index] = value;
            count++;
        }


        /// <summary>
        /// Remove a ResultMap of the collection.
        /// </summary>
        public void Remove(IResultMap value)
        {
            for (int i = 0; i < count; i++)
            {
                if (innerList[i].Id == value.Id)
                {
                    RemoveAt(i);
                    return;
                }
            }

        }

        /// <summary>
        /// Removes a ResultMap at the given index. The size of the list is
        /// decreased by one.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int remaining = count - index - 1;

            if (remaining > 0)
            {
                Array.Copy(innerList, index + 1, innerList, index, remaining);
            }

            count--;
            innerList[count] = null;
        }

        /// <summary>
        /// Ensures that the capacity of this collection is at least the given minimum
        /// value. If the currect capacity of the list is less than min, the
        /// capacity is increased to twice the current capacity.
        /// </summary>
        /// <param name="minSize"></param>
        private void Resize(int minSize)
        {
            int oldSize = innerList.Length;

            if (minSize > oldSize)
            {
                IResultMap[] oldEntries = innerList;
                int newSize = oldEntries.Length * CAPACITY_MULTIPLIER;

                if (newSize < minSize)
                {
                    newSize = minSize;
                }
                innerList = new IResultMap[newSize];
                Array.Copy(oldEntries, 0, innerList, 0, count);
            }
        }
    }
}
