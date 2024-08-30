using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;

namespace IBatisNetSelf.DataMapper.Commands
{
    /// <summary>
    /// Summary description for DefaultPreparedCommand.
    /// </summary>
    internal class DefaultPreparedCommand : IPreparedCommand
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region IPreparedCommand Members

        /// <summary>
        /// Create an IDbCommand for the SqlMapSession and the current SQL Statement
        /// and fill IDbCommand IDataParameter's with the parameterObject.
        /// </summary>
        /// <param name="aRequest"></param>
        /// <param name="aSession">The SqlMapSession</param>
        /// <param name="aStatement">The IStatement</param>
        /// <param name="aParameterObject">
        /// The parameter object that will fill the sql parameter
        /// </param>
        /// <returns>An IDbCommand with all the IDataParameter filled.</returns>
        public void Create(RequestScope aRequest, ISqlMapSession aSession, IStatement aStatement, object aParameterObject)
        {
            // the IDbConnection & the IDbTransaction are assign in the CreateCommand 
            IDbCommand _dbComnand = aSession.CreateCommand(aStatement.CommandType);
            aRequest.IDbCommand = new DbCommandDecorator(_dbComnand, aRequest);

            aRequest.IDbCommand.CommandText = aRequest.PreparedStatement.PreparedSql;

            if (logger.IsDebugEnabled)
            {
                logger.Debug("Statement Id: [" + aStatement.Id + "] PreparedStatement : [" + aRequest.IDbCommand.CommandText + "]");
            }
            //给执行命令创建参数，并赋值，从aParameterObject获取参数对应的值。
            ApplyParameterMap(aSession, aRequest.IDbCommand, aRequest, aStatement, aParameterObject);
        }


