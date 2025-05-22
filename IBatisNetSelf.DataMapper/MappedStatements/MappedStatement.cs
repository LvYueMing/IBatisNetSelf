using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy;
using IBatisNetSelf.DataMapper.Commands;
using IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy;

namespace IBatisNetSelf.DataMapper.MappedStatements
{
    /// <summary>
    ///  MappedStatement 类，表示一个映射的 SQL 语句及其执行策略。
    /// </summary>
    public class MappedStatement : IMappedStatement
    {
        /// <summary>
        /// 执行查询时触发的事件
        /// </summary>
        public event ExecuteEventHandler Execute;

        #region Fields

        // 常量：表示不限制最大返回结果数
        internal const int NO_MAXIMUM_RESULTS = -1;
        // 常量：表示不跳过任何结果
        internal const int NO_SKIPPED_RESULTS = -1;

        // SQL 语句对象（配置文件中定义的 <select>、<insert> 等）
        private IStatement statement = null;

        // 当前语句所在的 SqlMapper 实例（即整个映射上下文）
        private ISqlMapper sqlMap = null;

        // 命令准备对象：构造 IDbCommand 及参数列表
        private IPreparedCommand preparedCommand = null;

        // 查询结果处理策略，例如映射为对象、集合等
        private IResultStrategy resultStrategy = null;

        #endregion

        #region Properties（属性）

        // 返回准备好的命令对象
        public IPreparedCommand PreparedCommand => this.preparedCommand;

        // 获取语句的唯一标识 ID（通常是配置文件中指定的 ID）
        public string Id => this.statement.Id;

        // 获取语句定义对象（包含 SQL、参数映射、结果映射等）
        public IStatement Statement => this.statement;

        // 获取 SqlMap 对象（数据库连接信息、类型处理器等）
        public ISqlMapper SqlMap => this.sqlMap;

        #endregion

        #region Constructor (s) / Destructor

        /// <summary>
        /// 构造函数：初始化 MappedStatement 实例
        /// </summary>
        /// <param name="sqlMap">所属的 SqlMap 映射器</param>
        /// <param name="statement">SQL 语句配置</param>
        internal MappedStatement(ISqlMapper sqlMap, IStatement statement)
        {
            this.sqlMap = sqlMap;
            this.statement = statement;
            //创建默认的DefaultPreparedCommand:IPreparedCommand
            this.preparedCommand = PreparedCommandFactory.GetPreparedCommand(false);
            // 获取结果策略处理器
            this.resultStrategy = ResultStrategyFactory.Get(this.statement);
        }
        #endregion


        #region Output Parameter Handling

