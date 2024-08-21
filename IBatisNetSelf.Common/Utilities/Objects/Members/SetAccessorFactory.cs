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
    /// A factory to build <see cref="SetAccessorFactory"/> for a type.
    /// </summary>
    public class SetAccessorFactory : ISetAccessorFactory
    {
        private delegate ISetAccessor CreatePropertySetAccessor(Type targetType, string propertyName);
        private delegate ISetAccessor CreateFieldSetAccessor(Type targetType, string fieldName);

        private CreatePropertySetAccessor createPropertySetAccessor = null;
        private CreateFieldSetAccessor createFieldSetAccessor = null;

        private IDictionary cachedISetAccessor = new HybridDictionary();
        private AssemblyBuilder assemblyBuilder = null;
        private ModuleBuilder moduleBuilder = null;
        private object syncObject = new object();

        public ModuleBuilder ModuleBuilder => this.moduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAccessorFactory"/> class.
        /// </summary>
        /// <param name="aAllowCodeGeneration">if set to <c>true</c> [allow code generation].</param>
        public SetAccessorFactory(bool aAllowCodeGeneration)
        {
            if (aAllowCodeGeneration)
            {
                AssemblyName _assemblyName = new AssemblyName();
                _assemblyName.Name = "IBatisNetSelf.FastSetAccessor" + HashCodeProvider.GetIdentityHashCode(this).ToString();

                //Create a new assembly with one module
                this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
                this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(_assemblyName.Name + ".dll");

                this.createPropertySetAccessor = new CreatePropertySetAccessor(CreatePropertyAccessor);
                this.createFieldSetAccessor = new CreateFieldSetAccessor(CreateFieldAccessor);
            }
            else
            {
                this.createPropertySetAccessor = new CreatePropertySetAccessor(CreateReflectionPropertySetAccessor);
                this.createFieldSetAccessor = new CreateFieldSetAccessor(CreateReflectionFieldSetAccessor);
            }
        }


        /// <summary>
        /// Create a ISetAccessor instance for a property
        /// </summary>
        /// <param name="aTtargetType">Target object type.</param>
        /// <param name="aPropertyName">Property name.</param>
        /// <returns>null if the generation fail</returns>
        private ISetAccessor CreatePropertyAccessor(Type aTtargetType, string aPropertyName)
        {
            ReflectionInfo _reflectionInfo = ReflectionInfo.GetInstance(aTtargetType);
            PropertyInfo _propertyInfo = _reflectionInfo.GetSetter(aPropertyName) as PropertyInfo;

            if (_propertyInfo.CanWrite)
            {
                MethodInfo _setMethodInfo = aTtargetType.GetMethod("set_" + aPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (_setMethodInfo == null)
                {
                    _setMethodInfo = aTtargetType.GetMethod("set_" + aPropertyName);
                }

                if (_setMethodInfo != null)// == visibilty public
                {
                    return new EmitPropertySetAccessor(aTtargetType, aPropertyName, this.assemblyBuilder, this.moduleBuilder);
                }
                else
                {
                    return new ReflectionPropertySetAccessor(aTtargetType, aPropertyName);
                }
            }
            else
            {
                throw new NotSupportedException($"Property \"{_propertyInfo.Name}\" on type {aTtargetType} cannot be set.");
            }
        }

        /// <summary>
        /// Create a ISetAccessor instance for a field
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>null if the generation fail</returns>
        private ISetAccessor CreateFieldAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo reflectionCache = ReflectionInfo.GetInstance(targetType);
            FieldInfo fieldInfo = (FieldInfo)reflectionCache.GetSetter(fieldName);

            if (fieldInfo.IsPublic)
            {
                return new EmitFieldSetAccessor(targetType, fieldName, assemblyBuilder, moduleBuilder);
            }
            else
            {
                return new ReflectionFieldSetAccessor(targetType, fieldName);
            }
        }

        /// <summary>
        /// Create a Reflection ISetAccessor instance for a property
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>null if the generation fail</returns>
        private ISetAccessor CreateReflectionPropertySetAccessor(Type targetType, string propertyName)
        {
            return new ReflectionPropertySetAccessor(targetType, propertyName);
        }

        /// <summary>
        /// Create Reflection ISetAccessor instance for a field
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="fieldName">field name.</param>
        /// <returns>null if the generation fail</returns>
        private ISetAccessor CreateReflectionFieldSetAccessor(Type targetType, string fieldName)
        {
            return new ReflectionFieldSetAccessor(targetType, fieldName);
        }

        #region ISetAccessorFactory Members

        /// <summary>
        /// Generate an <see cref="ISetAccessor"/> instance.
        /// </summary>
        /// <param name="aTargetType">Target object type.</param>
        /// <param name="aName">Field or Property name.</param>
        /// <returns>null if the generation fail</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ISetAccessor CreateSetAccessor(Type aTargetType, string aName)
        {
            string _key = new StringBuilder(aTargetType.FullName).Append(".").Append(aName).ToString();

            if (this.cachedISetAccessor.Contains(_key))
            {
                return (ISetAccessor)this.cachedISetAccessor[_key];
            }
            else
            {
                ISetAccessor _setAccessor = null;
                lock (this.syncObject)
                {
                    if (!this.cachedISetAccessor.Contains(_key))
                    {
                        // Property
                        ReflectionInfo _reflectionCache = ReflectionInfo.GetInstance(aTargetType);
                        MemberInfo _memberInfo = _reflectionCache.GetSetter(aName);

                        if (_memberInfo != null)
                        {
                            if (_memberInfo is PropertyInfo)
                            {
                                _setAccessor = this.createPropertySetAccessor(aTargetType, aName);
                                this.cachedISetAccessor[_key] = _setAccessor;
                            }
                            else
                            {
                                _setAccessor = createFieldSetAccessor(aTargetType, aName);
                                this.cachedISetAccessor[_key] = _setAccessor;
                            }
                        }
                        else
                        {
                            throw new ProbeException(
                                $"No property or field named \"{aName}\" exists for type {aTargetType}.");
                        }
                    }
                    else
                    {
                        _setAccessor = (ISetAccessor)this.cachedISetAccessor[_key];
                    }
                }
                return _setAccessor;
            }
        }

        #endregion

    }
}
