using IBatisNetSelf.Common;
using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.DataMapper;
using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using System.Xml;

namespace TestSession
{
    public partial class Form1 : Form
    {

        private static string applicationBase = AppDomain.CurrentDomain.BaseDirectory;
        private static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        ISqlMapper sqlMapper = null;
        private IDataSource dataSource = null;


        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.dataSource = GetDataSource();
            this.label1.Text = dataSource.DbProvider.Name + " 驱动程序已加载";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sqlMapper = new SqlMapper(null,null);
            sqlMapper.DataSource = this.dataSource;
            sqlMapper.OpenConnection();
            sqlMapper.LocalSession.OpenConnection();
            this.label2.Text = $"ID为 {this.sqlMapper.Id} 会话已初始化！";
        }


        private IDataSource? GetDataSource()
        {
            IDataSource _dataSource = new DataSource();
            _dataSource.ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=HIS)));Persist Security Info=True; User ID=hsupgrade;Password=hsupgrade;Pooling=true; Min Pool Size=2; Max Pool Size=5; Decr Pool Size=1;Incr Pool Size=1; Connect Timeout=1000;Validate Connection=false";
            //dataSource.ConnectionString = @"Data Source=his;Persist Security Info=True; User ID=hsupgrade;Password=hsupgrade;Pooling=true; Min Pool Size=2; Max Pool Size=5; Decr Pool Size=1;Incr Pool Size=1; Connect Timeout=1000;Validate Connection=false";
            _dataSource.DbProvider = GetProviders();

            return _dataSource;
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

        private void button3_Click(object sender, EventArgs e)
        {
            ISqlMapSession _sqlMapSession = sqlMapper.LocalSession;

            IDbCommand _command = _sqlMapSession.CreateCommand(CommandType.Text);

            _command.CommandText = "select * from hospital.hosp_";

            IDbDataAdapter _adapter = _sqlMapSession.CreateDataAdapter();
            _adapter.SelectCommand = _command;

            DataSet _ds = new DataSet();

            _adapter.Fill(_ds);

            this.dataGridView1.DataSource = _ds.Tables[0];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ISqlMapSession _sqlMapSession = sqlMapper.LocalSession;

            IDbCommand _command = _sqlMapSession.CreateCommand(CommandType.Text);

            _command.CommandText = "select * from hospital.patient_ where rownum <5";

            IDbDataAdapter _adapter = _sqlMapSession.CreateDataAdapter();
            _adapter.SelectCommand = _command;

            DataSet _ds = new DataSet();

            _adapter.Fill(_ds);

            this.dataGridView1.DataSource = _ds.Tables[0];
        }
    }
}