using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper
{
    public interface IMapper
    {
        ISqlMapper SqlMapper { get; }
    }


    public sealed class Mapper : IMapper
    {
        private readonly object _lock = new();
        private ISqlMapper? _sqlMapper;
        private readonly string? _resourceName;

        public ISqlMapper SqlMapper
        {
            get
            {
                if (_sqlMapper == null)
                {
                    lock (_lock)
                    {
                        if (_sqlMapper == null)
                        {
                            var configureHandler = new ConfigureHandler(Configure);
                            var sqlMapBuilder = new DomSqlMapBuilder();

                            _sqlMapper = string.IsNullOrEmpty(_resourceName)
                                ? sqlMapBuilder.ConfigureAndWatch(configureHandler)
                                : sqlMapBuilder.ConfigureAndWatch(_resourceName, configureHandler);
                        }
                    }
                }

                return _sqlMapper!;
            }
        }

        public Mapper()
        {
            _resourceName = null;
        }

        public Mapper(string aResource)
        {
            _resourceName = aResource;
        }

        private void Configure(object obj)
        {
            lock (_lock)
            {
                _sqlMapper = null;
            }
        }
    }
}

