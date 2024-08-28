using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.Scope;
using IBatisNetSelf.DataMapper.TypeHandlers;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.DataMapper.Configuration.ParameterMapping
{
    /// <summary>
    /// Summary description for InlineParameterMapParser.
    /// </summary>
    internal class InlineParameterMapParser
    {

        #region Fields

        private const string PARAMETER_TOKEN = "#";
        private const string PARAM_DELIM = ":";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public InlineParameterMapParser()
        {
        }
        #endregion

        /// <summary>
        /// Parse Inline ParameterMap
        /// </summary>
        /// <param name="aStatement"></param>
        /// <param name="aStringSql"></param>
        /// <returns>A new sql command text.</returns>
        /// <param name="aConfigScope"></param>
        public SqlText ParseInlineParameterMap(IScope aConfigScope, IStatement aStatement, string aStringSql)
        {
            string _newSql = aStringSql;
            ArrayList _paramPropertyList = new ArrayList();
            Type _parameterClassType = null;

            if (aStatement != null)
            {
                _parameterClassType = aStatement.ParameterClass;
            }

            Common.Utilities.StringTokenizer _sqlTokens = new Common.Utilities.StringTokenizer(aStringSql, PARAMETER_TOKEN, true);
            StringBuilder _newSqlBuffer = new StringBuilder();

            string _token = null;
            string _lastToken = null;

            IEnumerator _enumerator = _sqlTokens.GetEnumerator();

            while (_enumerator.MoveNext())
            {
                _token = (string)_enumerator.Current;

                if (PARAMETER_TOKEN.Equals(_lastToken))
                {
                    if (PARAMETER_TOKEN.Equals(_token))
                    {
                        _newSqlBuffer.Append(PARAMETER_TOKEN);
                        _token = null;
                    }
                    else
                    {
                        ParameterProperty _paramProperty = null;
                        _paramProperty = NewParseMapping(_token, _parameterClassType, aConfigScope);

                        _paramPropertyList.Add(_paramProperty);
                        _newSqlBuffer.Append("? ");

                        _enumerator.MoveNext();
                        _token = (string)_enumerator.Current;
                        if (!PARAMETER_TOKEN.Equals(_token))
                        {
                            throw new DataMapperException("Unterminated inline parameter in mapped statement (" + aStatement.Id + ").");
                        }
                        _token = null;
                    }
                }
                else
                {
                    if (!PARAMETER_TOKEN.Equals(_token))
                    {
                        _newSqlBuffer.Append(_token);
                    }
                }
                _lastToken = _token;
            }

            _newSql = _newSqlBuffer.ToString();

            ParameterProperty[] _paramPropertyArray = (ParameterProperty[])_paramPropertyList.ToArray(typeof(ParameterProperty));

            SqlText _sqlText = new SqlText();
            _sqlText.Text = _newSql;
            _sqlText.Parameters = _paramPropertyArray;

            return _sqlText;
        }


        /// <summary>
        /// Parse inline parameter with syntax as
        /// #propertyName,type=string,dbype=Varchar,direction=Input,nullValue=N/A,handler=string#
        /// </summary>
        /// <param name="token"></param>
        /// <param name="parameterClassType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        private ParameterProperty NewParseMapping(string token, Type parameterClassType, IScope scope)
        {
            ParameterProperty _mapping = new ParameterProperty();

            Common.Utilities.StringTokenizer _paramParser = new Common.Utilities.StringTokenizer(token, "=,", false);
            IEnumerator _enumeratorParam = _paramParser.GetEnumerator();

            _enumeratorParam.MoveNext();

            _mapping.PropertyName = ((string)_enumeratorParam.Current).Trim();

            while (_enumeratorParam.MoveNext())
            {
                string _field = (string)_enumeratorParam.Current;
                if (_enumeratorParam.MoveNext())
                {
                    string _value = (string)_enumeratorParam.Current;
                    if ("type".Equals(_field))
                    {
                        _mapping.CLRType = _value;
                    }
                    else if ("dbType".Equals(_field))
                    {
                        _mapping.DbType = _value;
                    }
                    else if ("direction".Equals(_field))
                    {
                        _mapping.DirectionAttribute = _value;
                    }
                    else if ("nullValue".Equals(_field))
                    {
                        _mapping.NullValue = _value;
                    }
                    else if ("handler".Equals(_field))
                    {
                        _mapping.CallBackName = _value;
                    }
                    else
                    {
                        throw new DataMapperException("Unrecognized parameter mapping field: '" + _field + "' in " + token);
                    }
                }
                else
                {
                    throw new DataMapperException("Incorrect inline parameter map format (missmatched name=value pairs): " + token);
                }
            }

            if (_mapping.CallBackName.Length > 0)
            {
                _mapping.Initialize(scope, parameterClassType);
            }
            else
            {
                ITypeHandler handler = null;
                if (parameterClassType == null)
                {
                    handler = scope.DataExchangeFactory.TypeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    handler = ResolveTypeHandler(scope.DataExchangeFactory.TypeHandlerFactory,
                        parameterClassType, _mapping.PropertyName,
                        _mapping.CLRType, _mapping.DbType);
                }
                _mapping.TypeHandler = handler;
                _mapping.Initialize(scope, parameterClassType);
            }

            return _mapping;
        }



        /// <summary>
        /// Resolve TypeHandler
        /// </summary>
        /// <param name="parameterClassType"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <param name="dbType"></param>
        /// <param name="typeHandlerFactory"></param>
        /// <returns></returns>
        private ITypeHandler ResolveTypeHandler(TypeHandlerFactory typeHandlerFactory,
            Type parameterClassType, string propertyName,
            string propertyType, string dbType)
        {
            ITypeHandler _handler = null;

            if (parameterClassType == null)
            {
                _handler = typeHandlerFactory.GetUnkownTypeHandler();
            }
            else if (typeof(IDictionary).IsAssignableFrom(parameterClassType))
            {
                if (propertyType == null || propertyType.Length == 0)
                {
                    _handler = typeHandlerFactory.GetUnkownTypeHandler();
                }
                else
                {
                    try
                    {
                        Type _typeClass = TypeUtils.ResolveType(propertyType);
                        _handler = typeHandlerFactory.GetTypeHandler(_typeClass, dbType);
                    }
                    catch (Exception e)
                    {
                        throw new ConfigurationException("Error. Could not set TypeHandler.  Cause: " + e.Message, e);
                    }
                }
            }
            else if (typeHandlerFactory.GetTypeHandler(parameterClassType, dbType) != null)
            {
                _handler = typeHandlerFactory.GetTypeHandler(parameterClassType, dbType);
            }
            else
            {
                Type _typeClass = ObjectProbe.GetMemberTypeForGetter(parameterClassType, propertyName);
                _handler = typeHandlerFactory.GetTypeHandler(_typeClass, dbType);
            }

            return _handler;
        }

    }
}