        /// <summary>
        /// 提取输出参数并设置到结果对象上（如使用存储过程或指定 ParameterMap 的 output 参数时）
        /// </summary>
        /// <param name="aRequest">请求作用域，封装了语句执行的上下文信息</param>
        /// <param name="aSession">当前 SQL 会话</param>
        /// <param name="aCommand">执行用的 SQL 命令对象</param>
        /// <param name="aResult">结果对象，用于写回输出参数</param>
        private void RetrieveOutputParameters(RequestScope aRequest, ISqlMapSession aSession, IDbCommand aCommand, object aResult)
        {
            // 如果定义了参数映射（ParameterMap）
            if (aRequest.ParameterMap != null)
            {
                // 获取映射中所有参数属性的数量,遍历每个参数属性
                int _count = aRequest.ParameterMap.PropertiesList.Count;
                for (int i = 0; i < _count; i++)
                {
                    // 获取第 i 个参数映射配置
                    ParameterProperty _mapping = aRequest.ParameterMap.GetProperty(i);

                    // 只处理 Output 或 InputOutput 类型的参数
                    if (_mapping.Direction == ParameterDirection.Output ||
                        _mapping.Direction == ParameterDirection.InputOutput)
                    {
                        string _parameterName = string.Empty;
                        // 判断是否需要为参数名加前缀（如 @、:）
                        if (aSession.DataSource.DbProvider.UseParameterPrefixInParameter == false)
                        {
                            // 不加前缀，直接用列名
                            _parameterName = _mapping.ColumnName;
                        }
                        else
                        {
                            // 加前缀（如 @ColumnName）
                            _parameterName = aSession.DataSource.DbProvider.ParameterPrefix +_mapping.ColumnName;
                        }

                        // 如果未指定类型处理器，则动态确定 TypeHandler
                        if (_mapping.TypeHandler == null)
                        {
                            lock (_mapping)
                            {
                                if (_mapping.TypeHandler == null)
                                {
                                    // 通过反射获取该属性在结果对象中的 .NET 类型
                                    Type _propertyType = ObjectProbe.GetMemberTypeForGetter(aResult, _mapping.PropertyName);

                                    // 根据类型获取对应的 TypeHandler
                                    _mapping.TypeHandler = aRequest.DataExchangeFactory.TypeHandlerFactory.GetTypeHandler(_propertyType);
                                }
                            }
                        }

                        // 获取执行完命令后的参数值
                        IDataParameter _dataParameter = (IDataParameter)aCommand.Parameters[_parameterName];
                        object _dbValue = _dataParameter.Value;

                        object _value = null;

                        // 判断数据库返回值是否为 null（即 DBNull）
                        bool _isNull = (_dbValue == DBNull.Value);
                        if (_isNull)
                        {
                            // 如果指定了 null 值的替代表示，则使用该值
                            if (_mapping.HasNullValue)
                            {
                                _value = _mapping.TypeHandler.ValueOf(_mapping.GetAccessor.MemberType, _mapping.NullValue);
                            }
                            else
                            {
                                // 否则使用 TypeHandler 自带的 null 处理方式
                                _value = _mapping.TypeHandler.NullValue;
                            }
                        }
                        else
                        {
                            // 从数据库类型转换为 .NET 类型（调用 TypeHandler）
                            _value = _mapping.TypeHandler.GetDataBaseValue(_dataParameter.Value, aResult.GetType());
                        }

                        // 如果提取到了非空结果，则标记为“找到数据”
                        aRequest.IsRowDataFound = aRequest.IsRowDataFound || (_value != null);

                        // 将提取的值设置到结果对象中（通过反射设置属性值）
                        aRequest.ParameterMap.SetOutputParameter(ref aResult, _mapping, _value);
                    }
                }
            }
        }

        #endregion


        #region ExecuteForObject

        /// <summary>
        /// 执行一条 SQL 查询语句，并返回一个单行结果对象（默认使用新的结果对象）
        /// </summary>
        /// <param name="session">执行语句所使用的 SqlMap 会话</param>
        /// <param name="parameterObject">作为 SQL 参数的对象</param>
        /// <returns>查询到的结果对象</returns>
        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject)
        {
            // 实际调用带有 resultObject 参数的重载，传入 null 表示不传入现成对象
            return ExecuteQueryForObject(session, parameterObject, null);
        }


        /// <summary>
        /// 执行一条 SQL 查询语句，并返回一个单行结果对象，可以传入用于接收结果的现成对象
        /// </summary>
        /// <param name="session">执行语句所使用的 SqlMap 会话</param>
        /// <param name="parameterObject">作为 SQL 参数的对象</param>
        /// <param name="resultObject">用于接收查询结果的对象实例</param>
        /// <returns>查询到的结果对象</returns>
        public virtual object ExecuteQueryForObject(ISqlMapSession session, object parameterObject, object resultObject)
        {
            object _obj = null;
            // 构造 RequestScope（包含 SQL 文本、映射、参数等执行上下文）
            RequestScope _request = statement.Sql.GetRequestScope(this, parameterObject, session);
            // 创建 IDbCommand 并准备参数列表（使用参数对象赋值）
            this.preparedCommand.Create(_request, session, this.Statement, parameterObject);
            // 执行查询并返回结果
            _obj = RunQueryForObject(_request, session, parameterObject, resultObject);

            return _obj;
        }


        /// <summary>
        /// 实际执行查询操作，并返回查询结果
        /// </summary>
        /// <param name="request">请求作用域，封装语句、参数、命令等</param>
        /// <param name="session">当前 SQL 会话</param>
        /// <param name="parameterObject">传入的参数对象</param>
        /// <param name="resultObject">用于接收结果的已有对象</param>
        /// <returns>查询到的结果对象</returns>
        internal object RunQueryForObject(RequestScope request, ISqlMapSession session, object parameterObject, object resultObject)
        {
            // 初始化结果为传入的 resultObject（若为 null 则最终由 resultStrategy 创建）
            object _result = resultObject;

