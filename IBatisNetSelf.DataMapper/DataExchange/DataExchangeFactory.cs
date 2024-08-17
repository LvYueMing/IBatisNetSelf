using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.DataExchange
{
    /// <summary>
    /// Factory for DataExchange objects
    /// </summary>
    public class DataExchangeFactory
    {
        private TypeHandlerFactory typeHandlerFactory = null;
        private IObjectFactory objectFactory = null;
        private AccessorFactory accessorFactory = null;

        private IDataExchange primitiveDataExchange = null;
        private IDataExchange complexDataExchange = null;
        private IDataExchange listDataExchange = null;
        private IDataExchange dictionaryDataExchange = null;

        /// <summary>
        ///  Getter for the type handler factory
        /// </summary>
        public TypeHandlerFactory TypeHandlerFactory
        {
            get { return typeHandlerFactory; }
        }

        /// <summary>
        /// The factory for object
        /// </summary>
        public IObjectFactory ObjectFactory
        {
            get { return objectFactory; }
        }

        /// <summary>
        /// The factory which build <see cref="ISetAccessor"/>
        /// </summary>
        public AccessorFactory AccessorFactory
        {
            get { return accessorFactory; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DataExchangeFactory"/> class.
        /// </summary>
        /// <param name="typeHandlerFactory">The type handler factory.</param>
        /// <param name="objectFactory">The object factory.</param>
        /// <param name="accessorFactory">The accessor factory.</param>
        public DataExchangeFactory(TypeHandlerFactory typeHandlerFactory, IObjectFactory objectFactory, AccessorFactory accessorFactory)
        {
            this.objectFactory = objectFactory;
            this.typeHandlerFactory = typeHandlerFactory;
            this.accessorFactory = accessorFactory;

            this.primitiveDataExchange = new PrimitiveDataExchange(this);
            this.complexDataExchange = new ComplexDataExchange(this);
            this.listDataExchange = new ListDataExchange(this);
            this.dictionaryDataExchange = new DictionaryDataExchange(this);
        }

        /// <summary>
        /// Get a DataExchange object for the passed in Class
        /// </summary>
        /// <param name="clazz">The class to get a DataExchange object for</param>
        /// <returns>The IDataExchange object</returns>
        public IDataExchange GetDataExchangeForClass(Type clazz)
        {
            IDataExchange _dataExchange = null;
            if (clazz == null)
            {
                _dataExchange = this.complexDataExchange;
            }
            else if (typeof(IList).IsAssignableFrom(clazz))
            {
                _dataExchange = this.listDataExchange;
            }
            else if (typeof(IDictionary).IsAssignableFrom(clazz))
            {
                _dataExchange = this.dictionaryDataExchange;
            }
            else if (this.typeHandlerFactory.GetTypeHandler(clazz) != null)
            {
                _dataExchange = this.primitiveDataExchange;
            }
            else
            {
                _dataExchange = new DotNetObjectDataExchange(clazz, this);
            }

            return _dataExchange;
        }

    }
}
