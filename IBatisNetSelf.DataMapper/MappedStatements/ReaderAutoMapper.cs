using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// Build a dynamic instance of a <see cref="ResultPropertyCollection"/>
    /// </summary>
    public sealed class ReaderAutoMapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Builds a <see cref="ResultPropertyCollection"/> for an <see cref="AutoResultMap"/>.
        /// </summary>
        /// <param name="dataExchangeFactory">The data exchange factory.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="resultObject">The result object.</param>
        public static ResultPropertyCollection Build(DataExchangeFactory dataExchangeFactory,
                                IDataReader reader,
                                ref object resultObject)
        {
            Type targetType = resultObject.GetType();
            ResultPropertyCollection properties = new ResultPropertyCollection();

            try
            {
                // Get all PropertyInfo from the resultObject properties
                ReflectionInfo reflectionInfo = ReflectionInfo.GetInstance(targetType);
                string[] membersName = reflectionInfo.GetWriteableMemberNames();

                Hashtable propertyMap = new Hashtable();
                int length = membersName.Length;
                for (int i = 0; i < length; i++)
                {
                    ISetAccessorFactory setAccessorFactory = dataExchangeFactory.AccessorFactory.SetAccessorFactory;
                    ISetAccessor setAccessor = setAccessorFactory.CreateSetAccessor(targetType, membersName[i]);
                    propertyMap.Add(membersName[i], setAccessor);
                }

                // Get all column Name from the reader
                // and build a resultMap from with the help of the PropertyInfo[].
                DataTable dataColumn = reader.GetSchemaTable();
                int count = dataColumn.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    string columnName = dataColumn.Rows[i][0].ToString();
                    ISetAccessor matchedSetAccessor = propertyMap[columnName] as ISetAccessor;

                    ResultProperty property = new ResultProperty();
                    property.ColumnName = columnName;
                    property.ColumnIndex = i;

                    if (resultObject is Hashtable)
                    {
                        property.PropertyName = columnName;
                        properties.Add(property);
                    }

                    Type propertyType = null;

                    if (matchedSetAccessor == null)
                    {
                        try
                        {
                            propertyType = ObjectProbe.GetMemberTypeForSetter(resultObject, columnName);
                        }
                        catch
                        {
                            _logger.Error("The column [" + columnName + "] could not be auto mapped to a property on [" + resultObject.ToString() + "]");
                        }
                    }
                    else
                    {
                        propertyType = matchedSetAccessor.MemberType;
                    }

                    if (propertyType != null || matchedSetAccessor != null)
                    {
                        property.PropertyName = (matchedSetAccessor != null ? matchedSetAccessor.Name : columnName);
                        if (matchedSetAccessor != null)
                        {
                            property.Initialize(dataExchangeFactory.TypeHandlerFactory, matchedSetAccessor);
                        }
                        else
                        {
                            property.TypeHandler = dataExchangeFactory.TypeHandlerFactory.GetTypeHandler(propertyType);
                        }

                        property.PropertyStrategy = PropertyStrategyFactory.Get(property);
                        properties.Add(property);
                    }
                }
            }
            catch (Exception e)
            {
                throw new DataMapperException("Error automapping columns. Cause: " + e.Message, e);
            }

            return properties;
        }

    }
}
