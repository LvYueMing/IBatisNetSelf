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
        string originStr = string.Empty;
        string delimiters = string.Empty;
        bool returnDelimiters = false;

        /// <summary>
        /// Constructs a StringTokenizer on the specified String, using the
        /// default delimiter set (which is " \t\n\r\f").
        /// </summary>
        /// <param name="str">The input String</param>
        public StringTokenizer(string str)
        {
            this.originStr = str;
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
            this.originStr = str;
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
            this.originStr = str;
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
                int _maxPosition = originStr.Length;

                while (_currPos < _maxPosition)
                {
                    while (!returnDelimiters &&
                        (_currPos < _maxPosition) &&
                        (delimiters.IndexOf(originStr[_currPos]) >= 0))
                    {
                        _currPos++;
                    }

                    if (_currPos >= _maxPosition)
                    {
                        break;
                    }

                    int start = _currPos;
                    while ((_currPos < _maxPosition) &&
                        (delimiters.IndexOf(originStr[_currPos]) < 0))
                    {
                        _currPos++;
                    }
                    if (returnDelimiters && (start == _currPos) &&
                        (delimiters.IndexOf(originStr[_currPos]) >= 0))
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
            private StringTokenizer strTokenizer;
            private int cursor = 0;
            private string next = null;

            public StringTokenizerEnumerator(StringTokenizer strToken)
            {
                strTokenizer = strToken;
            }

            public bool MoveNext()
            {
                next = GetNext();
                return next != null;
            }

            public void Reset()
            {
                cursor = 0;
            }

            public object Current
            {
                get
                {
                    return next;
                }
            }

            private string GetNext()
            {
                char _char;
                bool _isDelim;

                if (cursor >= strTokenizer.originStr.Length)
                    return null;

                _char = strTokenizer.originStr[cursor];
                _isDelim = (strTokenizer.delimiters.IndexOf(_char) != -1);

                if (_isDelim)
                {
                    cursor++;
                    if (strTokenizer.returnDelimiters)
                    {
                        return _char.ToString();
                    }
                    return GetNext();
                }

                int _nextDelimPos = strTokenizer.originStr.IndexOfAny(strTokenizer.delimiters.ToCharArray(), cursor);
                if (_nextDelimPos == -1)
                {
                    _nextDelimPos = strTokenizer.originStr.Length;
                }

                string _nextToken = strTokenizer.originStr.Substring(cursor, _nextDelimPos - cursor);
                cursor = _nextDelimPos;
                return _nextToken;
            }

        }
    }
}
