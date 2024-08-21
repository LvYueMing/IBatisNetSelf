using IBatisNetSelf.Common.Xml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.Common
{
    /// <summary>
    /// Summary description for DataSourceDeSerializer.
    /// </summary>
    public sealed class DataSourceDeSerializer
    {
        /// <summary>
        /// Deserialize a DataSource object
        /// </summary>
        /// <param name="aNode"></param>
        /// <returns></returns>
        public static DataSource Deserialize(XmlNode aNode)
        {
            DataSource _dataSource = new DataSource();
            NameValueCollection _prop = XmlNodeUtils.ParseAttributes(aNode);

            _dataSource.ConnectionString = _prop["connectionString"];
            _dataSource.Name = _prop["name"];

            return _dataSource;
        }
    }
}
