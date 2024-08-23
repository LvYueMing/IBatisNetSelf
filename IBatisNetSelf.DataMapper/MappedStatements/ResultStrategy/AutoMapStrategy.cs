using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// <see cref="IResultStrategy"/> implementation used when implicit 'ResultMap'.
    /// </summary>
    public sealed class AutoMapStrategy : IResultStrategy
    {
        /// <summary>
        /// Auto-map the reader to the result object.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aReader">The reader.</param>
        /// <param name="aResultObject">The result object.</param>
        /// <returns>The AutoResultMap use to map the resultset.</returns>
        private AutoResultMap InitializeAutoResultMap(RequestScope aRequest, ref IDataReader aReader, ref object aResultObject)
        {
            AutoResultMap _resultMap = aRequest.CurrentResultMap as AutoResultMap;

            if (aRequest.Statement.AllowRemapping)
            {
                _resultMap = _resultMap.Clone();

                ResultPropertyCollection _properties = ReaderAutoMapper.Build(
                    aRequest.DataExchangeFactory,
                    aReader,
                    ref aResultObject);

                _resultMap.Properties.AddRange(_properties);
            }
            else
            {
                if (!_resultMap.IsInitalized)
                {
                    lock (_resultMap)
                    {
                        if (!_resultMap.IsInitalized)
                        {
                            ResultPropertyCollection _properties = ReaderAutoMapper.Build(
                               aRequest.DataExchangeFactory,
                               aReader,
                               ref aResultObject);

                            _resultMap.Properties.AddRange(_properties);
                            _resultMap.IsInitalized = true;
                        }
                    }
                }

            }

            return _resultMap;
        }


        #region IResultStrategy Members

        /// <summary>
        /// Processes the specified <see cref="IDataReader"/> 
        /// a an auto result map is used.
        /// </summary>
        /// <param name="aRequest">The request.</param>
        /// <param name="aReader">The reader.</param>
        /// <param name="aResultObject">The result object.</param>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {
            object _outObject = aResultObject;

            if (_outObject == null)
            {
                _outObject = (aRequest.CurrentResultMap as AutoResultMap).CreateInstanceOfResultClass();
            }

            AutoResultMap _resultMap = InitializeAutoResultMap(aRequest, ref aReader, ref _outObject);

            // En configuration initialiser des AutoResultMap (IResultMap) avec uniquement leur class name et class et les mettres
            // ds Statement.ResultsMap puis ds AutoMapStrategy faire comme AutoResultMap ds Java
            // tester si la request.CurrentResultMap [AutoResultMap (IResultMap)] est initialis閑 
            // [if (allowRemapping || getResultMappings() == null) {initialize(rs);] java
            // si ( request.Statement.AllowRemapping || (request.CurrentResultMap as AutoResultMap).IsInitalized) ....

            for (int index = 0; index < _resultMap.Properties.Count; index++)
            {
                ResultProperty _property = _resultMap.Properties[index];
                _resultMap.SetValueOfProperty(ref _outObject, _property,
                                             _property.GetDataBaseValue(aReader));
            }

            return _outObject;
        }

        #endregion
    }
}
