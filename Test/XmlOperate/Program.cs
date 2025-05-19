using IBatisNetSelf.Common.Utilities;
using IBatisNetSelf.Common.Exceptions;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XmlOperate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XmlDocument _xmlDoc = Resources.GetResourceAsXmlDocument("SqlMap.config");

            XmlNamespaceManager _xmlNameSpace = new XmlNamespaceManager(_xmlDoc.NameTable);
            _xmlNameSpace.AddNamespace("mapper", "http://ibatis.apache.org/dataMapper");
            _xmlNameSpace.AddNamespace("provider", "http://ibatis.apache.org/providers");
            _xmlNameSpace.AddNamespace("mapping", "http://ibatis.apache.org/mapping");

            ValidateSchema(_xmlDoc.ChildNodes[1]);

            XmlNode _node = _xmlDoc.SelectSingleNode("mapper:sqlMapConfig", _xmlNameSpace);

            XmlNode _node1 = _node.SelectSingleNode("mapper:properties", _xmlNameSpace);

            if (_node1.HasChildNodes)
            {
                Console.WriteLine($"子节点");
                foreach (XmlNode _node2 in _node1.SelectNodes("mapper:property", _xmlNameSpace))
                {

                    XmlAttribute _keyAttr = _node2.Attributes["key"];
                    XmlAttribute _valueAttr = _node2.Attributes["value"];
                    if (_keyAttr != null && _valueAttr != null)
                    {
                        Console.WriteLine($"Add property \"{_keyAttr.Value}\" value \"{_valueAttr.Value}\"");
                    }
                    else
                    {
                        XmlDocument _propertiesConfig = Resources.GetSubfileAsXmlDocument(_node2, new System.Collections.Specialized.NameValueCollection());

                        foreach (XmlNode _node3 in _propertiesConfig.SelectNodes("*/add", _xmlNameSpace))
                        {
                            Console.WriteLine($"Add property \"{_node3.Attributes["key"].Value}\" value \"{_node3.Attributes["value"].Value}\"");
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine($"没有子节点");
                XmlDocument _propertiesConfig = Resources.GetSubfileAsXmlDocument(_node1,new System.Collections.Specialized.NameValueCollection());

                foreach (XmlNode _node3 in _propertiesConfig.SelectNodes("*/add", _xmlNameSpace))
                {
                    Console.WriteLine($"Add property \"{_node3.Attributes["key"].Value}\" value \"{_node3.Attributes["value"].Value}\"");
                }
            }

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
                    throw new IBatisConfigException("Unable to locate embedded resource [IBatisNet.DataMapper." + _schemaFileName + "]. If you are building from source, verfiy the file is marked as an embedded resource.");
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