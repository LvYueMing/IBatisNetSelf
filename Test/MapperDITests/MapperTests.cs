using IBatisNetSelf.DataMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MapperDITests
{
    public class MapperTests
    {
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
    }
}
