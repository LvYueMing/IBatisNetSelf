using IBatisNetSelf.Common.Logging;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.MappedStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Proxy
{
    /// <summary>
    /// This class is responsible of create lazy load proxies for a concrete class with virtual method.
    /// </summary>
    public class LazyLoadProxyFactory : ILazyFactory
    {
        #region Fields
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region ILazyFactory Members
        /// <summary>
        /// Builds the specified lazy load proxy for a concrete class with virtual method.
        /// </summary>
        /// <param name="selectStatement">The mapped statement used to build the lazy loaded object.</param>
        /// <param name="param">The parameter object used to build lazy loaded object.</param>
        /// <param name="target">The target object which contains the property proxydied..</param>
        /// <param name="setAccessor">The proxified member accessor.</param>
        /// <returns>Return a proxy object</returns>
        public object CreateProxy(IMappedStatement selectStatement, object param,
            object target, ISetAccessor setAccessor)
        {
            //Type typeProxified = setAccessor.MemberType;

            //if (_logger.IsDebugEnabled)
            //{
            //    _logger.Debug(string.Format("Statement '{0}', create proxy for member {1}.", selectStatement.Id, setAccessor.MemberType));
            //}

            //// Build the proxy
            //IInterceptor handler = new LazyLoadInterceptor(selectStatement, param, target, setAccessor);

            //// if you want to proxy concrete classes, there are also 2 main requirements : 
            //// the class can not be sealed and only virtual methods can be intercepted. 
            //// The reason is that DynamicProxy will create a subclass of your class overriding all methods 
            //// so it can dispatch the invocations to the interceptor.

            //// The proxified type must also have an empty constructor
            //object proxy = ProxyGeneratorFactory.GetProxyGenerator().CreateClassProxy(typeProxified, handler, Type.EmptyTypes);

            //return proxy;
            return null;
        }

        #endregion
    }
}
