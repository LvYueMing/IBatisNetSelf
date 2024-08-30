using IBatisNetSelf.Common;
using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper;
using IBatisNetSelf.DataMapper.Configuration;
using IBatisNetSelf.DataMapper.Configuration.Serializers;
using IBatisNetSelf.DataMapper.Configuration.Statements;
using IBatisNetSelf.DataMapper.MappedStatements;
using IBatisNetSelf.DataMapper.Scope;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;

namespace DomSqlMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DomSqlMapBuilder _sqlMapBuilder = new DomSqlMapBuilder();
            //不验证 配置文件格式
            _sqlMapBuilder.ValidateSqlMapConfig = false;
            _sqlMapBuilder.Configure();

        }
    }
}