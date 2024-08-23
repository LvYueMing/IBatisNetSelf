using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Represent a store Procedure.
    /// </summary>
    [Serializable]
    [XmlRoot("procedure", Namespace = "http://ibatis.apache.org/mapping")]
    public class Procedure : Statement
    {

        #region Properties
        /// <summary>
        /// The type of the statement StoredProcedure.
        /// </summary>
        [XmlIgnore]
        public override CommandType CommandType
        {
            get { return CommandType.StoredProcedure; }
        }

        /// <summary>
        /// Extend statement attribute
        /// </summary>
        [XmlIgnore]
        public override string ExtendStatement
        {
            get { return string.Empty; }
            set { }
        }
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// Do not use direclty, only for serialization.
        /// </summary>
        public Procedure() : base()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aConfigurationScope">The scope of the configuration</param>
        override internal void Initialize(ConfigurationScope aConfigurationScope)
        {
            base.Initialize(aConfigurationScope);
            if (this.ParameterMap == null)
            {
                //throw new ConfigurationException("The parameterMap attribute is required in the procedure tag named '"+ this.Id +"'.");
                this.ParameterMap = aConfigurationScope.SqlMapper.GetParameterMap(ConfigurationScope.EMPTY_PARAMETER_MAP);
            }
        }
        #endregion

    }
}
