using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Serializers
{
    /// <summary>
    /// Summary description for DeSerializerFactory.
    /// </summary>
    public class DeSerializerFactory
    {
        private IDictionary serializerMap = new HybridDictionary();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aConfigScope"></param>
        public DeSerializerFactory(ConfigurationScope aConfigScope)
        {
            serializerMap.Add("dynamic", new DynamicDeSerializer(aConfigScope));
            serializerMap.Add("isEqual", new IsEqualDeSerializer(aConfigScope));
            serializerMap.Add("isNotEqual", new IsNotEqualDeSerializer(aConfigScope));
            serializerMap.Add("isGreaterEqual", new IsGreaterEqualDeSerializer(aConfigScope));
            serializerMap.Add("isGreaterThan", new IsGreaterThanDeSerializer(aConfigScope));
            serializerMap.Add("isLessEqual", new IsLessEqualDeSerializer(aConfigScope));
            serializerMap.Add("isLessThan", new IsLessThanDeSerializer(aConfigScope));
            serializerMap.Add("isNotEmpty", new IsNotEmptyDeSerializer(aConfigScope));
            serializerMap.Add("isEmpty", new IsEmptyDeSerializer(aConfigScope));
            serializerMap.Add("isNotNull", new IsNotNullDeSerializer(aConfigScope));
            serializerMap.Add("isNotParameterPresent", new IsNotParameterPresentDeSerializer(aConfigScope));
            serializerMap.Add("isNotPropertyAvailable", new IsNotPropertyAvailableDeSerializer(aConfigScope));
            serializerMap.Add("isNull", new IsNullDeSerializer(aConfigScope));
            serializerMap.Add("isParameterPresent", new IsParameterPresentDeSerializer(aConfigScope));
            serializerMap.Add("isPropertyAvailable", new IsPropertyAvailableDeSerializer(aConfigScope));
            serializerMap.Add("iterate", new IterateSerializer(aConfigScope));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDeSerializer GetDeSerializer(string name)
        {
            return (IDeSerializer)serializerMap[name];
        }

    }
}
