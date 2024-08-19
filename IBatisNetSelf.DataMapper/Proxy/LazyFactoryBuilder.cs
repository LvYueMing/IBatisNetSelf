using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Proxy
{
    /// <summary>
    /// Gets <see cref="ILazyFactory"/> instance.
    /// </summary>
    public class LazyFactoryBuilder
    {
        private IDictionary factory = new HybridDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyFactoryBuilder"/> class.
        /// </summary>
        public LazyFactoryBuilder()
        {
            factory[typeof(IList)] = new LazyListFactory();
        }


        /// <summary>
        /// Register (add) a lazy load Proxy for a type and member type
        /// </summary>
        /// <param name="type">The target type which contains the member proxyfied</param>
        /// <param name="memberName">The member name the proxy must emulate</param>
        /// <param name="factory">The <see cref="ILazyFactory"/>.</param>
        public void Register(Type type, string memberName, ILazyFactory factory)
        {
            // To use for further used, support for custom proxy
        }

        /// <summary>
        /// Get a ILazyLoadProxy for a type, member name
        /// </summary>
        /// <param name="type">The target type which contains the member proxyfied</param>
        /// <returns>Return the ILazyLoadProxy instance</returns>
        public ILazyFactory GetLazyFactory(Type type)
        {
            if (type.IsInterface)
            {
                if (type == typeof(IList))
                {
                    return factory[type] as ILazyFactory;
                }
                else
                {
                    throw new DataMapperException("Cannot proxy others interfaces than IList or IList<>.");
                }
            }
            else
            {
                // if you want to proxy concrete classes, there are also two requirements: 
                // the class can not be sealed and only virtual methods can be intercepted. 
                // The reason is that DynamicProxy will create a subclass of your class overriding all methods 
                // so it can dispatch the invocations to the interceptor.
                return new LazyLoadProxyFactory();
            }
        }
    }
}
