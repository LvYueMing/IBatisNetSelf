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
    /// The <see cref="EmitPropertySetAccessor"/> class provides an IL-based set access   
    /// to a property of a specified target class.
    /// </summary>
    public sealed class EmitPropertySetAccessor : BaseAccessor, ISetAccessor
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

        private bool canWrite = false;
        /// <summary>
        /// The IL emitted ISet
        /// </summary>
        private ISet emittedSet = null;
        /// <summary>
        /// 动态创建的类名
        /// </summary>
        private string emitClassName = string.Empty;


        #region IAccessor Members

        /// <summary>
        /// Gets the member name.
        /// </summary>
        /// <value></value>
        public string Name=> this.propertyName;

        /// <summary>
        /// Gets the type of this member (field or property).
        /// </summary>
        /// <value></value>
        public Type MemberType=>this.propertyType;

        #endregion

        #region ISet Members

        /// <summary>
        /// Sets the property for the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="value">Value to set.</param>
        public void Set(object target, object value)
        {
            if (canWrite)
            {
                object _newValue = value;
                if (_newValue == null)
                {
                    // If the value to assign is null, assign null internal value
                    _newValue = nullInternal;
                }

                this.emittedSet.Set(target, _newValue);
            }
            else
            {
                throw new NotSupportedException(
                    $"Property \"{propertyName}\" on type {targetType} doesn't have a set method.");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitPropertySetAccessor"/> class.
        /// Generates the implementation for setter methods.
        /// </summary>
        /// <param name="aTargetObjectType">Type of the target object.</param>
        /// <param name="aPropertyName">Name of the property.</param>
        /// <param name="aAssemblyBuilder">The <see cref="AssemblyBuilder"/>.</param>
        /// <param name="aModuleBuilder">The <see cref="ModuleBuilder"/>.</param>
        public EmitPropertySetAccessor(Type aTargetObjectType, string aPropertyName, AssemblyBuilder aAssemblyBuilder, ModuleBuilder aModuleBuilder)
        {
            this.targetType = aTargetObjectType;
            this.propertyName = aPropertyName;

            // deals with Overriding a property using new and reflection
            PropertyInfo _propertyInfo = this.targetType.GetProperty(aPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (_propertyInfo == null)
            {
                _propertyInfo = this.targetType.GetProperty(aPropertyName);
            }

            // Make sure the property exists
            if (_propertyInfo == null)
            {
                throw new NotSupportedException($"Property \"{aPropertyName}\" does not exist for type {targetType}.");
            }
            else
            {
                this.propertyType = _propertyInfo.PropertyType;
                this.canWrite = _propertyInfo.CanWrite;
                this.EmitIL(aAssemblyBuilder, aModuleBuilder);
            }
        }


        /// <summary>
        /// This method create a new type oject for the the property accessor class 
        /// that will provide dynamic access.
        /// </summary>
        /// <param name="aAssemblyBuilder">The assembly builder.</param>
        /// <param name="aModuleBuilder">The module builder.</param>
        private void EmitIL(AssemblyBuilder aAssemblyBuilder, ModuleBuilder aModuleBuilder)
        {
            // Create a new type object for the the field accessor class.
            this.EmitType(aModuleBuilder);

            // Create a new instance
            this.emittedSet = aAssemblyBuilder.CreateInstance(this.emitClassName) as ISet;

            this.nullInternal = this.GetNullInternal(propertyType);

            if (this.emittedSet == null)
            {
                throw new NotSupportedException(
                    $"Unable to create a get propert accessor for '{propertyName}' property on class  '{propertyType.ToString()}'.");
            }
        }


        /// <summary>
        /// Create an type that will provide the set access method.
        /// </summary>
        /// <remarks>
        ///  new ReflectionPermission(PermissionState.Unrestricted).Assert();
        ///  CodeAccessPermission.RevertAssert();
        /// </remarks>
        /// <param name="aModuleBuilder">The module builder.</param>
        private void EmitType(ModuleBuilder aModuleBuilder)
        {
            // Define a public class named "FastSetAccessor.SetFor+TagetTypeName+PropertyName" in the assembly.
            // this.targetType.FullName = "Namespace.ClassName"
            // this.targetType.Name = "ClassName"
            this.emitClassName = "FastSetAccessor.SetFor" + this.targetType.Name + this.propertyName;
            TypeBuilder _typeBuilder = aModuleBuilder.DefineType(this.emitClassName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

            // Mark the class as implementing ISet. 
            _typeBuilder.AddInterfaceImplementation(typeof(ISet));

            // Add a constructor
            _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            #region 实现ISet的方法 void Set(object target, object value)
            // Define a method named "Set" for the set operation (ISet).
            Type[] _setParamTypes = new Type[] { typeof(object), typeof(object) };

            MethodBuilder _methodBuilder = _typeBuilder.DefineMethod("Set",MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                _setParamTypes);

            // Get an ILGenerator and  used to emit the IL that we want.
            // Set(object, value);
            ILGenerator _generatorIL = _methodBuilder.GetILGenerator();
            if (this.canWrite)
            {
                // Emit the IL for the set access. 
                MethodInfo _targetSetMethod = this.targetType.GetMethod("set_" + propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (_targetSetMethod == null)
                {
                    _targetSetMethod = this.targetType.GetMethod("set_" + propertyName);
                }
                Type _paramType = _targetSetMethod.GetParameters()[0].ParameterType;

                //声明指定类型(_paramType)的局部变量
                _generatorIL.DeclareLocal(_paramType);
                _generatorIL.Emit(OpCodes.Ldarg_1); //Load the first argument (target object)

                //OpCodes.Castclass:尝试将引用传递的对象转换为指定的类。
                _generatorIL.Emit(OpCodes.Castclass, targetType); //Cast to the source type(强制转换为源类型)
                _generatorIL.Emit(OpCodes.Ldarg_2); //Load the second argument (value object)
                if (_paramType.IsValueType)
                {
                    _generatorIL.Emit(OpCodes.Unbox, _paramType); //Unbox it 	
                    if (typeToOpcode[_paramType] != null)
                    {
                        //OpCodes.Ldind_I1:将 int8 类型的值作为 int32 间接加载(即从地址加载)到计算堆栈上
                        OpCode _opCode = (OpCode)typeToOpcode[_paramType];
                        _generatorIL.Emit(_opCode); 
                    }
                    else
                    {
                        //OpCodes.Ldobj 将地址指向的值类型对象复制到计算堆栈的顶部。
                        _generatorIL.Emit(OpCodes.Ldobj, _paramType);
                    }
                }
                else
                {
                    _generatorIL.Emit(OpCodes.Castclass, _paramType); //Cast class
                }
                //OpCodes.Callvirt:对对象调用后期绑定方法(虚函数)，并且将返回值推送到计算堆栈上。
                // 1、对象引用obj被推送到堆栈上。
                // 2、方法参数arg1-argN被推送到堆栈上。
                // 3、方法参数arg1-argN和对象引用obj从堆栈中弹出;
                //   使用这些参数执行方法调用，并将控制转移到方法元数据标记引用的obj中的方法。
                //   完成时，被调用方方法生成一个返回值并将其发送给调用方。
                // 4、返回值被推送到堆栈上。
                _generatorIL.EmitCall(OpCodes.Callvirt, _targetSetMethod, null); //Set the property value
                _generatorIL.Emit(OpCodes.Ret);
            }
            else
            {
                _generatorIL.ThrowException(typeof(MissingMethodException));
            }
            #endregion

            // Load the type
            _typeBuilder.CreateType();
        }

    }
}
