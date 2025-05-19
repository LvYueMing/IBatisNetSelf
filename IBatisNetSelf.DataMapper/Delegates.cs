using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// A delegate called once per row in the QueryWithRowDelegate method
    /// </summary>
    /// <param name="obj">The object currently being processed.</param>
    /// <param name="parameterObject">The optional parameter object passed into the QueryWithRowDelegate method.</param>
    /// <param name="list">The IList that will be returned to the caller.</param>
    public delegate void RowDelegate(object obj, object parameterObject, IList list);


    /// <summary>
    /// A delegate called once per row in the QueryForMapWithRowDelegate method
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="parameterObject">The optional parameter object passed into the QueryForMapWithRowDelegate method.</param>
    /// <param name="dictionary">The IDictionary that will be returned to the caller.</param>
    public delegate void DictionaryRowDelegate(object key, object value, object parameterObject, IDictionary dictionary);
}
