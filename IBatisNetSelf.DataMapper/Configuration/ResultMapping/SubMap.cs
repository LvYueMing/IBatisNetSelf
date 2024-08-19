using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.ResultMapping
{
    /// <summary>
    /// Summary description for SubMap.
    /// </summary>
    [Serializable]
    [XmlRoot("subMap", Namespace = "http://ibatis.apache.org/mapping")]
    public class SubMap
    {
        // <resultMap id="document" class="Document">
        //			<result property="Id" column="Document_ID"/>
        //			<result property="Title" column="Document_Title"/>
        //			<discriminator column="Document_Type" [formula="CustomFormula, AssemblyName"] /> 
        //						-- attribute column (not used if discriminator use a custom formula)
        //						-- attribute formula (not required will used the DefaultFormula) calculate the discriminator value (DefaultFormula is default), else used an aliasType wich implement IDiscriminatorFormula), 
        //			<subMap value="Book" -- discriminator value
        //					resultMapping="book" />
        //	</resultMap>
        //
        //  <resultMap 
        //		id="book"  
        //		class="Book"
        //		extend="document">
        //  ...
        // </resultMap>

        #region Fields
        [NonSerialized]
        private string discriminatorValue = string.Empty;
        [NonSerialized]
        private string resultMapName = string.Empty;
        [NonSerialized]
        private IResultMap resultMap = null;
        #endregion

        #region Properties

        /// <summary>
        /// Discriminator value
        /// </summary>
        [XmlAttribute("value")]
        public string DiscriminatorValue
        {
            get { return discriminatorValue; }
        }

        /// <summary>
        /// The name of the ResultMap used if the column value is = to the Discriminator Value
        /// </summary>
        [XmlAttribute("resultMapping")]
        public string ResultMapName
        {
            get { return resultMapName; }
        }

        /// <summary>
        /// The resultMap used if the column value is = to the Discriminator Value
        /// </summary>
        [XmlIgnore]
        public IResultMap ResultMap
        {
            get { return resultMap; }
            set { resultMap = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubMap"/> class.
        /// </summary>
        /// <param name="discriminatorValue">The discriminator value.</param>
        /// <param name="resultMapName">Name of the result map.</param>
        public SubMap(string discriminatorValue, string resultMapName)
        {
            this.discriminatorValue = discriminatorValue;
            this.resultMapName = resultMapName;
        }
        #endregion

    }
}
