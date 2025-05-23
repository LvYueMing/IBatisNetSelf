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
using IBatisNetSelf.DataMapper.TypeHandlers;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    /// 构建一个 <see cref="ResultPropertyCollection"/> 的动态实例
    /// </summary>
    public sealed class ReaderAutoMapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 为 <see cref="AutoResultMap"/> 构建一个 <see cref="ResultPropertyCollection"/>。
        /// </summary>
        /// <param name="dataExchangeFactory">用于处理类型访问器与类型处理器的工厂。</param>
        /// <param name="reader">数据库查询结果的读取器。</param>
        /// <param name="resultObject">用于映射结果的目标对象。</param>
        public static ResultPropertyCollection Build(DataExchangeFactory dataExchangeFactory, IDataReader reader, ref object resultObject)
        {
            // 获取目标对象的类型（resultObject）
            Type targetType = resultObject.GetType();
            // 初始化结果属性集合
            ResultPropertyCollection properties = new ResultPropertyCollection();

            try
            {
                // 获取可写属性名并构造不区分大小写的属性映射表
                ReflectionInfo reflectionInfo = ReflectionInfo.GetInstance(targetType);
                // 获取所有可写入的属性名称
                string[] _propertyNames = reflectionInfo.GetWriteablePropertyNames();

                // 构建属性映射表（列名 -> 属性Setter,列名不区分大小写
                Dictionary<string, ISetAccessor> propertyMap = new Dictionary<string, ISetAccessor>(StringComparer.OrdinalIgnoreCase);
                ISetAccessorFactory _setAccessorFactory = dataExchangeFactory.AccessorFactory.SetAccessorFactory;
                foreach (string _name in _propertyNames)
                {
                    ISetAccessor setAccessor = _setAccessorFactory.CreateSetAccessor(targetType, _name);
                    propertyMap.Add(_name, setAccessor);
                }

                //从 reader 中获取所有列的 schema 信息（结构定义），并借助 PropertyInfo[] 构建一个 resultMap
                DataTable dataColumn = reader.GetSchemaTable();

                for (int i = 0; i < dataColumn.Rows.Count; i++)
                {
                    string _columnName = dataColumn.Rows[i][0].ToString();

                    // 如果列名与属性名称不匹配，则跳过
                    ISetAccessor _matchedSetAccessor = null;
                    if (!propertyMap.TryGetValue(_columnName, out _matchedSetAccessor))
                        continue;

                    // 初始化一个新的结果属性
                    ResultProperty _property = new ResultProperty();
                    _property.ColumnName = _columnName;
                    _property.ColumnIndex = i;

                    //TODO: 应该不会到这里来
                    // 如果是 Hashtable 类型，则直接将列名作为属性名加入集合
                    //if (resultObject is Hashtable)
                    //{
                    //    _property.PropertyName = columnName;
                    //    properties.Add(_property);
                    //}

                    Type _propertyType = null;
                    if (_matchedSetAccessor == null)
                    {
                        // 如果找不到匹配的 SetAccessor，尝试使用反射探测属性类型
                        try
                        {
                            _propertyType = ObjectProbe.GetMemberTypeForSetter(resultObject, _columnName);
                        }
                        catch
                        {
                            _logger.Error("The column [" + _columnName + "] could not be auto mapped to a property on [" + resultObject.ToString() + "]");
                        }
                    }
                    else
                    {
                        // 有匹配的 SetAccessor，则直接获取其类型
                        _propertyType = _matchedSetAccessor.MemberType;
                    }

                    // 如果能获取到属性类型或访问器，则继续处理映射关系
                    if (_propertyType != null || _matchedSetAccessor != null)
                    {
                        // 设置属性名（优先用 SetAccessor 的名称）
                        _property.PropertyName = (_matchedSetAccessor != null ? _matchedSetAccessor.Name : _columnName);
                        if (_matchedSetAccessor != null)
                        {
                            // 使用 SetAccessor 初始化 TypeHandler
                            _property.Initialize(dataExchangeFactory.TypeHandlerFactory, _matchedSetAccessor);
                        }
                        else
                        {
                            // 没有访问器，则手动获取 TypeHandler
                            _property.TypeHandler = dataExchangeFactory.TypeHandlerFactory.GetTypeHandler(_propertyType);
                        }

                        _property.PropertyStrategy = PropertyStrategyFactory.Get(_property);
                        properties.Add(_property);
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
