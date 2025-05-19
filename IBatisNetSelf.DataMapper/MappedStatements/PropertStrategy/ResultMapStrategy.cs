using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PropertStrategy
{
    /// <summary>
    /// <see cref="IPropertyStrategy"/> implementation when a 'resultMapping' attribute exists
    /// on a <see cref="ResultProperty"/>.
    /// </summary>
    public sealed class ResultMapStrategy : BaseStrategy, IPropertyStrategy
    {
        #region IPropertyStrategy Members

        /// <summary>
        /// 当 <see cref="ResultProperty"/> 上存在 'resultMapping' 属性时，
        /// 设置目标对象上对应属性的值。
        /// </summary>
        /// <param name="request">封装当前请求上下文，包括配置、缓存、参数等。</param>
        /// <param name="resultMap">当前对象的结果映射配置。</param>
        /// <param name="mapping">当前正在处理的属性映射。</param>
        /// <param name="target">目标对象（被填充属性值的实例）。</param>
        /// <param name="reader">数据库读取器，读取行数据。</param>
        /// <param name="keys">主键值集合。</param>
        public void Set(RequestScope request, IResultMap resultMap,
            ResultProperty mapping, ref object target, IDataReader reader, object keys)
        {
            // 获取属性的值（可能是嵌套对象）
            object obj = Get(request, resultMap, mapping, ref target, reader);
            // 设置目标对象上的属性值
            resultMap.SetValueOfProperty(ref target, mapping, obj);
        }

        /// <summary>
        /// 获取需要设置在目标对象属性上的值（支持嵌套对象的递归映射）。
        /// </summary>
        /// <param name="request">封装当前映射上下文的信息。</param>
        /// <param name="resultMap">当前的结果映射配置。</param>
        /// <param name="mapping">当前属性映射配置。</param>
        /// <param name="reader">数据读取器，用于读取字段值。</param>
        /// <param name="target">目标对象实例。</param>
        /// <returns>返回要赋值的对象（可为 null）。</returns>
        public object Get(RequestScope request, IResultMap resultMap, ResultProperty mapping, ref object target, IDataReader reader)
        {
            object[] parameters = null;
            bool isParameterFound = false;

            // 获取嵌套映射（子映射），适用于嵌套对象或聚合对象
            IResultMap resultMapping = mapping.NestedResultMap.ResolveSubMap(reader);

            // 如果嵌套对象需要构造函数参数
            if (resultMapping.ConstructorParams.Count > 0)
            {
                parameters = new object[resultMapping.ConstructorParams.Count];
                // 依次读取参数值
                for (int index = 0; index < resultMapping.ConstructorParams.Count; index++)
                {
                    ResultProperty resultProperty = resultMapping.ConstructorParams[index];
                    // 获取构造函数参数的值
                    parameters[index] = resultProperty.ArgumentStrategy.GetValue(request, resultProperty, ref reader, null);
                    // 如果任一参数不为 null，则认为有数据
                    request.IsRowDataFound = request.IsRowDataFound || (parameters[index] != null);
                    isParameterFound = isParameterFound || (parameters[index] != null);
                }
            }

            object obj = null;
            // 如果定义了构造函数参数但都为 null，则表示该对象应为 null（空对象）
            if (resultMapping.ConstructorParams.Count > 0 && isParameterFound == false)
            {
                obj = null;
            }
            else
            {
                // 创建嵌套对象实例（调用构造函数）
                obj = resultMapping.CreateInstanceOfResult(parameters);

                // 将数据填充到嵌套对象中，如果失败（无数据），则将对象设为 null
                if (this.FillObjectWithReaderAndResultMap(request, reader, resultMapping, ref obj) == false)
                {
                    obj = null;
                }
            }

            return obj;
        }
        #endregion
    }
}
