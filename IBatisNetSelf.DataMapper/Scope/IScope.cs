using IBatisNetSelf.DataMapper.DataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Scope
{
    public interface IScope
    {
        /// <summary>
        ///  Get the error context
        /// </summary>
        ErrorContext ErrorContext { get; }

        /// <summary>
        /// The factory for DataExchange objects
        /// </summary>
        DataExchangeFactory DataExchangeFactory { get; }
    }
}
