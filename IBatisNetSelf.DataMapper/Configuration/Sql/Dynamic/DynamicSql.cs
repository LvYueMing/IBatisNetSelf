using IBatisNetSelf.DataMapper.Configuration.ParameterMapping;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers;
using IBatisNetSelf.DataMapper.Configuration.Sql.SimpleDynamic;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.DataExchange;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic
{
    /// <summary>
    /// DynamicSql represent the root element of a dynamic sql statement
    /// </summary>
    /// <example>
    ///      <dynamic prepend="where">...</dynamic>
    /// </example>
    internal sealed class DynamicSql : ISql, IDynamicParent
    {

        #region Fields

        private IList children = new ArrayList();
        private IStatement statement = null;
        private bool usePositionalParameters = false;
        private InlineParameterMapParser paramParser = null;
        private DataExchangeFactory dataExchangeFactory = null;

        #endregion

        #region Constructor (s) / Destructor


        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSql"/> class.
        /// </summary>
        /// <param name="configScope">The config scope.</param>
        /// <param name="statement">The statement.</param>
        internal DynamicSql(ConfigurationScope configScope, IStatement statement)
        {
            this.statement = statement;

            this.usePositionalParameters = configScope.DataSource.DbProvider.UsePositionalParameters;
            this.dataExchangeFactory = configScope.DataExchangeFactory;
        }
        #endregion

        #region Methods

        #region ISql IDynamicParent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ISqlChild child)
        {
            this.children.Add(child);
        }

        #endregion

        #region ISql Members


        /// <summary>
        /// Builds a new <see cref="RequestScope"/> and the <see cref="IDbCommand"/> text to execute.
        /// </summary>
        /// <param name="aParameterObject">The parameter object (used in DynamicSql)</param>
        /// <param name="aSession">The current session</param>
        /// <param name="aMappedStatement">The <see cref="IMappedStatement"/>.</param>
        /// <returns>A new <see cref="RequestScope"/>.</returns>
        public RequestScope GetRequestScope(IMappedStatement aMappedStatement,
            object aParameterObject, ISqlMapSession aSession)
        {
            RequestScope _request = new RequestScope(dataExchangeFactory, aSession, statement);

            this.paramParser = new InlineParameterMapParser();

            string _sqlStatement = this.Process(_request, aParameterObject);
            _request.PreparedStatement = this.BuildPreparedStatement(aSession, _request, _sqlStatement);
            _request.MappedStatement = aMappedStatement;

            return _request;
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aRequest"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        private string Process(RequestScope aRequest, object aParameterObject)
        {
            SqlTagContext _sqlTagContext = new SqlTagContext();
            IList _localChildren = this.children;

            this.ProcessBodyChildren(aRequest, _sqlTagContext, aParameterObject, _localChildren);

            // Builds a 'dynamic' ParameterMap
            ParameterMap _map = new ParameterMap(aRequest.DataExchangeFactory);
            _map.Id = statement.Id + "-InlineParameterMap";
            _map.Initialize(usePositionalParameters, aRequest);
            _map.Class = statement.ParameterClass;

            // Adds 'dynamic' ParameterProperty
            IList _parameters = _sqlTagContext.GetParameterMappings();
            int count = _parameters.Count;
            for (int i = 0; i < count; i++)
            {
                _map.AddParameterProperty((ParameterProperty)_parameters[i]);
            }
            aRequest.ParameterMap = _map;

            string _dynSql = _sqlTagContext.BodyText;

            // Processes $substitutions$ after DynamicSql
            if (SimpleDynamicSql.IsSimpleDynamicSql(_dynSql))
            {
                _dynSql = new SimpleDynamicSql(aRequest, _dynSql, statement).GetSql(aParameterObject);
            }
            return _dynSql;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aRequest"></param>
        /// <param name="aSqlTagContext"></param>
        /// <param name="aParameterObject"></param>
        /// <param name="aLocalChildren"></param>
        private void ProcessBodyChildren(RequestScope aRequest, SqlTagContext aSqlTagContext,
            object aParameterObject, IList aLocalChildren)
        {
            StringBuilder _sqlBuffer = aSqlTagContext.GetWriter();
            this.ProcessBodyChildren(aRequest, aSqlTagContext, aParameterObject, aLocalChildren.GetEnumerator(), _sqlBuffer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="aRequest"></param>
        /// <param name="aSqlTagContext"></param>
        /// <param name="aParameterObject"></param>
        /// <param name="aLocalChildrenEnumerator"></param>
        /// <param name="aSqlBuffer"></param>
        private void ProcessBodyChildren(RequestScope aRequest, SqlTagContext aSqlTagContext,
            object aParameterObject, IEnumerator aLocalChildrenEnumerator, StringBuilder aSqlBuffer)
        {
            while (aLocalChildrenEnumerator.MoveNext())
            {
                ISqlChild _child = (ISqlChild)aLocalChildrenEnumerator.Current;

                if (_child is SqlText)
                {
                    SqlText _sqlText = (SqlText)_child;
                    string _sqlStatement = _sqlText.Text;
                    if (_sqlText.IsWhiteSpace)
                    {
                        aSqlBuffer.Append(_sqlStatement);
                    }
                    else
                    {
                        aSqlBuffer.Append(" ");
                        aSqlBuffer.Append(_sqlStatement);

                        ParameterProperty[] _parameters = _sqlText.Parameters;
                        if (_parameters != null)
                        {
                            int _length = _parameters.Length;
                            for (int i = 0; i < _length; i++)
                            {
                                aSqlTagContext.AddParameterMapping(_parameters[i]);
                            }
                        }
                    }
                }
                else if (_child is SqlTag)
                {
                    SqlTag _tag = (SqlTag)_child;
                    ISqlTagHandler _handler = _tag.Handler;
                    int _response = BaseTagHandler.INCLUDE_BODY;

                    do
                    {
                        StringBuilder _body = new StringBuilder();

                        _response = _handler.DoStartFragment(aSqlTagContext, _tag, aParameterObject);
                        if (_response != BaseTagHandler.SKIP_BODY)
                        {
                            if (aSqlTagContext.IsOverridePrepend
                                && aSqlTagContext.FirstNonDynamicTagWithPrepend == null
                                && _tag.IsPrependAvailable
                                && !(_tag.Handler is DynamicTagHandler))
                            {
                                aSqlTagContext.FirstNonDynamicTagWithPrepend = _tag;
                            }

                            this.ProcessBodyChildren(aRequest, aSqlTagContext, aParameterObject, _tag.GetChildrenEnumerator(), _body);

                            _response = _handler.DoEndFragment(aSqlTagContext, _tag, aParameterObject, _body);
                            _handler.DoPrepend(aSqlTagContext, _tag, aParameterObject, _body);
                            if (_response != BaseTagHandler.SKIP_BODY)
                            {
                                if (_body.Length > 0)
                                {
                                    // BODY OUT

                                    if (_handler.IsPostParseRequired)
                                    {
                                        SqlText sqlText = paramParser.ParseInlineParameterMap(aRequest, null, _body.ToString());
                                        aSqlBuffer.Append(sqlText.Text);
                                        ParameterProperty[] mappings = sqlText.Parameters;
                                        if (mappings != null)
                                        {
                                            int length = mappings.Length;
                                            for (int i = 0; i < length; i++)
                                            {
                                                aSqlTagContext.AddParameterMapping(mappings[i]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        aSqlBuffer.Append(" ");
                                        aSqlBuffer.Append(_body.ToString());
                                    }
                                    if (_tag.IsPrependAvailable && _tag == aSqlTagContext.FirstNonDynamicTagWithPrepend)
                                    {
                                        aSqlTagContext.IsOverridePrepend = false;
                                    }
                                }
                            }
                        }
                    }
                    while (_response == BaseTagHandler.REPEAT_BODY);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="request"></param>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        private PreparedStatement BuildPreparedStatement(ISqlMapSession session, RequestScope request, string sqlStatement)
        {
            PreparedStatementFactory _factory = new PreparedStatementFactory(session, request, statement, sqlStatement);
            return _factory.Prepare();
        }
        #endregion

    }
}