            // 获取 SQL 命令对象，确保在 using 块中自动释放资源
            using (IDbCommand command = request.IDbCommand)
            {
                // 执行命令，返回只读数据流读取器
                IDataReader _reader = command.ExecuteReader();
                try
                {
                    // 遍历查询结果（只取第一行）
                    while (_reader.Read())
                    {
                        // 使用结果策略进行数据映射（如映射为实体类对象）
                        object _obj = this.resultStrategy.Process(request, ref _reader, resultObject);
                        // 判断是否跳过此行（通常用于 discriminator 分支跳过）
                        if (_obj != BaseStrategy.SKIP)
                        {
                            _result = _obj;
                        }
                    }
                }
                catch
                {
                    // 出现异常直接抛出（调用方处理）
                    throw;
                }
                finally
                {
                    // 无论成功与否都关闭 reader，释放资源
                    _reader.Close();
                    _reader.Dispose();
                }

                // 执行后置的延迟绑定操作（如嵌套 select 映射）
                ExecutePostSelect(request);

                #region remark
                // 若使用 OleDbProvider，则必须先关闭 reader 才能读取 output 参数
                #endregion

                // 提取 output 或 input-output 参数并赋值给参数对象
                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            // 触发 Execute 事件（可用于日志记录或扩展）
            RaiseExecuteEvent();

            return _result;
        }

        #endregion


        #region ExecuteQueryForList


        /// <summary>
        /// 执行 SQL 并返回所有结果行（List 列表形式）。
        /// 实际上等价于调用 ExecuteQueryForList(session, parameterObject, NO_SKIPPED_RESULTS, NO_MAXIMUM_RESULTS)。
        /// </summary>
        /// <param name="aSession">执行 SQL 的当前会话对象（ISqlMapSession）</param>
        /// <param name="aParameterObject">用于填充 SQL 参数的输入对象</param>
        /// <returns>结果列表（IList 类型）</returns>
        public virtual IList ExecuteQueryForList(ISqlMapSession aSession, object aParameterObject)
        {
            // 通过 SQL 映射配置生成请求作用域对象（RequestScope）
            // 包含：语句内容、参数映射、结果映射等上下文信息
            RequestScope _request = this.statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            // 使用请求作用域、SQL 映射、参数对象创建 IDbCommand 命令对象
            // 1.创建命令对象IDbCommand，_request.IDbCommand=aSession.CreateCommand(aStatement.CommandType)
            // 2.命令对象执行语句CommandText赋值，_request.IDbCommand.CommandText = _request.PreparedStatement.PreparedSql
            // 3.给命令对象创建参数列表（aRequest.IDbCommand.Parameters），并赋值（从aParameterObject获取参数对应的值）
            // 4.一切准备就绪，可执行命令获取数据
            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            // 调用实际执行逻辑方法，获取查询结果（默认不分页、不传递 RowDelegate）
            return RunQueryForList(_request, aSession, aParameterObject, null, null);
        }

        /// <summary>
        /// 执行一条 SQL 查询语句，并允许用户通过 RowDelegate 委托对每一行数据自定义处理。
        /// </summary>
        /// <param name="session">执行 SQL 的当前会话（ISqlMapSession）</param>
        /// <param name="parameterObject">用于填充 SQL 参数的输入对象</param>
        /// <param name="rowDelegate">用于处理每一行数据的委托方法（RowDelegate 委托）</param>
        /// <returns>结果集合（IList），由 RowDelegate 控制填充逻辑</returns>
        public virtual IList ExecuteQueryForRowDelegate(ISqlMapSession session, object parameterObject, RowDelegate rowDelegate)
        {
            // 创建执行上下文 RequestScope，包含 SQL、参数映射、结果映射等信息
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            // 使用上下文信息和参数对象准备 IDbCommand 对象
            // 包括生成 SQL 命令、绑定参数值等
            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            if (rowDelegate == null)
            {
                throw new DataMapperException("A null RowDelegate was passed to QueryForRowDelegate.");
            }
            // 执行查询，并交给 rowDelegate 对每一行处理，最终返回结果集合
            return RunQueryForList(request, session, parameterObject, null, rowDelegate);
        }

