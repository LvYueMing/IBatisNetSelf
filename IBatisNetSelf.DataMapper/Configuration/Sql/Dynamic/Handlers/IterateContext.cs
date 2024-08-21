using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Summary description for IterateContext.
    /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp01212002.asp
    /// http://www.microsoft.com/mspress/books/sampchap/6173.asp
    /// http://www.dur.ac.uk/barry.cornelius/java/a.taste.of.csharp/onefile/
    /// </summary>
    public sealed class IterateContext : IEnumerator
    {
        #region Fields
        private ICollection _collection;
        private ArrayList _items = new ArrayList();
        int _index = -1;
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection"></param>
        public IterateContext(object collection)
        {
            if (collection is ICollection)
            {
                _collection = (ICollection)collection;
            }
            else if (collection.GetType().IsArray)
            {
                object[] array = (object[])collection;
                ArrayList list = new ArrayList();
                int lenght = array.Length;
                for (int i = 0; i < lenght; i++)
                {
                    list.Add(array[i]);
                }
                _collection = (ICollection)list;
            }
            else
            {
                throw new DataMapperException("ParameterObject or property was not a Collection, Array or Iterator.");
            }

            IEnumerable enumerable = (IEnumerable)collection;
            IEnumerator enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext() != false)
            {
                _items.Add(enumerator.Current);
            }
            IDisposable disposable = enumerator as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            _index = -1;
        }

        /// <summary>
        /// Sets the enumerator to its initial position,
        ///  which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element;
        /// False if the enumerator has passed the end of the collection.
        ///</returns>
        public bool MoveNext()
        {
            _index++;
            if (_index == _items.Count)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public object Current
        {
            get
            {
                return _items[_index];
            }
        }

        /// <summary>
        /// Gets the index of the current element in the collection.
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// Return true if the current element is the first.
        /// </summary>
        public bool IsFirst
        {
            get
            {
                return (_index == 0);
            }
        }

        /// <summary>
        ///  Return true if the current element is the last.
        /// </summary>
        public bool IsLast
        {
            get
            {
                return (_index == (_items.Count - 1));
            }
        }

        /// <summary>
        /// Removes from the underlying collection the last element returned by the iterator. 
        /// </summary>
        public void Remove()
        {
            if (_collection is IList)
            {
                ((IList)_collection).Remove(this.Current);
            }
            else if (_collection is IDictionary)
            {
                ((IDictionary)_collection).Remove(this.Current);
            }
        }

        /// <summary>
        /// Returns true if the iteration has more elements. (In other words, returns true 
        /// if next would return an element rather than throwing an exception.) 
        /// </summary>
        public bool HasNext
        {
            get
            {
                if ((_index >= -1) && (_index < (_items.Count - 1)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
