using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Commands
{
    /// <summary>
    /// Summary description for PreparedCommandFactory.
    /// </summary>
    internal sealed class PreparedCommandFactory
    {
        /// <summary>
        /// Get an IPreparedCommand.
        /// </summary>
        /// <returns></returns>
        static public IPreparedCommand GetPreparedCommand(bool isEmbedStatementParams)
        {
            IPreparedCommand _preparedCommand;

            //			if (isEmbedStatementParams)
            //			{
            //				preparedCommand = new EmbedParamsPreparedCommand();
            //			}
            //			else
            //			{
            _preparedCommand = new DefaultPreparedCommand();
            //			}

            return _preparedCommand;
        }

    }
}
