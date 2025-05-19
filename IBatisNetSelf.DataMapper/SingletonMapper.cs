using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    /// <summary>
    /// A singleton class to access the default SqlMapper defined by the SqlMap.Config
    /// </summary>
    public sealed class SingletonMapper
    {
        #region Fields
        private static volatile ISqlMapper sqlMapper = null;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public static void Configure(object obj)
        {
            sqlMapper = null;
        }

        /// <summary>
        /// Init the 'default' SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        public static void InitMapper()
        {
            ConfigureHandler _handler = new ConfigureHandler(Configure);
            DomSqlMapBuilder _builder = new DomSqlMapBuilder();
            sqlMapper = _builder.ConfigureAndWatch(_handler);
        }

        /// <summary>
        /// Get the instance of the SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        /// <returns>A SqlMapper initalized via the SqlMap.Config file.</returns>
        public static ISqlMapper Instance()
        {
            if (sqlMapper == null)
            {
                lock (typeof(SqlMapper))
                {
                    if (sqlMapper == null) // double-check
                    {
                        InitMapper();
                    }
                }
            }
            return sqlMapper;
        }

        /// <summary>
        /// Get the instance of the SqlMapper defined by the SqlMap.Config file. (Convenience form of Instance method.)
        /// </summary>
        /// <returns>A SqlMapper initalized via the SqlMap.Config file.</returns>
        public static ISqlMapper Get()
        {
            return Instance();
        }
    }
}