        /// <summary>
        /// 执行 SQL 查询，并在每一行处理时，通过 DictionaryRowDelegate 委托将结果写入到 Map（字典）中。
        /// </summary>
        /// <param name="session">当前用于执行 SQL 的会话对象</param>
        /// <param name="parameterObject">用于填充 SQL 参数的输入对象</param>
        /// <param name="keyProperty">从每个结果对象中提取作为字典 key 的属性名</param>
        /// <param name="valueProperty">从每个结果对象中提取作为字典 value 的属性名，若为 null 表示整个对象作为 value</param>
        /// <param name="rowDelegate">自定义行处理逻辑的委托方法</param>
        /// <returns>一个字典，key 为 keyProperty 指定的属性值，value 为 valueProperty 或整个对象</returns>
        /// <exception cref="DataMapperException">如果 rowDelegate 为 null 或数据库操作出错则抛出异常</exception>
        public virtual IDictionary ExecuteQueryForMapWithRowDelegate(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            // 构造执行上下文，封装语句定义、参数映射、结果映射等
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            if (rowDelegate == null)
            {
                throw new DataMapperException("A null DictionaryRowDelegate was passed to QueryForMapWithRowDelegate.");
            }

            // 基于 SQL 配置、参数对象，构建执行命令（IDbCommand），包括设置 SQL 文本和参数
            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            // 执行查询，并交由 RunQueryForMap 方法完成数据读取和字典填充（由委托控制逻辑）
            return RunQueryForMap(request, session, parameterObject, keyProperty, valueProperty, rowDelegate);
        }


        /// <summary>
        /// 执行 SQL 查询，并返回部分结果（支持分页），可指定跳过的行数和最大返回行数。
        /// </summary>
        /// <param name="session">执行 SQL 的会话对象（ISqlMapSession）</param>
        /// <param name="parameterObject">用于设置 SQL 参数的对象</param>
        /// <param name="skipResults">要跳过的记录条数（用于分页）</param>
        /// <param name="maxResults">最多返回的记录条数（用于分页）</param>
        /// <returns>包含查询结果的列表（IList）</returns>
        public virtual IList ExecuteQueryForList(ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            this.preparedCommand.Create(request, session, this.Statement, parameterObject);

            // 实际执行查询逻辑，返回结果列表
            // 传入跳过行数和最大行数实现分页效果
            return RunQueryForList(request, session, parameterObject, skipResults, maxResults);
        }

