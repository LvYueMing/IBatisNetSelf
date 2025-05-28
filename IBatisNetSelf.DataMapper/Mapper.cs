using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.DataMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

    //{
    //  "IBatis": {
    //    "SqlMapConfig": "SqlMap.config"  // 你的 SQL Map 配置文件路径
    //  }
    //}
    public class MapperOptions
    {
        public string SqlMapConfig { get; set; } = "SqlMap.config";
    }


    public sealed class Mapper : IMapper
    {
        private readonly object _lock = new();
        private ISqlMapper? _sqlMapper;
        private readonly string? _resourceName;

        public Mapper()
        {
            _resourceName = null;
        }

        public Mapper(IOptions<MapperOptions> options)
        {
            _resourceName = options.Value.SqlMapConfig;
        }

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

        private void Configure(object obj)
        {
            lock (_lock)
            {
                _sqlMapper = null;
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIBatisNet(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MapperOptions>(configuration.GetSection("IBatis"));
            services.AddSingleton<IMapper, Mapper>();
            return services;
        }
    }
}

