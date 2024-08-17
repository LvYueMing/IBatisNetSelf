using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.Common
{
    /// <summary>
    /// Information about a data source.
    /// </summary>
    [Serializable]
    [XmlRoot("dataSource", Namespace = "http://ibatis.apache.org/dataMapper")]
    public class DataSource : IDataSource
    {
        #region Fields

        [NonSerialized]
        private string name = string.Empty;
        [NonSerialized]
        private string connectionString = string.Empty;
        [NonSerialized]
        private IDbProvider provider;

        #endregion

        #region Properties
        /// <summary>
        /// DataSource Name
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return this.name; }
            set
            {
                CheckPropertyString("Name", value);
                this.name = value;
            }
        }
        /// <summary>
        /// The connection string.
        /// </summary>
        [XmlAttribute("connectionString")]
        public virtual string ConnectionString
        {
            get { return this.connectionString; }
            set
            {
                this.CheckPropertyString("ConnectionString", value);
                this.connectionString = value;
            }
        }


        /// <summary>
        /// The provider to use for this data source.
        /// </summary>
        [XmlIgnore]
        public virtual IDbProvider DbProvider
        {
            get { return this.provider; }
            set { this.provider = value; }
        }
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataSource()
        {
        }
        #endregion

        #region Methods

        private void CheckPropertyString(string propertyName, string value)
        {
            if (value == null || value.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "The " + propertyName + " property cannot be " +
                    "set to a null or empty string value.", propertyName);
            }
        }

        /// <summary>
        /// ToString implementation.
        /// </summary>
        /// <returns>A string that describes the data source</returns>
        public override string ToString()
        {
            return "Source: ConnectionString : " + ConnectionString;
        }
        #endregion
    }
}