        /// <summary>
        /// 执行查询，返回指定范围（分页）的结果列表。
        /// </summary>
        /// <param name="request">请求上下文（包含 SQL、参数、命令等）</param>
        /// <param name="session">当前会话，用于数据库连接和操作</param>
        /// <param name="parameterObject">传入的参数对象</param>
        /// <param name="skipResults">需要跳过的行数</param>
        /// <param name="maxResults">需要返回的最大行数</param>
        /// <returns>查询结果列表</returns>
        internal IList RunQueryForList(RequestScope request, ISqlMapSession session, object parameterObject, int skipResults, int maxResults)
        {
            IList list = null;

            using (IDbCommand command = request.IDbCommand)
            {
                // 判断是否指定了结果集合类型
                if (statement.ListClass == null)
                {
                    // 如果没有指定，默认用 ArrayList 接收结果
                    list = new ArrayList();
                }
                else
                {
                    // 如果配置了 ListClass，则使用反射创建实例
                    list = statement.CreateInstanceOfListClass();
                }

                // 执行命令，获取 DataReader 读取数据
                IDataReader reader = command.ExecuteReader();

                try
                {
                    // 跳过 skipResults 行（实现分页）
                    for (int i = 0; i < skipResults; i++)
                    {
                        // 如果没数据可读，提前退出循环
                        if (!reader.Read())
                        {
                            break;
                        }
                    }

                    // 获取数据的主循环
                    int resultsFetched = 0;// 已获取的记录数
                    while ((maxResults == NO_MAXIMUM_RESULTS || resultsFetched < maxResults)
                        && reader.Read())
                    {
                        // 使用结果处理策略将一行数据转换为对象
                        object obj = this.resultStrategy.Process(request, ref reader, null);
                        // 过滤掉被标记为 SKIP 的对象（如 discriminator 不匹配）
                        if (obj != BaseStrategy.SKIP)
                        {
                            list.Add(obj);
                        }
                        resultsFetched++;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }

                // 执行 PostSelect（延迟绑定、嵌套查询等）
                ExecutePostSelect(request);

                // 如果有 Output 参数（如存储过程），读取输出参数
                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            return list;
        }

        /// <summary>
        /// 执行 SQL 查询，并将结果填充到指定的列表中。可选传入 RowDelegate 对每行自定义处理。
        /// </summary>
        /// <param name="aRequest">查询请求作用域（包含语句、参数、IDbCommand 等信息）</param>
        /// <param name="aSession">当前数据库会话</param>
        /// <param name="aParameterObject">用于设置 SQL 参数的对象</param>
        /// <param name="aResultObject">外部传入的结果列表（可为 null）</param>
        /// <param name="aRowDelegate">自定义每行处理逻辑的委托</param>
        /// <returns>填充好的结果列表</returns>
        internal IList RunQueryForList(RequestScope aRequest, ISqlMapSession aSession, object aParameterObject, IList aResultObject, RowDelegate aRowDelegate)
        {
            IList _list = aResultObject;

            using (IDbCommand _command = aRequest.IDbCommand)
            {
                if (aResultObject == null)
                {
                    if (this.statement.ListClass == null)
                    {
                        // 如果未指定 ListClass，使用默认 ArrayList
                        _list = new ArrayList();
                    }
                    else
                    {
                        // 否则使用指定的类型创建实例（支持泛型集合等）
                        _list = this.statement.CreateInstanceOfListClass();
                    }
                }

                // 执行查询命令，获取数据读取器
                IDataReader _reader = _command.ExecuteReader();

                try
                {
                    // 支持处理多结果集（DataReader.NextResult），一般用于存储过程返回多个结果集的情况
                    do
                    {
                        if (aRowDelegate == null)
                        {
                            // 如果未指定 RowDelegate，默认将每一行转换为对象并加入结果集合
                            while (_reader.Read())
                            {
                                object obj = this.resultStrategy.Process(aRequest, ref _reader, null);
                                if (obj != BaseStrategy.SKIP)
                                {
                                    _list.Add(obj);
                                }
                            }
                        }
                        else
                        {
                            // 如果指定了 RowDelegate，则由回调方法处理每一行结果
                            while (_reader.Read())
                            {
                                object obj = this.resultStrategy.Process(aRequest, ref _reader, null);
                                // 调用回调委托进行自定义处理（例如按某个规则分组）
                                aRowDelegate(obj, aParameterObject, _list);
                            }
                        }
                    }
                    while (_reader.NextResult());// 继续读取下一个结果集（如果有）
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _reader.Close();
                    _reader.Dispose();
                }
                // 执行延迟绑定/后处理（如嵌套查询 select）
                this.ExecutePostSelect(aRequest);
                // 获取输出参数（如果是存储过程）
                this.RetrieveOutputParameters(aRequest, aSession, _command, aParameterObject);
            }

            return _list;
        }


        /// <summary>
        /// 执行 SQL 查询，并将结果填充到指定的强类型集合中（由调用者提供 IList）。
        /// </summary>
        /// <param name="aSession">用于执行语句的 SQL 映射会话对象</param>
        /// <param name="aParameterObject">用于设置 SQL 参数的输入对象</param>
        /// <param name="aResultObject">用于接收查询结果的目标集合（强类型 IList，如 List&lt;User&gt;）</param>
        public virtual void ExecuteQueryForList(ISqlMapSession aSession, object aParameterObject, IList aResultObject)
        {
            RequestScope _request = this.statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            RunQueryForList(_request, aSession, aParameterObject, aResultObject, null);
        }


        #endregion


        #region ExecuteUpdate

        /// <summary>
        /// 执行一条更新语句（UPDATE 或 DELETE），返回受影响的行数。
        /// </summary>
        /// <param name="session">用于执行语句的会话对象（ISqlMapSession）</param>
        /// <param name="parameterObject">用于设置 SQL 参数的输入对象</param>
        /// <returns>实际被更新或删除的记录条数</returns>
        public virtual int ExecuteUpdate(ISqlMapSession session, object parameterObject)
        {
            int rows = 0; // 初始化受影响行数为 0
            // 构造执行上下文对象 RequestScope,包含语句内容、参数映射、SQL 命令等
            RequestScope request = statement.Sql.GetRequestScope(this, parameterObject, session);

            // 使用参数对象和上下文准备 IDbCommand,会设置 CommandText、参数等内容
            preparedCommand.Create(request, session, this.Statement, parameterObject);

            using (IDbCommand command = request.IDbCommand)
            {
                // 执行非查询语句（UPDATE/DELETE），返回受影响行数
                rows = command.ExecuteNonQuery();

                // 处理存储过程等可能的 Output 参数（如影响记录数、输出结果）
                RetrieveOutputParameters(request, session, command, parameterObject);
            }

            // 触发执行事件（用于记录日志、监控等）
            RaiseExecuteEvent();

            return rows;
        }

        #endregion 


        #region  ExecuteInsert
        /// <summary>
        /// 执行一条 insert 语句。如果定义了 output 参数或主键生成规则（如 selectKey），将填充参数对象并返回主键值。
        /// </summary>
        /// <param name="session">当前会话</param>
        /// <param name="parameterObject">用于填充参数的对象</param>
        /// <returns>插入后生成的主键值（如有）</returns>
        public virtual object ExecuteInsert(ISqlMapSession session, object parameterObject)
        {
            object generatedKey = null;  // 保存插入后生成的主键（如自增 ID）
            SelectKey selectKeyStatement = null; // 保存 <selectKey> 映射配置（如果存在）

            // 构造执行上下文 RequestScope，包含 SQL、参数映射、命令等信息
            RequestScope request = this.statement.Sql.GetRequestScope(this, parameterObject, session);

            // 如果当前语句是 <insert> 类型，尝试获取 selectKey 配置
            if (statement is Insert)
            {
                selectKeyStatement = ((Insert)statement).SelectKey;
            }

            // 如果存在 <selectKey> 并配置为插入前执行（isAfter=false）
            if (selectKeyStatement != null && !selectKeyStatement.isAfter)
            {
                // 获取并执行 selectKey 映射语句，返回主键值
                IMappedStatement mappedStatement = sqlMap.GetMappedStatement(selectKeyStatement.Id);
                generatedKey = mappedStatement.ExecuteQueryForObject(session, parameterObject);

                // 将主键值设置回参数对象中指定的属性
                ObjectProbe.SetMemberValue(parameterObject, selectKeyStatement.PropertyName, generatedKey,
                    request.DataExchangeFactory.ObjectFactory,
                    request.DataExchangeFactory.AccessorFactory);
            }

            // 准备 IDbCommand，包括设置 SQL 文本和参数
            preparedCommand.Create(request, session, this.Statement, parameterObject);

            // 执行 SQL 命令
            using (IDbCommand command = request.IDbCommand)
            {
                // 如果是 insert 语句，直接执行非查询命令（ExecuteNonQuery）
                if (statement is Insert)
                {
                    command.ExecuteNonQuery();
                }
                // 如果是存储过程并指定了返回类（ResultClass），且是简单类型
                else if (statement is Procedure && (statement.ResultClass != null) &&
                        sqlMap.TypeHandlerFactory.IsSimpleType(statement.ResultClass))
                {
                    // 添加一个 ReturnValue 类型的参数（用于接收返回值）
                    IDataParameter returnValueParameter = command.CreateParameter();
                    returnValueParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValueParameter);

                    // 执行命令
                    command.ExecuteNonQuery();
                    generatedKey = returnValueParameter.Value;

                    // 使用类型处理器转换返回值
                    ITypeHandler typeHandler = sqlMap.TypeHandlerFactory.GetTypeHandler(statement.ResultClass);
                    generatedKey = typeHandler.GetDataBaseValue(generatedKey, statement.ResultClass);
                }
                else
                {
                    // 否则使用 ExecuteScalar 获取可能的返回值（如 insert 后 select last_insert_id()）
                    generatedKey = command.ExecuteScalar();

                    // 如果设置了 ResultClass，并且是简单类型，则使用类型处理器转换
                    if ((statement.ResultClass != null) &&
                        sqlMap.TypeHandlerFactory.IsSimpleType(statement.ResultClass))
                    {
                        ITypeHandler typeHandler = sqlMap.TypeHandlerFactory.GetTypeHandler(statement.ResultClass);
                        generatedKey = typeHandler.GetDataBaseValue(generatedKey, statement.ResultClass);
                    }
                }

                // 如果 <selectKey> 配置为插入后执行（isAfter=true）
                if (selectKeyStatement != null && selectKeyStatement.isAfter)
                {
                    // 执行 selectKey 映射语句获取主键值
                    IMappedStatement mappedStatement = sqlMap.GetMappedStatement(selectKeyStatement.Id);
                    generatedKey = mappedStatement.ExecuteQueryForObject(session, parameterObject);

                    // 将主键设置回参数对象的指定属性
                    ObjectProbe.SetMemberValue(parameterObject, selectKeyStatement.PropertyName, generatedKey,
                        request.DataExchangeFactory.ObjectFactory,
                        request.DataExchangeFactory.AccessorFactory);
                }

                // 如果是存储过程，还需提取 Output 参数并写入参数对象
                RetrieveOutputParameters(request, session, command, parameterObject);
            }
            // 触发查询执行事件（用于日志、监控等扩展）
            RaiseExecuteEvent();

            return generatedKey;
        }

        #endregion


        #region ExecuteQueryForMap

        /// <summary>
        /// 执行 SQL 查询，返回结果集，并按指定的属性构造键值映射（字典）。
        /// </summary>
        /// <param name="session">执行 SQL 的会话</param>
        /// <param name="parameterObject">用于设置参数的对象</param>
        /// <param name="keyProperty">作为字典 key 的属性名</param>
        /// <param name="valueProperty">作为字典 value 的属性名，若为 null 则整个对象作为 value</param>
        /// <returns>一个 Hashtable，key 来自 keyProperty，value 来自 valueProperty 或整个对象</returns>
        public virtual IDictionary ExecuteQueryForMap(ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty)
        {
            // 构建 SQL 执行上下文
            RequestScope _request = statement.Sql.GetRequestScope(this, parameterObject, session);
            // 创建 IDbCommand 对象，准备 SQL 和参数
            this.preparedCommand.Create(_request, session, this.Statement, parameterObject);
            // 实际执行查询并组装结果为字典返回（不使用委托）
            return RunQueryForMap(_request, session, parameterObject, keyProperty, valueProperty, null);
        }


        /// <summary>
        /// 实际执行 SQL 查询，并组装成以指定属性为 key 的 Dictionary。
        /// </summary>
        internal IDictionary RunQueryForMap(RequestScope request, ISqlMapSession session, object parameterObject, string keyProperty, string valueProperty, DictionaryRowDelegate rowDelegate)
        {
            // 初始化结果字典（Hashtable 类型）
            IDictionary map = new Hashtable();

            // 使用 using 块保证命令对象资源及时释放
            using (IDbCommand command = request.IDbCommand)
            {
                // 执行 SQL，获取数据读取器
                IDataReader reader = command.ExecuteReader();
                try
                {
                    if (rowDelegate == null)
                    {
                        // 不使用委托，直接逐行处理构造 map
                        while (reader.Read())
                        {
                            // 使用结果映射策略将一行数据转为对象
                            object obj = this.resultStrategy.Process(request, ref reader, null);
                            // 通过反射读取 obj 中的 key 属性值（作为字典键）
                            object key = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
                            object value = obj;
                            // 如果配置了 valueProperty，则只取其中一个属性作为值
                            if (valueProperty != null)
                            {
                                value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
                            }
                            // 加入到 map 中
                            map.Add(key, value);
                        }
                    }
                    else
                    {
                        // 使用 DictionaryRowDelegate 回调每一行，自定义 map 构建逻辑
                        while (reader.Read())
                        {
                            object obj = this.resultStrategy.Process(request, ref reader, null);
                            object key = ObjectProbe.GetMemberValue(obj, keyProperty, request.DataExchangeFactory.AccessorFactory);
                            object value = obj;
                            if (valueProperty != null)
                            {
                                value = ObjectProbe.GetMemberValue(obj, valueProperty, request.DataExchangeFactory.AccessorFactory);
                            }
                            // 调用自定义委托处理当前这条记录（用户可按需组织 map）
                            rowDelegate(key, value, parameterObject, map);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
                // 执行后绑定操作，如嵌套 select 处理
                ExecutePostSelect(request);
            }
            return map;
        }


        #endregion


        #region ExecuteQueryForDataSet


        /// <summary>
        /// 执行sql返回DataSet数据集，没有应用结果策略(resultStrategy)，使用IDbDataAdapter直接返回DataSet
        /// </summary>
        /// <param name="aSession">当前 SQL 会话</param>
        /// <param name="aParameterObject">SQL 参数对象</param>
        /// <returns>包含结果的 DataSet</returns>
        public virtual DataSet ExecuteQueryForDataSet(ISqlMapSession aSession, object aParameterObject)
        {
            RequestScope _request = this.statement.Sql.GetRequestScope(this, aParameterObject, aSession);

            //创建命令对象IDbCommand，_request.IDbCommand=aSession.CreateCommand(aStatement.CommandType)
            //命令对象执行语句CommandText赋值，_request.IDbCommand.CommandText = _request.PreparedStatement.PreparedSql
            //给命令对象创建参数列表（aRequest.IDbCommand.Parameters），并赋值（从aParameterObject获取参数对应的值）
            //一切准备就绪，可执行命令获取数据
            this.preparedCommand.Create(_request, aSession, this.Statement, aParameterObject);

            return RunQueryForDataSet(_request, aSession, aParameterObject);
        }


        /// <summary>
        /// 执行sql返回DataSet数据集，没有应用结果策略，使用IDbDataAdapter直接返回DataSet
        /// </summary>
        /// <param name="aRequest">执行上下文</param>
        /// <param name="aSession">当前 SQL 会话</param>
        /// <param name="aParameterObject">SQL 参数对象</param>
        /// <returns>包含结果的 DataSet</returns>
        internal DataSet RunQueryForDataSet(RequestScope aRequest, ISqlMapSession aSession, object aParameterObject)
        {
            DataSet _ds = new DataSet();

            // 提取真实 IDbCommand
            IDbCommand command = (aRequest.IDbCommand is DbCommandDecorator decorator) ? decorator.IDbCommand : aRequest.IDbCommand;

            using (command)
            {
                try
                {
                    // 使用连接创建 DataAdapter（通常为 SqlDataAdapter、OracleDataAdapter 等）
                    IDbDataAdapter _dataAdapter = aSession.CreateDataAdapter(command);
                    _dataAdapter.Fill(_ds);
                }
                catch
                {
                    throw;
                }
            }

            return _ds;
        }

        #endregion


        /// <summary>
        /// 执行 <see cref="PostBindind"/> 中定义的延迟绑定策略（通常在主查询之后处理子查询）
        /// </summary>
        /// <remarks>
        /// 此方法用于执行“后绑定”操作（Post Binding），主要用于处理 延迟加载 或 嵌套查询 的场景，例如一个字段对应另一个 select 查询
        /// </remarks>
        /// <param name="aRequest">当前请求作用域，包含延迟队列 QueueSelect</param>
        private void ExecutePostSelect(RequestScope aRequest)
        {
            // 如果队列中还有需要后处理的绑定操作，就逐个执行
            while (aRequest.QueueSelect.Count > 0)
            {
                // 从队列中取出一个 PostBindind 对象（包含方法名、目标对象等信息）
                PostBindind _postSelect = aRequest.QueueSelect.Dequeue() as PostBindind;
                // 通过方法名获取对应的后处理策略（延迟加载策略、嵌套查询策略等），并执行
                PostSelectStrategyFactory.Get(_postSelect.Method).Execute(_postSelect, aRequest);
            }
        }


        /// <summary>
        /// 触发 Execute 事件，传递 ExecuteEventArgs 参数
        /// （当查询执行完成时调用，用于通知监听者）
        /// </summary>
        private void RaiseExecuteEvent()
        {
            // 创建一个事件参数对象
            ExecuteEventArgs e = new ExecuteEventArgs();
            // 设置当前执行的语句名称（即 SQL 映射语句的 ID）
            e.StatementName = statement.Id;

            // 如果有外部订阅了 Execute 事件
            if (Execute != null)
            {
                // 调用事件委托，通知外部执行已完成
                Execute(this, e);
            }
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns>A string that describes the MappedStatement</returns>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("\tMappedStatement: " + this.Id);
            buffer.Append(Environment.NewLine);
            if (statement.ParameterMap != null) buffer.Append(statement.ParameterMap.Id);

            return buffer.ToString();
        }



    }
}
