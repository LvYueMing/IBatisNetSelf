using IBatisNetSelf.DataMapper.MappedStatements.ResultStrategy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.MappedStatements.PostSelectStrategy
{
    /// <summary>
    /// Factory to get <see cref="IPostSelectStrategy"/> implementation.
    /// </summary>
    public sealed class PostSelectStrategyFactory
    {
        private static IDictionary strategies = new HybridDictionary();

        /// <summary>
        /// Initializes the <see cref="PostSelectStrategyFactory"/> class.
        /// </summary>
        static PostSelectStrategyFactory()
        {
            strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForArrayList, new ArrayStrategy());
            strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForIList, new ListStrategy());
            strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForObject, new ObjectStrategy());
            strategies.Add(PostBindind.ExecuteMethod.ExecuteQueryForStrongTypedIList, new StrongTypedListStrategy());
        }


        /// <summary>
        /// Gets the <see cref="IPostSelectStrategy"/>.
        /// </summary>
        /// <param name="method">The <see cref="PostBindind.ExecuteMethod"/>.</param>
        /// <returns>The <see cref="IPostSelectStrategy"/></returns>
        public static IPostSelectStrategy Get(PostBindind.ExecuteMethod method)
        {
            return (IPostSelectStrategy)strategies[method];
        }
    }
}
