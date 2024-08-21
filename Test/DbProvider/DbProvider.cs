using IBatisNetSelf.Common;
using System.Data;
using System.Data.Common;
using System.Xml;

namespace Test
{
    public partial class DBpriver : Form
    {
        private static string applicationBase = AppDomain.CurrentDomain.BaseDirectory;
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private DataSource dataSource = new DataSource();
        public DBpriver()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataSource.ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=HIS)));Persist Security Info=True; User ID=hsupgrade;Password=hsupgrade;Pooling=true; Min Pool Size=2; Max Pool Size=5; Decr Pool Size=1;Incr Pool Size=1; Connect Timeout=1000;Validate Connection=false";
            //dataSource.ConnectionString = @"Data Source=his;Persist Security Info=True; User ID=hsupgrade;Password=hsupgrade;Pooling=true; Min Pool Size=2; Max Pool Size=5; Decr Pool Size=1;Incr Pool Size=1; Connect Timeout=1000;Validate Connection=false";
            dataSource.DbProvider = GetProviders();

            this.label1.Text = dataSource.DbProvider.Name + " 驱动程序已加载";

        }


        private IDbProvider? GetProviders()
        {
            XmlDocument _xmlDoc = new XmlDocument();
            _xmlDoc.Load(applicationBase + "\\Provider.config");

            XmlNodeList _nodes = _xmlDoc.SelectNodes("providers/provider");

            IDbProvider _provider = null;
            foreach (XmlNode _node in _nodes)
            {
                _provider = ProviderDeSerializer.Deserialize(_node);
                if (_provider.IsEnabled)
                {
                    _provider.Initialize();
                }
            }
            return _provider;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IDbConnection _connet = dataSource.DbProvider.CreateConnection();
            _connet.ConnectionString = dataSource.ConnectionString;
            _connet.Open();

            IDbCommand _command = dataSource.DbProvider.CreateCommand();
            _command.Connection = _connet;
            _command.CommandType = CommandType.Text;
            _command.CommandText = "select * from hospital.hosp_";

            IDbDataAdapter _adapter = dataSource.DbProvider.CreateDataAdapter();
            _adapter.SelectCommand = _command;

            DataSet _ds = new DataSet();

            _adapter.Fill(_ds);

            this.dataGridView1.DataSource = _ds.Tables[0];

        }

    }
}