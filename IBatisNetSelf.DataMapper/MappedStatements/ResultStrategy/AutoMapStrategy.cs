using IBatisNetSelf.DataMapper.Configuration.ResultMapping;
using IBatisNetSelf.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy
{
    /// <summary>
    /// `AutoMapStrategy` 是一个 `IResultStrategy` 实现，
    /// 当映射文件未显式声明 `<resultMap>` 时，它会自动根据查询结果集字段名与目标对象属性名进行映射。
    /// </summary>
    public sealed class AutoMapStrategy : IResultStrategy
    {
        /// <summary>
        /// 初始化 AutoResultMap，用于将 reader 中的数据自动映射到结果对象。
        /// </summary>
        /// <param name="aRequest">当前请求上下文</param>
        /// <param name="aReader">数据库读取器</param>
        /// <param name="aResultObject">输出结果对象（可能是 null）</param>
        /// <returns>构建好的 AutoResultMap</returns>
        private AutoResultMap InitializeAutoResultMap(RequestScope aRequest, ref IDataReader aReader, ref object aResultObject)
        {
            // 获取当前的 ResultMap，并转换为 AutoResultMap 类型
            AutoResultMap _resultMap = aRequest.CurrentResultMap as AutoResultMap;

            // 如果配置允许每次都重新映射（常用于动态 SQL）
            if (aRequest.Statement.AllowRemapping)
            {
                // 克隆出一个新的 AutoResultMap，避免线程冲突
                _resultMap = _resultMap.Clone();

                // 构建属性集合（从 reader 映射到 resultObject 的属性集合）
                ResultPropertyCollection _properties = ReaderAutoMapper.Build(
                    aRequest.DataExchangeFactory,
                    aReader,
                    ref aResultObject);

                // 添加到 AutoResultMap 中
                _resultMap.Properties.AddRange(_properties);
            }
            else
            {
                // 如果未初始化过 AutoResultMap，则进行线程安全的初始化
                if (!_resultMap.IsInitalized)
                {
                    lock (_resultMap)
                    {
                        if (!_resultMap.IsInitalized)
                        {
                            ResultPropertyCollection _properties = ReaderAutoMapper.Build(
                               aRequest.DataExchangeFactory,
                               aReader,
                               ref aResultObject);

                            _resultMap.Properties.AddRange(_properties);
                            _resultMap.IsInitalized = true;// 标记已初始化
                        }
                    }
                }

            }

            return _resultMap;
        }


        #region IResultStrategy Members

        /// <summary>
        /// 使用自动映射策略从 DataReader 生成结果对象。
        /// </summary>
        /// <param name="aRequest">请求上下文</param>
        /// <param name="aReader">数据读取器</param>
        /// <param name="aResultObject">输入或输出结果对象</param>
        /// <returns>完整填充后的结果对象</returns>
        public object Process(RequestScope aRequest, ref IDataReader aReader, object aResultObject)
        {
            // 如果 resultObject 为 null，则创建一个新的对象实例（用于接收映射数据）
            object _outObject = aResultObject;

            if (_outObject == null)
            {
                _outObject = (aRequest.CurrentResultMap as AutoResultMap).CreateInstanceOfResultClass();
            }

            // 初始化自动映射配置
            AutoResultMap _resultMap = InitializeAutoResultMap(aRequest, ref aReader, ref _outObject);

            // 遍历所有自动识别出的属性，逐个从 reader 中读取值，并赋值到结果对象的对应属性上
            for (int index = 0; index < _resultMap.Properties.Count; index++)
            {
                ResultProperty _property = _resultMap.Properties[index];
                _resultMap.SetValueOfProperty(ref _outObject, _property, _property.GetDataBaseValue(aReader));
            }

            return _outObject;
        }

        #endregion
    }
}
