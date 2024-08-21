using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="EmitPropertyGetAccessor"/> class provides an IL-based get access   
    /// to a property of a specified target class.
    /// </summary>
    public sealed class EmitPropertyGetAccessor : BaseAccessor, IGetAccessor
    {
        /// <summary>
        /// The property name
        /// </summary>
        private string propertyName = string.Empty;
        /// <summary>
        /// The property type
        /// </summary>
        private Type propertyType = null;
        /// <summary>
        /// The class parent type
        /// </summary>
        private Type targetType = null;
        private bool canRead = false;
        /// <summary>
        /// The IL emitted IGet
        /// </summary>
        private IGet emittedGet = null;

        /// <summary>
        /// 动态创建的类名
        /// </summary>
        private string emitClassName = string.Empty;


        #region IAccessor Members

        /// <summary>
        /// Gets the property's name.
        /// </summary>
        /// <value></value>
        public string Name => this.propertyName;


        /// <summary>
        /// Gets the property's type.
        /// </summary>
        /// <value></value>
        public Type MemberType => this.propertyType;

        #endregion

        #region IGet Members

        /// <summary>
        /// Gets the property value from the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <returns>Property value.</returns>
        public object Get(object target)
        {
            if (this.canRead)
            {
                return this.emittedGet.Get(target);
            }
            else
            {
                throw new NotSupportedException($"Property \"{this.propertyName}\" on type {this.targetType} doesn't have a get method.");
            }
        }

        #endregion



        /// <summary>
        /// Initializes a new instance of the <see cref="EmitPropertyGetAccessor"/> class.
        /// </summary>
        /// <param name="targetObjectType">Type of the target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="assemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <param name="moduleBuilder">The <see cref="ModuleBuilder"/>.</param>
        public EmitPropertyGetAccessor(Type targetObjectType, string propertyName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this.targetType = targetObjectType;
            this.propertyName = propertyName;

            // deals with Overriding a property using new and reflection
            PropertyInfo propertyInfo = this.targetType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (propertyInfo == null)
            {
                propertyInfo = this.targetType.GetProperty(propertyName);
            }

            // Make sure the property exists
            if (propertyInfo == null)
            {
                throw new NotSupportedException($"Property \"{propertyName}\" does not exist for type {this.targetType}.");
            }
            else
            {
                this.propertyType = propertyInfo.PropertyType;
                this.canRead = propertyInfo.CanRead;
                this.EmitIL(assemblyBuilder, moduleBuilder);
            }
        }

        /// <summary>
        /// This method create a new type oject for the the property accessor class 
        /// that will provide dynamic access.
        /// </summary>
        /// <param name="assemblyBuilder">The assembly builder.</param>
        /// <param name="moduleBuilder">The module builder.</param>
        private void EmitIL(AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            // Create a new type object for the the field accessor class.
            EmitType(moduleBuilder);

            // Create a new instance
            this.emittedGet = assemblyBuilder.CreateInstance(this.emitClassName) as IGet;

            this.nullInternal = this.GetNullInternal(this.propertyType);

            if (this.emittedGet == null)
            {
                throw new NotSupportedException(
                    string.Format("Unable to create a get property accessor for \"{0}\".", this.propertyType));
            }
        }

        /// <summary>
        /// Create an type that will provide the set access method.
        /// </summary>
        /// <remarks>
        ///  new ReflectionPermission(PermissionState.Unrestricted).Assert();
        ///  CodeAccessPermission.RevertAssert();
        /// </remarks>
        /// <param name="moduleBuilder">The module builder.</param>
        private void EmitType(ModuleBuilder moduleBuilder)
        {

            // Define a public class named "FastSetAccessor.GetFor + TagetTypeName + PropertyName" in the assembly.
            // this.targetType.FullName = "Namespace.ClassName"
            // this.targetType.Name = "ClassName"
            this.emitClassName = "FastGetAccessor.GetFor" + this.targetType.Name + this.propertyName;
            TypeBuilder _typeBuilder = moduleBuilder.DefineType(this.emitClassName,TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

            // Mark the class as implementing IMemberAccessor. 
            _typeBuilder.AddInterfaceImplementation(typeof(IGet));

            // Add a constructor
            _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            #region Emit Get
            // Define a method named "Get" for the get operation (IGet). 
            Type[] _getParamTypes = new Type[] { typeof(object) };
            MethodBuilder _methodBuilder = _typeBuilder.DefineMethod("Get",MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), _getParamTypes);
            // Get an ILGenerator and used it to emit the IL that we want.
            ILGenerator generatorIL = _methodBuilder.GetILGenerator();

            if (this.canRead)
            {
                // Emit the IL for get access. 
                MethodInfo _targetGetMethod = this.targetType.GetMethod("get_" + this.propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (_targetGetMethod == null)
                {
                    _targetGetMethod = this.targetType.GetMethod("get_" + this.propertyName);
                }

                generatorIL.DeclareLocal(typeof(object));
                generatorIL.Emit(OpCodes.Ldarg_1);	//Load the first argument,(target object)
                generatorIL.Emit(OpCodes.Castclass, this.targetType);	//Cast to the source type
                generatorIL.EmitCall(OpCodes.Call, _targetGetMethod, null); //Get the property value
                if (_targetGetMethod.ReturnType.IsValueType)
                {
                    generatorIL.Emit(OpCodes.Box, _targetGetMethod.ReturnType); //Box if necessary
                }
                generatorIL.Emit(OpCodes.Stloc_0); //Store it
                generatorIL.Emit(OpCodes.Ldloc_0);
                generatorIL.Emit(OpCodes.Ret);
            }
            else
            {
                generatorIL.ThrowException(typeof(MissingMethodException));
            }
            #endregion

            // Load the type
            _typeBuilder.CreateType();
        }

    }
}
