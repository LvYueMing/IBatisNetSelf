using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// <see cref="IPropertyStrategy"/> implementation when a 'select' attribute exists
    /// on a <see cref="ResultProperty"/>
    /// </summary>
    public class SelectStrategy : IPropertyStrategy
    {
        private IPropertyStrategy _selectStrategy = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="SelectStrategy"/> class.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="selectArrayStrategy">The select array strategy.</param>
        /// <param name="selectGenericListStrategy">The select generic list strategy.</param>
        /// <param name="selectListStrategy">The select list strategy.</param>
        /// <param name="selectObjectStrategy">The select object strategy.</param>
        public SelectStrategy(ResultProperty mapping,
            IPropertyStrategy selectArrayStrategy,
            IPropertyStrategy selectGenericListStrategy,
            IPropertyStrategy selectListStrategy,
            IPropertyStrategy selectObjectStrategy)
        {
            // Collection object or .NET object			
            if (mapping.SetAccessor.MemberType.BaseType == typeof(Array))
            {
                _selectStrategy = selectArrayStrategy;
            }
            // Check if the object to Map implement 'IList' or is IList type
            // If yes the ResultProperty is map to a IList object
            else if (typeof(IList).IsAssignableFrom(mapping.SetAccessor.MemberType))
            {
                _selectStrategy = selectListStrategy;
            }

            else // The ResultProperty is map to a .Net object
            {
                _selectStrategy = selectObjectStrategy;
            }
        }

        #region IPropertyStrategy members

        ///<summary>
        /// Sets value of the specified <see cref="ResultProperty"/> on the target object
        /// when a 'select' attribute exists
        /// on the <see cref="ResultProperty"/> are empties.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The ResultProperty.</param>
        /// <param name="target">The target.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="selectKeys">The keys</param>
        public void Set(RequestScope request, IResultMap resultMap,
            ResultProperty mapping, ref object target, IDataReader reader, object selectKeys)
        {
            string paramString = mapping.ColumnName;
            object keys = null;
            bool wasNull = false;

            #region Finds the select keys.
            if (paramString.IndexOf(',') > 0 || paramString.IndexOf('=') > 0) // composite parameters key
            {
                IDictionary keyMap = new Hashtable();
                keys = keyMap;
                // define which character is seperating fields
                char[] splitter = { '=', ',' };

                string[] paramTab = paramString.Split(splitter);
                if (paramTab.Length % 2 != 0)
                {
                    throw new DataMapperException("Invalid composite key string format in '" + mapping.PropertyName + ". It must be: property1=column1,property2=column2,...");
                }
                IEnumerator enumerator = paramTab.GetEnumerator();
                while (!wasNull && enumerator.MoveNext())
                {
                    string hashKey = ((string)enumerator.Current).Trim();
                    if (paramString.Contains("="))// old 1.x style multiple params
                    {
                        enumerator.MoveNext();
                    }
                    object hashValue = reader.GetValue(reader.GetOrdinal(((string)enumerator.Current).Trim()));

                    keyMap.Add(hashKey, hashValue);
                    wasNull = (hashValue == DBNull.Value);
                }
            }
            else // single parameter key
            {
                keys = reader.GetValue(reader.GetOrdinal(paramString));
                wasNull = reader.IsDBNull(reader.GetOrdinal(paramString));
            }
            #endregion

            if (wasNull)
            {
                // set the value of an object property to null
                mapping.SetAccessor.Set(target, null);
            }
            else // Collection object or .Net object
            {
                _selectStrategy.Set(request, resultMap, mapping, ref target, reader, keys);
            }
        }

        /// <summary>
        /// Gets the value of the specified <see cref="ResultProperty"/> that must be set on the target object.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="resultMap">The result map.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="target">The target object</param>
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            throw new NotSupportedException("Get method on ResultMapStrategy is not supported");
        }
        #endregion
    }
}
