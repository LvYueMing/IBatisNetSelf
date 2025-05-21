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
        /// 初始化一个新的 <see cref="EmitPropertySetAccessor"/> 实例。
        /// 为属性设置器方法生成动态实现（IL）
        /// </summary>
        /// <param name="aTargetObjectType">目标对象的类型。</param>
        /// <param name="aPropertyName">属性名。</param>
        /// <param name="aAssemblyBuilder">用于生成动态程序集的 <see cref="AssemblyBuilder"/>。</param>
        /// <param name="aModuleBuilder">用于生成动态模块的 <see cref="ModuleBuilder"/>。</param>
        public EmitPropertySetAccessor(Type aTargetObjectType, string aPropertyName, AssemblyBuilder aAssemblyBuilder, ModuleBuilder aModuleBuilder)
        {
            // 保存目标类型
            this.targetType = aTargetObjectType;
            // 保存属性名
            this.propertyName = aPropertyName;

            // 处理通过 "new" 关键字隐藏基类属性的情况，只查找当前类声明的 public 实例属性
            PropertyInfo _propertyInfo = this.targetType.GetProperty(aPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            // 如果当前类未声明该属性，则尝试查找继承自基类的属性（所有可见属性）
            if (_propertyInfo == null)
            {
                _propertyInfo = this.targetType.GetProperty(aPropertyName);
            }

            // 确保属性存在,属性找不到，抛出异常
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
        /// 本方法创建一个用于属性访问的动态类型对象，
        /// 该类型将提供对属性的动态访问能力。
        /// </summary>
        /// <param name="aAssemblyBuilder">用于生成程序集的构建器。</param>
        /// <param name="aModuleBuilder">用于生成模块的构建器。</param>
        private void EmitIL(AssemblyBuilder aAssemblyBuilder, ModuleBuilder aModuleBuilder)
        {
            // 调用 EmitType 方法：使用 ModuleBuilder 发射（生成）一个新的类型（类），
            // 该类型实现了 ISet 接口，并具有 Set(object target, object value) 方法，
            // 可用于动态设置指定属性的值。
            this.EmitType(aModuleBuilder);

            // 使用 AssemblyBuilder 创建上一步生成的类型实例，
            // 并强制转换为 ISet 接口类型。
            // emittedSet 将被缓存用于后续调用 Set 方法。
            this.emittedSet = aAssemblyBuilder.CreateInstance(this.emitClassName) as ISet;

            // 获取当前属性类型的“空值”内部表示（用于处理 null 值或默认值）。
            // 例如：如果是值类型，可能返回对应的默认值。
            this.nullInternal = this.GetNullInternal(propertyType);

            // 如果实例创建失败（可能是类型发射出错），则抛出异常
            if (this.emittedSet == null)
            {
                throw new NotSupportedException(
                    $"Unable to create a get propert accessor for '{propertyName}' property on class  '{propertyType.ToString()}'.");
            }
        }


        /// <summary>
        /// 创建一个用于设置属性值的动态类型。
        /// </summary>
        /// <remarks>
        /// 注意：需要完全信任权限时可使用如下权限控制：
        /// new ReflectionPermission(PermissionState.Unrestricted).Assert();
        /// CodeAccessPermission.RevertAssert();
        /// </remarks>
        /// <param name="aModuleBuilder">模块构建器，用于生成动态类型。</param>
        private void EmitType(ModuleBuilder aModuleBuilder)
        {
            // 定义一个公共密封类，类名为 "FastSetAccessor.SetFor+类名+属性名"
            // this.targetType.FullName = "命名空间.类名"
            // this.targetType.Name = "类名"
            this.emitClassName = "FastSetAccessor.SetFor" + this.targetType.Name + this.propertyName;

            // 定义该类型（class），设置为 public sealed
            TypeBuilder _typeBuilder = aModuleBuilder.DefineType(this.emitClassName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

            // 实现 ISet 接口（表示该类型是一个属性设置器）
            _typeBuilder.AddInterfaceImplementation(typeof(ISet));

            // 添加一个无参的默认构造函数（public）
            _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            #region 实现ISet的方法 void Set(object target, object value)
            /*  等效代码 ****
               void Set(object target, object value)
               {
                    ((TargetType)target).set_propertyName((PropertyType)value);
               }
             */

            // 设置方法参数类型为 （object, object）（目标对象 和 值）
            Type[] _setParamTypes = new Type[] { typeof(object), typeof(object) };

            // 定义 "Set" 方法，public virtual void Set(object target, object value)
            MethodBuilder _methodBuilder = _typeBuilder.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, null, _setParamTypes);

            // 获取 IL 生成器用于发射 IL 指令
            // Set(object, value);
            ILGenerator _generatorIL = _methodBuilder.GetILGenerator();
            // 如果该属性可写
            if (this.canWrite)
            {
                // 获取对应属性的 set 方法
                MethodInfo _targetSetMethod = this.targetType.GetMethod("set_" + propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                // 如果未找到，尝试从继承层次结构中查找
                if (_targetSetMethod == null)
                {
                    _targetSetMethod = this.targetType.GetMethod("set_" + propertyName);
                }

                // 获取 set 方法的参数类型（属性类型）
                Type _paramType = _targetSetMethod.GetParameters()[0].ParameterType;

                // 声明一个局部变量(_paramType)，用于存储强转后的类型值
                // Stloc_0（存值）Ldloc_0（取值）
                _generatorIL.DeclareLocal(_paramType);

                //Ldarg_0 加载 this（当前实例）
                //Ldarg_1 加载第一个方法参数（这里是 target）
                //Ldarg_2 加载第二个方法参数（这里是 value）

                // 在动态生成的 IL 方法中，把传入的第一个参数 target（目标对象）压入栈中，以备后续操作（如类型转换、调用属性 setter）
                _generatorIL.Emit(OpCodes.Ldarg_1);

                // 从栈顶取出 target，把它转换为 targetType 类型，然后再把转换后的结果放回栈顶
                _generatorIL.Emit(OpCodes.Castclass, targetType);

                // 加载第二个参数（object value）
                _generatorIL.Emit(OpCodes.Ldarg_2);

                // 如果是值类型
                if (_paramType.IsValueType)
                {
                    // 将栈顶的对象(第二个参数)从引用类型转换（拆箱）为指定的值类型 _paramType，并将该值的地址压入计算栈。
                    _generatorIL.Emit(OpCodes.Unbox, _paramType);

                    // Ldind_I4：从栈顶的地址（指向 int32 的指针）读取值，并将该 int32 压入栈顶
                    if (typeToOpcode[_paramType] != null)
                    {
                        // OpCodes.Ldind_I1:将 int8 类型的值作为 int32 间接加载(即从地址加载)到计算堆栈上
                        OpCode _opCode = (OpCode)typeToOpcode[_paramType];
                        _generatorIL.Emit(_opCode); 
                    }
                    else
                    {
                        // 若未在字典中找到具体指令，则使用通用加载对象指令
                        // OpCodes.Ldobj 将地址指向的值类型对象复制到计算堆栈的顶部。
                        _generatorIL.Emit(OpCodes.Ldobj, _paramType);
                    }
                }
                else
                {
                    // 若是引用类型，则直接强制转换为目标类型
                    _generatorIL.Emit(OpCodes.Castclass, _paramType); //Cast class
                }

                //OpCodes.Callvirt:对对象调用后期绑定方法(虚函数)，并且将返回值推送到计算堆栈上。
                // 1、对象引用obj被推送到堆栈上。
                // 2、方法参数arg1-argN被推送到堆栈上。
                // 3、方法参数arg1-argN和对象引用obj从堆栈中弹出;
                //   使用这些参数执行方法调用，并将控制转移到方法元数据标记引用的obj中的方法。
                //   完成时，被调用方方法生成一个返回值并将其发送给调用方。
                // 4、返回值被推送到堆栈上。

                // 调用目标对象的属性设置方法（虚调用）
                //执行这条指令前，栈上应该有两个值：
                //对象实例（调用者） → 用来调用 setter 的对象。
                //要设置的值（参数） → 要赋给属性的值(栈顶)。

                //这行代码将调用你目标类型上的 set_属性名(...) 方法，把前面栈上的对象和值作为参数传进去，实现属性赋值的 IL 指令
                _generatorIL.EmitCall(OpCodes.Callvirt, _targetSetMethod, null); //Set the property value
                
                // 方法结束，返回
                // 执行前的栈要求：
                // * 如果当前方法有返回值，则返回值必须已经压入栈顶。
                // * 如果是 void 方法（比如我们生成的 Set(object, object)），则栈上不能有未消费的值。
                // * 一旦执行 Ret，方法执行结束，跳出当前作用域。
                _generatorIL.Emit(OpCodes.Ret);
            }
            else
            {
                // 如果属性不可写，抛出 MissingMethodException 异常
                _generatorIL.ThrowException(typeof(MissingMethodException));
            }
            #endregion

            // 创建并加载该类型到当前 AppDomain 中
            _typeBuilder.CreateType();
        }

    }
}
