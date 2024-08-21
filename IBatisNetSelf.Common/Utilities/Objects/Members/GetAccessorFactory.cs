using IBatisNetSelf.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// A factory to build <see cref="IGetAccessorFactory"/> for a type.
    /// </summary>
    public class GetAccessorFactory : IGetAccessorFactory
    {
        private delegate IGetAccessor CreatePropertyGetAccessor(Type targetType, string propertyName);
        private delegate IGetAccessor CreateFieldGetAccessor(Type targetType, string fieldName);

        private CreatePropertyGetAccessor createPropertyGetAccessor = null;
        private CreateFieldGetAccessor createFieldGetAccessor = null;

        private IDictionary cachedIGetAccessor = new HybridDictionary();
        private AssemblyBuilder assemblyBuilder = null;
        private ModuleBuilder moduleBuilder = null;
        private object syncObject = new object();

        public ModuleBuilder ModuleBuilder => this.moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAccessorFactory"/> class.
        /// </summary>
        /// <param name="allowCodeGeneration">if set to <c>true</c> [allow code generation].</param>
        public GetAccessorFactory(bool allowCodeGeneration)
        {
            if (allowCodeGeneration)
            {
                AssemblyName assemblyName = new AssemblyName();
                assemblyName.Name = "IBatisNetSelf.FastGetAccessor" + HashCodeProvider.GetIdentityHashCode(this).ToString();

                // Create a new assembly with one module
                this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");

                this.createPropertyGetAccessor = new CreatePropertyGetAccessor(CreatePropertyAccessor);
                this.createFieldGetAccessor = new CreateFieldGetAccessor(CreateFieldAccessor);
            }
            else
            {
                this.createPropertyGetAccessor = new CreatePropertyGetAccessor(CreateReflectionPropertyGetAccessor);
                this.createFieldGetAccessor = new CreateFieldGetAccessor(CreateReflectionFieldGetAccessor);
            }
        }


        /// <summary>
        /// Create a IGetAccessor instance for a property
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>null if the generation fail</returns>
        private IGetAccessor CreatePropertyAccessor(Type targetType, string propertyName)
        {
            ReflectionInfo reflectionCache = ReflectionInfo.GetInstance(targetType);
            PropertyInfo propertyInfo = (PropertyInfo)reflectionCache.GetGetter(propertyName);

            if (propertyInfo.CanRead)
            {
                MethodInfo methodInfo = null;

                methodInfo = targetType.GetMethod("get_" + propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methodInfo == null)
                {
                    methodInfo = targetType.GetMethod("get_" + propertyName);
                }
                if (methodInfo != null)// == visibilty public
                {
                    return new EmitPropertyGetAccessor(targetType, propertyName, this.assemblyBuilder, this.moduleBuilder);
                }
                else
                {
                    return new ReflectionPropertyGetAccessor(targetType, propertyName);
                }
            }
            else
            {
                throw new NotSupportedException($"Property \"{propertyInfo.Name}\" on type {targetType} cannot be get.");
            }
        }

        /// <summary>
        /// Create a IGetAccessor instance for a field
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>null if the generation fail</returns>
        private IGetAccessor CreateFieldAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo reflectionCache = ReflectionInfo.GetInstance(targetType);
            FieldInfo fieldInfo = (FieldInfo)reflectionCache.GetGetter(fieldName);

            if (fieldInfo.IsPublic)
            {
                return new EmitFieldGetAccessor(targetType, fieldName, this.assemblyBuilder, this.moduleBuilder);
            }
            else
            {
                return new ReflectionFieldGetAccessor(targetType, fieldName);
            }
        }

        /// <summary>
        /// Create a Reflection IGetAccessor instance for a property
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>null if the generation fail</returns>
        private IGetAccessor CreateReflectionPropertyGetAccessor(Type targetType, string propertyName)
        {
            return new ReflectionPropertyGetAccessor(targetType, propertyName);
        }

        /// <summary>
        /// Create Reflection IGetAccessor instance for a field
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="fieldName">field name.</param>
        /// <returns>null if the generation fail</returns>
        private IGetAccessor CreateReflectionFieldGetAccessor(Type targetType, string fieldName)
        {
            return new ReflectionFieldGetAccessor(targetType, fieldName);
        }

        #region IGetAccessorFactory Members

        /// <summary>
        /// Generate an <see cref="IGetAccessor"/> instance.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="name">Field or Property name.</param>
        /// <returns>null if the generation fail</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IGetAccessor CreateGetAccessor(Type targetType, string name)
        {
            string _key = new StringBuilder(targetType.FullName).Append(".").Append(name).ToString();

            if (this.cachedIGetAccessor.Contains(_key))
            {
                return (IGetAccessor)this.cachedIGetAccessor[_key];
            }
            else
            {
                IGetAccessor _getAccessor = null;
                lock (this.syncObject)
                {
                    if (!this.cachedIGetAccessor.Contains(_key))
                    {
                        // Property
                        ReflectionInfo _reflectionCache = ReflectionInfo.GetInstance(targetType);
                        MemberInfo memberInfo = _reflectionCache.GetGetter(name);

                        if (memberInfo != null)
                        {
                            if (memberInfo is PropertyInfo)
                            {
                                _getAccessor = this.createPropertyGetAccessor(targetType, name);
                                this.cachedIGetAccessor[_key] = _getAccessor;
                            }
                            else
                            {
                                _getAccessor = this.createFieldGetAccessor(targetType, name);
                                this.cachedIGetAccessor[_key] = _getAccessor;
                            }
                        }
                        else
                        {
                            throw new ProbeException($"No property or field named \"{name}\" exists for type {targetType}.");
                        }
                    }
                    else
                    {
                        _getAccessor = (IGetAccessor)this.cachedIGetAccessor[_key];
                    }
                }
                return _getAccessor;
            }
        }

        #endregion
    }
}
