using IBatisNetSelf.DataMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MapperDITests
{
    public class MapperTests
    {
        // 测试无效配置路径
        [Fact]
        public void SqlMapper_WithInvalidConfig_ShouldThrow()
        {
            // 模拟无效配置路径（假设文件不存在时会抛异常）
            var mapper = new Mapper("invalid.config");

            // 验证初始化时抛出异常
            Assert.Throws<InvalidOperationException>(() => mapper.SqlMapper);
        }

        // 测试默认配置
        [Fact]
        public void TestDefaultConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddIBatisNet(new ConfigurationBuilder().Build());

            // Act
            using var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            // Assert
            Assert.NotNull(mapper.SqlMapper);
        }

        // 测试配置文件路径
        [Fact]
        public void TestCustomConfigurationPath()
        {
            // 从内存加载
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] {
                new KeyValuePair<string, string>("IBatis:SqlMapConfig", "SqlMap.config")
                })
                .Build();

            var services = new ServiceCollection();
            services.AddIBatisNet(config);

            // Act
            using var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            // Assert
            Assert.NotNull(mapper.SqlMapper);
        }

        // 测试配置文件路径
        [Fact]
        public void TestCustomConfigurationFile()
        {
            // 从文件加载
            // 创建配置构建器
            IConfigurationBuilder builder = new ConfigurationBuilder()
                // 设置基础路径（通常是项目根目录）
                .SetBasePath(Directory.GetCurrentDirectory())
                // 添加 JSON 配置文件（支持可选参数，如是否可选、是否 reloadOnChange）
                .AddJsonFile("Ibatis.json", optional: false, reloadOnChange: true);

            // 构建配置对象
            IConfiguration config = builder.Build();

            var services = new ServiceCollection();
            services.AddIBatisNet(config);

            // Act
            using var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            // Assert
            Assert.NotNull(mapper.SqlMapper);
        }


        // 测试直接传入路径
        [Fact]
        public void TestDirectPathConfiguration()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddIBatisNet("SqlMap.config");

            // Act
            using var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            // Assert
            Assert.NotNull(mapper.SqlMapper);
        }

        // 资源释放测试
        [Fact]
        public void Dispose_ShouldReleaseResources()
        {
            // 测试释放资源（假设 ISqlMapper 实现了 IDisposable）
            var mapper = new Mapper() as Mapper;
            Assert.NotNull(mapper);

            var sqlMapper = mapper.SqlMapper;
            mapper.Dispose();

            // 验证 _sqlMapper 被释放并置空
            var sqlMapperField = typeof(Mapper).GetField("_sqlMapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Null(sqlMapperField?.GetValue(mapper));
        }


        // 延迟初始化测试
        [Fact]
        public void SqlMapper_ShouldBeInitializedLazily()
        {
            // 测试延迟初始化（未访问时为 null）
            var mapper = new Mapper();
            // 注意：由于 _sqlMapper 是私有字段，可通过反射或行为验证
            var sqlMapperField = typeof(Mapper).GetField("_sqlMapper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.Null(sqlMapperField?.GetValue(mapper));

            // 访问后应初始化
            var sqlMapper = mapper.SqlMapper;
            Assert.NotNull(sqlMapper);
            Assert.Same(sqlMapper, sqlMapperField?.GetValue(mapper));
        }

        // 线程安全测试
        [Fact]
        public void SqlMapper_ShouldBeThreadSafe()
        {
            // 多线程并发访问，验证实例唯一
            var services = new ServiceCollection();
            services.AddIBatisNet(new ConfigurationBuilder().Build());
            using var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            ISqlMapper? instance = null;
            var tasks = new Task[100];  // 100 个并发任务

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var current = mapper.SqlMapper;
                    // 首次赋值，后续验证是否与首次相同
                    if (instance == null)
                        instance = current;
                    else
                        Assert.Same(instance, current);
                });
            }

            Task.WaitAll(tasks);
            Assert.NotNull(instance);
        }
    }
}
