using IBatisNetSelf.DataMapper.Configuration;

namespace DomSqlMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DomSqlMapBuilder _sqlMapBuilder= new DomSqlMapBuilder();
            //不验证 配置文件格式
            _sqlMapBuilder.ValidateSqlMapConfig = false;
            _sqlMapBuilder.Configure();
        }
    }
}