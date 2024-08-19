using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// A StringTokenizer java like object 
    /// </summary>
    public class StringTokenizer : IEnumerable
    {
        private static readonly string defaultDelim = " \t\n\r\f";
        string origin = string.Empty;
        string delimiters = string.Empty;
        bool returnDelimiters = false;

        /// <summary>
        /// Constructs a StringTokenizer on the specified String, using the
        /// default delimiter set (which is " \t\n\r\f").
        /// </summary>
        /// <param name="str">The input String</param>
        public StringTokenizer(string str)
        {
            this.origin = str;
            this.delimiters = defaultDelim;
            this.returnDelimiters = false;
        }


        /// <summary>
        /// Constructs a StringTokenizer on the specified String, 
        /// using the specified delimiter set.
        /// </summary>
        /// <param name="str">The input String</param>
        /// <param name="delimiters">The delimiter String</param>
        public StringTokenizer(string str, string delimiters)
        {
            this.origin = str;
            this.delimiters = delimiters;
            this.returnDelimiters = false;
        }


        /// <summary>
        /// Constructs a StringTokenizer on the specified String, 
        /// using the specified delimiter set.
        /// </summary>
        /// <param name="str">The input String</param>
        /// <param name="delimiters">The delimiter String</param>
        /// <param name="returnDelimiters">Returns delimiters as tokens or skip them</param>
        public StringTokenizer(string str, string delimiters, bool returnDelimiters)
        {
            this.origin = str;
            this.delimiters = delimiters;
            this.returnDelimiters = returnDelimiters;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new StringTokenizerEnumerator(this);
        }


        /// <summary>
        /// Returns the number of tokens in the String using
        /// the current deliminter set.  This is the number of times
        /// nextToken() can return before it will generate an exception.
        /// Use of this routine to count the number of tokens is faster
        /// than repeatedly calling nextToken() because the substrings
        /// are not constructed and returned for each token.
        /// </summary>
        public int TokenNumber
        {
            get
            {
                int _count = 0;
                int _currPos = 0;
                int _maxPosition = origin.Length;

                while (_currPos < _maxPosition)
                {
                    while (!returnDelimiters &&
                        (_currPos < _maxPosition) &&
                        (delimiters.IndexOf(origin[_currPos]) >= 0))
                    {
                        _currPos++;
                    }

                    if (_currPos >= _maxPosition)
                    {
                        break;
                    }

                    int start = _currPos;
                    while ((_currPos < _maxPosition) &&
                        (delimiters.IndexOf(origin[_currPos]) < 0))
                    {
                        _currPos++;
                    }
                    if (returnDelimiters && (start == _currPos) &&
                        (delimiters.IndexOf(origin[_currPos]) >= 0))
                    {
                        _currPos++;
                    }
                    _count++;
                }
                return _count;

            }

        }


        private class StringTokenizerEnumerator : IEnumerator
        {
            private StringTokenizer _stokenizer;
            private int _cursor = 0;
            private string _next = null;

            public StringTokenizerEnumerator(StringTokenizer stok)
            {
                _stokenizer = stok;
            }

            public bool MoveNext()
            {
                _next = GetNext();
                return _next != null;
            }

            public void Reset()
            {
                _cursor = 0;
            }

            public object Current
            {
                get
                {
                    return _next;
                }
            }

            private string GetNext()
            {
                char _c;
                bool _isDelim;

                if (_cursor >= _stokenizer.origin.Length)
                    return null;

                _c = _stokenizer.origin[_cursor];
                _isDelim = (_stokenizer.delimiters.IndexOf(_c) != -1);

                if (_isDelim)
                {
                    _cursor++;
                    if (_stokenizer.returnDelimiters)
                    {
                        return _c.ToString();
                    }
                    return GetNext();
                }

                int _nextDelimPos = _stokenizer.origin.IndexOfAny(_stokenizer.delimiters.ToCharArray(), _cursor);
                if (_nextDelimPos == -1)
                {
                    _nextDelimPos = _stokenizer.origin.Length;
                }

                string _nextToken = _stokenizer.origin.Substring(_cursor, _nextDelimPos - _cursor);
                _cursor = _nextDelimPos;
                return _nextToken;
            }

        }
    }
}
