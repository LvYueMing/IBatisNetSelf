using IBatisNetSelf.DataMapper;
using IBatisNetSelf.DataMapper.Configuration;
using System.Collections;

namespace ParseSelectSql
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DomSqlMapBuilder _sqlMapBuilder = new DomSqlMapBuilder();
            //不验证 配置文件格式
            _sqlMapBuilder.ValidateSqlMapConfig = false;
            ISqlMapper _sqlMapper = _sqlMapBuilder.Configure();

            Hashtable param = new Hashtable();
            param.Add("in_patient_id", "000023");
            IList list = _sqlMapper.QueryForList("Test_GetInPatient", param);
        }
    }
}