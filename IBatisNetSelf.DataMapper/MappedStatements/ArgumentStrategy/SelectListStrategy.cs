using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Commands;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ArgumentStrategy
{
    /// <summary>
    /// <see cref="IArgumentStrategy"/> implementation when a 'select' attribute exists
    /// on a <see cref="IList"/> <see cref="ArgumentProperty"/>
    /// </summary>
    public sealed class SelectListStrategy : IArgumentStrategy
    {
        #region IArgumentStrategy Members

        /// <summary>
        /// Gets the value of an argument constructor.
        /// </summary>
        /// <param name="request">The current <see cref="RequestScope"/>.</param>
        /// <param name="mapping">The <see cref="ResultProperty"/> with the argument infos.</param>
        /// <param name="reader">The current <see cref="IDataReader"/>.</param>
        /// <param name="keys">The keys</param>
        /// <returns>The paremeter value.</returns>
        public object GetValue(RequestScope request, ResultProperty mapping,
                               ref IDataReader reader, object keys)
        {
            // Get the select statement
            IMappedStatement selectStatement = request.MappedStatement.SqlMap.GetMappedStatement(mapping.Select);

            if (mapping.MemberType == typeof(IList))
            {
                reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
                return selectStatement.ExecuteQueryForList(request.Session, keys);
            }
            else // Strongly typed List
            {
                reader = DataReaderTransformer.Transform(reader, request.Session.DataSource.DbProvider);
                IFactory factory = request.DataExchangeFactory.ObjectFactory.CreateFactory(mapping.MemberType, Type.EmptyTypes);
                object values = factory.CreateInstance(null);
                selectStatement.ExecuteQueryForList(request.Session, keys, (IList)values);
                return values;
            }
        }

        #endregion
    }
}
