using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.ParameterMapping
{
    /// <summary>
    /// A ParameterProperty Collection.
    /// </summary>
    public class ParameterPropertyCollection
    {
        private const int DEFAULT_CAPACITY = 4;
        private const int CAPACITY_MULTIPLIER = 2;
        private int count = 0;
        private ParameterProperty[] innerList = null;


        /// <summary>
        /// Read-only property describing how many elements are in the Collection.
        /// </summary>
        public int Count => this.count;

        /// <summary>
        /// Length of the collection
        /// </summary>
        public int Length=> innerList.Length;


        /// <summary>
        /// Sets or Gets the ParameterProperty at the given index.
        /// </summary>
        public ParameterProperty this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this.innerList[index];
            }
            set
            {
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                this.innerList[index] = value;
            }
        }


        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructs a ParameterProperty collection. The list is initially empty and has a capacity
        /// of zero. Upon adding the first element to the list the capacity is
        /// increased to 8, and then increased in multiples of two as required.
        /// </summary>
        public ParameterPropertyCollection()
        {
            this.innerList = new ParameterProperty[DEFAULT_CAPACITY];
            count = 0;
        }

        /// <summary>
        ///  Constructs a ParameterPropertyCollection with a given initial capacity. 
        ///  The list is initially empty, but will have room for the given number of elements
        ///  before any reallocations are required.
        /// </summary>
        /// <param name="capacity">The initial capacity of the list</param>
        public ParameterPropertyCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("Capacity", "The size of the list must be >0.");
            }
            this.innerList = new ParameterProperty[capacity];
        }

        #endregion


        /// <summary>
        /// Add an ParameterProperty
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Index</returns>
        public int Add(ParameterProperty value)
        {
            Resize(count + 1);
            int _index = count++;
            this.innerList[_index] = value;

            return _index;
        }


        /// <summary>
        /// Add a list of ParameterProperty to the collection
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(ParameterProperty[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                Add(value[i]);
            }
        }


        /// <summary>
        /// Add a list of ParameterProperty to the collection
        /// </summary>
        /// <param name="value"></param>
        public void AddRange(ParameterPropertyCollection value)
        {
            for (int i = 0; i < value.Count; i++)
            {
                Add(value[i]);
            }
        }


        /// <summary>
        /// Indicate if a ParameterProperty is in the collection
        /// </summary>
        /// <param name="value">A ParameterProperty</param>
        /// <returns>True fi is in</returns>
        public bool Contains(ParameterProperty value)
        {
            for (int i = 0; i < count; i++)
            {
                if (innerList[i].PropertyName == value.PropertyName)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Insert a ParameterProperty in the collection.
        /// </summary>
        /// <param name="index">Index where to insert.</param>
        /// <param name="value">A ParameterProperty</param>
        public void Insert(int index, ParameterProperty value)
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
        /// Remove a ParameterProperty of the collection.
        /// </summary>
        public void Remove(ParameterProperty value)
        {
            for (int i = 0; i < count; i++)
            {
                if (innerList[i].PropertyName == value.PropertyName)
                {
                    RemoveAt(i);
                    return;
                }
            }

        }

        /// <summary>
        /// Removes a ParameterProperty at the given index. The size of the list is
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
            int _oldSize = this.innerList.Length;

            if (minSize > _oldSize)
            {
                ParameterProperty[] _oldEntries = this.innerList;
                int _newSize = _oldEntries.Length * CAPACITY_MULTIPLIER;

                if (_newSize < minSize)
                {
                    _newSize = minSize;
                }
                this.innerList = new ParameterProperty[_newSize];
                Array.Copy(_oldEntries, 0, innerList, 0, count);
            }
        }
    }
}