        /// <summary>
        /// Applies the parameter map.
        /// </summary>
        /// <param name="aSession">The session.</param>
        /// <param name="aCommand">The command.</param>
        /// <param name="aRequest">The request.</param>
        /// <param name="aStatement">The statement.</param>
        /// <param name="aParameterObject">The parameter object.</param>
        protected virtual void ApplyParameterMap(ISqlMapSession aSession, IDbCommand aCommand,
            RequestScope aRequest, IStatement aStatement, object aParameterObject)
        {
            StringCollection _dbParameterNames = aRequest.PreparedStatement.DbParametersName;
            IDbDataParameter[] _preparedDbParameters = aRequest.PreparedStatement.DbParameters;
            StringBuilder _paramListLog = new StringBuilder(); // Log info
            StringBuilder _typeListLog = new StringBuilder(); // Log info

            int _count = _dbParameterNames.Count;

            for (int i = 0; i < _count; ++i)
            {
                IDbDataParameter _preparedDbParameter = _preparedDbParameters[i];
                IDbDataParameter _dbParameter = aCommand.CreateParameter();
                ParameterProperty _parameterProperty = aRequest.ParameterMap.GetProperty(i);

                #region Logging
                if (logger.IsDebugEnabled)
                {
                    _paramListLog.Append(_preparedDbParameter.ParameterName);
                    _paramListLog.Append("=[");
                    _typeListLog.Append(_preparedDbParameter.ParameterName);
                    _typeListLog.Append("=[");
                }
                #endregion

                if (aCommand.CommandType == CommandType.StoredProcedure)
                {
                    #region store procedure command

                    // A store procedure must always use a ParameterMap 
                    // to indicate the mapping order of the properties to the columns
                    if (aRequest.ParameterMap == null) // Inline Parameters
                    {
                        throw new DataMapperException("A procedure statement tag must alway have a parameterMap attribute, which is not the case for the procedure '" + aStatement.Id + "'.");
                    }
                    else // Parameters via ParameterMap
                    {
                        if (_parameterProperty.DirectionAttribute.Length == 0)
                        {
                            _parameterProperty.Direction = _preparedDbParameter.Direction;
                        }

                        _preparedDbParameter.Direction = _parameterProperty.Direction;
                    }
                    #endregion
                }

                #region Logging
                if (logger.IsDebugEnabled)
                {
                    _paramListLog.Append(_parameterProperty.PropertyName);
                    _paramListLog.Append(",");
                }
                #endregion

                //参数对象_dbParameter赋值，从aParameterObject获取参数对应的值。
                aRequest.ParameterMap.SetParameter(_parameterProperty, _dbParameter, aParameterObject);

                _dbParameter.Direction = _preparedDbParameter.Direction;

                // With a ParameterMap, we could specify the ParameterDbTypeProperty
                if (aRequest.ParameterMap != null)
                {
                    if (_parameterProperty.DbType != null && _parameterProperty.DbType.Length > 0)
                    {
                        string _dbTypePropertyName = aSession.DataSource.DbProvider.ParameterDbTypeProperty;
                        //获取预备参数_preparedDbParameter对象的_dbTypePropertyName属性的值，即预备参数_preparedDbParameter的数据类型
                        object _propertyValue = ObjectProbe.GetMemberValue(_preparedDbParameter, _dbTypePropertyName, aRequest.DataExchangeFactory.AccessorFactory);
                        //设置参数_dbParameter对象的_dbTypePropertyName属性值，即设置参数对象_dbParameter的数据类型
                        ObjectProbe.SetMemberValue(_dbParameter, _dbTypePropertyName, _propertyValue,
                            aRequest.DataExchangeFactory.ObjectFactory, aRequest.DataExchangeFactory.AccessorFactory);
                    }
                    else
                    {
                        //parameterCopy.DbType = sqlParameter.DbType;
                    }
                }
                else
                {
                    //parameterCopy.DbType = sqlParameter.DbType;
                }


                #region Logging
                if (logger.IsDebugEnabled)
                {
                    if (_dbParameter.Value == DBNull.Value)
                    {
                        _paramListLog.Append("null");
                        _paramListLog.Append("], ");
                        _typeListLog.Append("System.DBNull, null");
                        _typeListLog.Append("], ");
                    }
                    else
                    {

                        _paramListLog.Append(_dbParameter.Value.ToString());
                        _paramListLog.Append("], ");

                        // sqlParameter.DbType could be null (as with Npgsql)
                        // if PreparedStatementFactory did not find a dbType for the parameter in:
                        // line 225: "if (property.DbType.Length >0)"
                        // Use parameterCopy.DbType

                        //typeLogList.Append( sqlParameter.DbType.ToString() );
                        _typeListLog.Append(_dbParameter.DbType.ToString());
                        _typeListLog.Append(", ");
                        _typeListLog.Append(_dbParameter.Value.GetType().ToString());
                        _typeListLog.Append("], ");
                    }
                }
                #endregion

                // JIRA-49 Fixes (size, precision, and scale)
                if (aSession.DataSource.DbProvider.SetDbParameterSize)
                {
                    if (_preparedDbParameter.Size > 0)
                    {
                        _dbParameter.Size = _preparedDbParameter.Size;
                    }
                }

                if (aSession.DataSource.DbProvider.SetDbParameterPrecision)
                {
                    _dbParameter.Precision = _preparedDbParameter.Precision;
                }

                if (aSession.DataSource.DbProvider.SetDbParameterScale)
                {
                    _dbParameter.Scale = _preparedDbParameter.Scale;
                }

                _dbParameter.ParameterName = _preparedDbParameter.ParameterName;

                aCommand.Parameters.Add(_dbParameter);
            }

            #region Logging

            if (logger.IsDebugEnabled && _dbParameterNames.Count > 0)
            {
                logger.Debug("Statement Id: [" + aStatement.Id + "] Parameters: [" + _paramListLog.ToString(0, _paramListLog.Length - 2) + "]");
                logger.Debug("Statement Id: [" + aStatement.Id + "] Types: [" + _typeListLog.ToString(0, _typeListLog.Length - 2) + "]");
            }
            #endregion
        }

        #endregion
    }
}
