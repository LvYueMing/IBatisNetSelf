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
    /// The <see cref="EmitFieldGetAccessor"/> class provides an IL-based get access   
    /// to a field of a specified target class.
    /// </summary>
    /// <remarks>Will Throw FieldAccessException on private field</remarks>
    public sealed class EmitFieldGetAccessor : BaseAccessor, IGetAccessor
    {
        private const BindingFlags BindingFlagsForField = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// The field name
        /// </summary>
        private string fieldName = string.Empty;
        /// <summary>
        /// The class parent type
        /// </summary>
        private Type fieldType = null;
        /// <summary>
        /// The IL emitted IGet
        /// </summary>
        private IGet emittedGet = null;
        private Type targetType = null;


        #region IAccessor Members

        /// <summary>
        /// Gets the field's name.
        /// </summary>
        /// <value></value>
        public string Name=> this.fieldName;


        /// <summary>
        /// Gets the field's type.
        /// </summary>
        /// <value></value>
        public Type MemberType=> this.fieldType;

        #endregion

        #region IGet Members

        /// <summary>
        /// Gets the value stored in the field for the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve the field from.</param>
        /// <returns>The value.</returns>
        public object Get(object target)
        {
            return this.emittedGet.Get(target);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitFieldGetAccessor"/> class.
        /// </summary>
        /// <param name="targetObjectType">Type of the target object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="assemblyBuilder">The assembly builder.</param>
        /// <param name="moduleBuilder">The module builder.</param>
        public EmitFieldGetAccessor(Type targetObjectType, string fieldName, AssemblyBuilder assemblyBuilder, ModuleBuilder moduleBuilder)
        {
            this.targetType = targetObjectType;
            this.fieldName = fieldName;

            FieldInfo fieldInfo = this.targetType.GetField(fieldName, BindingFlagsForField);

            // Make sure the field exists
            if (fieldInfo == null)
            {
                throw new NotSupportedException($"Field \"{fieldName}\" does not exist for type {targetObjectType}.");
            }
            else
            {
                this.fieldType = fieldInfo.FieldType;
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
            this.emittedGet = assemblyBuilder.CreateInstance("GetFor" + this.targetType.FullName + this.fieldName) as IGet;

            this.nullInternal = this.GetNullInternal(this.fieldType);

            if (this.emittedGet == null)
            {
                throw new NotSupportedException(
                    $"Unable to create a get field accessor for '{this.fieldName}' field on class  '{this.fieldType}");
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
            // Define a public class named "GetFor.FullTagetTypeName.FieldName" in the assembly.
            TypeBuilder _typeBuilder = moduleBuilder.DefineType("GetFor" + this.targetType.FullName + this.fieldName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

            // Mark the class as implementing IMemberAccessor. 
            _typeBuilder.AddInterfaceImplementation(typeof(IGet));

            // Add a constructor
            _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            #region Emit Get
            // Define a method named "Get" for the get operation (IMemberAccessor). 
            Type[] getParamTypes = new Type[] { typeof(object) };
            MethodBuilder getMethod = _typeBuilder.DefineMethod("Get",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                getParamTypes);

            // Get an ILGenerator and used it to emit the IL that we want.
            ILGenerator _getIL = getMethod.GetILGenerator();

            FieldInfo _targetField = this.targetType.GetField(this.fieldName, BindingFlagsForField);

            // Emit the IL for get access. 
            if (_targetField != null)
            {
                // We need a reference to the current instance (stored in local argument index 1) 
                // so Ldfld can load from the correct instance (this one).
                _getIL.Emit(OpCodes.Ldarg_1);
                _getIL.Emit(OpCodes.Ldfld, _targetField);
                if (this.fieldType.IsValueType)
                {
                    // Now, we execute the box opcode, which pops the value of field 'x',
                    // returning a reference to the filed value boxed as an object.
                    _getIL.Emit(OpCodes.Box, _targetField.FieldType);
                }
                _getIL.Emit(OpCodes.Ret);
            }
            else
            {
                _getIL.ThrowException(typeof(MissingMethodException));
            }
            #endregion

            // Load the type
            _typeBuilder.CreateType();
        }

    }
}
