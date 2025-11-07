using IBatisNetSelf.DataMapper;
using IBatisNetSelf.DataMapper.Configuration;
using System.Collections;
using System.Data;
using System.Xml.Linq;

namespace ResultMap
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DomSqlMapBuilder _sqlMapBuilder = new DomSqlMapBuilder();
            //不验证 配置文件格式
            _sqlMapBuilder.ValidateSqlMapConfig = false;
            ISqlMapper _sqlMapper = _sqlMapBuilder.Configure();

            Hashtable ht = new Hashtable();
            //ht.Add("in_id", "DE01.00.010.00");
            //DataSet ds = _sqlMapper.QueryForDataSet("Element_GetELement", ht);
            //var element = _sqlMapper.QueryForObject("Element_GetELement1", ht) as Element;

            //var element = _sqlMapper.QueryForObject<Element>("Element_GetELement", ht); 

            //var element = _sqlMapper.QueryForList<Element>("Element_GetAllELement", ht);

            ht.Add("in_ownerid", "H1.168139.1");
            ht.Add("in_hospid", "H1");
            var element = _sqlMapper.QueryForList<OrderList>("Order_GetOrderList", ht);

            Console.Read();
        }
    }
}
