using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IBatisNetSelf.Common.Xml
{
    /// <summary>
    /// Summary description for XmlNodeUtils.
    /// </summary>
    public sealed class XmlNodeUtils
    {
        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="aAttributes"></param>
        /// <param name="aName">The key</param>
        /// <returns></returns>
        public static string GetStringAttribute(NameValueCollection aAttributes, string aName)
        {
            string? _value = aAttributes[aName];
            if (_value == null)
            {
                return string.Empty;
            }
            else
            {
                return _value;
            }
        }

        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name">The key</param>
        /// <param name="defaultValue">The default value to be returned if the attribute is not found.</param>
        /// <returns></returns>
        public static string GetStringAttribute(NameValueCollection attributes, string name, string defaultValue)
        {
            string? value = attributes[name];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name">The key</param>
        /// <param name="defaultValue">The default value to be returned if the attribute is not found.</param>
        /// <returns></returns>
        public static byte GetByteAttribute(NameValueCollection attributes, string name, byte defaultValue)
        {
            string? value = attributes[name];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return XmlConvert.ToByte(value);
            }
        }

        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name">The key</param>
        /// <param name="defaultValue">The default value to be returned if the attribute is not found.</param>
        /// <returns></returns>
        public static int GetIntAttribute(NameValueCollection attributes, string name, int defaultValue)
        {
            string? value = attributes[name];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return XmlConvert.ToInt32(value);
            }
        }

        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="aAttributes"></param>
        /// <param name="aName">The key</param>
        /// <param name="defaultValue">The default value to be returned if the attribute is not found.</param>
        /// <returns></returns>
        public static bool GetBooleanAttribute(NameValueCollection aAttributes, string aName, bool defaultValue)
        {
            string? value = aAttributes[aName];
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return XmlConvert.ToBoolean(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static NameValueCollection ParseAttributes(XmlNode node)
        {
            return ParseAttributes(node, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aXmlNode"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static NameValueCollection ParseAttributes(XmlNode aXmlNode, NameValueCollection? variables)
        {
            NameValueCollection _attributes = new NameValueCollection();
            if (aXmlNode.Attributes != null)
            {
                foreach (XmlAttribute _attribute in aXmlNode.Attributes)
                {
                    string _value = ParsePropertyTokens(_attribute.Value, variables);
                    _attributes.Add(_attribute.Name, _value);
                }
            }

            return _attributes;
        }


        /// <summary>
        /// Replace properties by their values in the given string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="aProperties"></param>
        /// exmple: "User ID=${hospital};Password=${source}"
        /// 第一次: _prepend   = "User ID=" 
        ///        _append    = ";Password=${source}" 
        ///        _propName  = "User" 
        ///        _propValue = "hospital_value"
        ///        _newString = "User ID=hospital_value;Password=${source}"
        /// return: "User ID=hospital_value;Password=source_value"
        /// <returns></returns>
        public static string ParsePropertyTokens(string str, NameValueCollection? aProperties)
        {
            string OPEN = "${";
            string CLOSE = "}";

            string _newString = str;
            if (_newString != null && aProperties != null)
            {
                int _startIndex = _newString.IndexOf(OPEN);
                int _endIndex = _newString.IndexOf(CLOSE);

                while (_startIndex > -1 && _endIndex > _startIndex)
                {
                    string _prepend = _newString.Substring(0, _startIndex);
                    string _append = _newString.Substring(_endIndex + CLOSE.Length);

                    int index = _startIndex + OPEN.Length;
                    string _propName = _newString.Substring(index, _endIndex - index);
                    string _propValue = aProperties.Get(_propName);
                    if (_propValue == null)
                    {
                        _newString = _prepend + _propName + _append;
                    }
                    else
                    {
                        _newString = _prepend + _propValue + _append;
                    }
                    _startIndex = _newString.IndexOf(OPEN);
                    _endIndex = _newString.IndexOf(CLOSE);
                }
            }
            return _newString;
        }
    }
}
