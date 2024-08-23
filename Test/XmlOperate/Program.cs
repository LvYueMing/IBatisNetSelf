using IBatisNetSelf.Common.Utilities;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Schema;

namespace XmlOperate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XmlDocument _xmlDoc = Resources.GetResourceAsXmlDocument("SqlMap.config");

            XmlNamespaceManager _xmlNS = new XmlNamespaceManager(_xmlDoc.NameTable);
            _xmlNS.AddNamespace("mapper", "http://ibatis.apache.org/dataMapper");
            _xmlNS.AddNamespace("provider", "http://ibatis.apache.org/providers");
            _xmlNS.AddNamespace("mapping", "http://ibatis.apache.org/mapping");

            ValidateSchema(_xmlDoc.ChildNodes[1]);

        }


        private static void ValidateSchema(XmlNode aSection)
        {

            XmlReader _xmlReader = null;
            FileStream _xsdStream = null;

            try
            {
                //Validate the document using a schema
                string _schemaFileName = AppDomain.CurrentDomain.BaseDirectory + "SqlMapConfig.xsd";
                _xsdStream = new FileStream(_schemaFileName, FileMode.Open, FileAccess.Read);

                if (_xsdStream == null)
                {
                    // TODO: avoid using hard-coded value "IBatisNet.DataMapper"
                    throw new ConfigurationException("Unable to locate embedded resource [IBatisNet.DataMapper." + _schemaFileName + "]. If you are building from source, verfiy the file is marked as an embedded resource.");
                }

                XmlSchema _schema = XmlSchema.Read(_xsdStream, new ValidationEventHandler(ValidationCallBack));

                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.ValidationType= ValidationType.Schema;
                readerSettings.ValidationEventHandler += ValidationCallBack;
                readerSettings.Schemas.Add(_schema);

                _xmlReader = XmlReader.Create(new XmlTextReader(new StringReader(aSection.OuterXml)), readerSettings);
                
                // Validate the document
                while (_xmlReader.Read()) { }

            }
            finally
            {
                if (_xmlReader != null) _xmlReader.Close();
                if (_xsdStream != null) _xsdStream.Close();
            }
        }


        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

    }
}