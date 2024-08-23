using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.TypeHandlers
{
    /// <summary>
    /// Description  ResultGetterImpl.
    /// </summary>
    public sealed class ResultGetterImpl : IResultGetter
    {

        #region Fields

        private int _columnIndex = int.MinValue;
        private string _columnName = string.Empty;
        private object _outputValue = null;

        private IDataReader _dataReader = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance for a IDataReader and column index
        /// </summary>
        /// <param name="dataReader">The dataReader</param>
        /// <param name="columnIndex">the column index</param>
        public ResultGetterImpl(IDataReader dataReader, int columnIndex)
        {
            _columnIndex = columnIndex;
            _dataReader = dataReader;
        }

        /// <summary>
        /// Creates an instance for a IDataReader and column name
        /// </summary>
        /// <param name="dataReader">The dataReader</param>
        /// <param name="columnName">the column name</param>
        public ResultGetterImpl(IDataReader dataReader, string columnName)
        {
            _columnName = columnName;
            _dataReader = dataReader;
        }

        /// <summary>
        /// Creates an instance for an output parameter
        /// </summary>
        /// <param name="outputValue">value of an output parameter (store procedure)</param>
        public ResultGetterImpl(object outputValue)
        {
            _outputValue = outputValue;
        }
        #endregion

        #region IResultGetter members

        /// <summary>
        /// Returns the underlying IDataReader
        /// </summary>
        /// <remarks>Null for an output parameter</remarks>
        public IDataReader DataReader
        {
            get { return _dataReader; }
        }

        /// <summary>
        /// Get the parameter value
        /// </summary>
        public object Value
        {
            get
            {
                if (_columnName.Length > 0)
                {
                    int index = _dataReader.GetOrdinal(_columnName);
                    return _dataReader.GetValue(index);
                }
                else if (_columnIndex >= 0)
                {
                    return _dataReader.GetValue(_columnIndex);
                }
                else
                {
                    return _outputValue;
                }
            }
        }
        #endregion

    }
}
