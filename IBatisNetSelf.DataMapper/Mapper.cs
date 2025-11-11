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


    public sealed class Mapper : IMapper, IDisposable
    {
        private readonly object _lock = new();
        // 确保多线程环境下字段的可见性和禁止指令重排序,volatile 是多线程环境下共享字段的 “安全开关”，确保字段的读写操作在多线程间是 “透明” 的
        // volatile 不能替代锁（lock）：它仅解决可见性和重排序问题，无法保证复合操作（如 i++）的原子性
        private volatile ISqlMapper? _sqlMapper;
        private readonly string _resourceName;

        public Mapper()
        {
            _resourceName = string.Empty;
        }

        public Mapper(string resourceName)
        {
            _resourceName = resourceName ?? string.Empty;
        }

        public Mapper(IOptions<MapperOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _resourceName = options.Value.SqlMapConfig ?? string.Empty;
        }

        public ISqlMapper SqlMapper
        {
            get
            {
                //双重检查锁定（double-checked locking）保证线程安全
                if (_sqlMapper == null)// 第一次检查（无锁，提高性能）
                {
                    lock (_lock)// 加锁
                    {
                        if (_sqlMapper == null)// 第二次检查（确保只初始化一次）
                        {
                            try
                            {
                                var configureHandler = new ConfigureHandler(Configure);
                                var sqlMapBuilder = new DomSqlMapBuilder();

                                _sqlMapper = string.IsNullOrEmpty(_resourceName)
                                    ? sqlMapBuilder.ConfigureAndWatch(configureHandler)
                                    : sqlMapBuilder.ConfigureAndWatch(_resourceName, configureHandler);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException("Failed to initialize SqlMapper", ex);
                            }
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

        public void Dispose()
        {
            if (_sqlMapper is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _sqlMapper = null;
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

        public static IServiceCollection AddIBatisNet(this IServiceCollection services, string sqlMapConfigPath)
        {
            services.AddSingleton<IMapper>(new Mapper(sqlMapConfigPath));
            return services;
        }
    }
}

