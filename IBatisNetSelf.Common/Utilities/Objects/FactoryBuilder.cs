using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IBatisNetSelf.Common.Exceptions;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// Build IFactory object via IL 
    /// </summary>
    public class FactoryBuilder
    {
        private const BindingFlags VISIBILITY = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const MethodAttributes CREATE_METHOD_ATTRIBUTES = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ModuleBuilder moduleBuilder = null;

        public ModuleBuilder ModuleBuilder => this.moduleBuilder;

        /// <summary>
        /// constructor
        /// </summary>
        public FactoryBuilder()
        {
            AssemblyName _assemblyName = new AssemblyName();
            _assemblyName.Name = "IBatisNetSelf.EmitFactory" + HashCodeProvider.GetIdentityHashCode(this).ToString();
                       
            // Create a new assembly with one module
            AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);

            this.moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name + ".dll");
        }


        /// <summary>
        /// Create a factory which build class of type typeToCreate
        /// </summary>
        /// <param name="aTypeToCreate">The type instance to build</param>
        /// <param name="aTypes">The types of the constructor arguments</param>
        /// <returns>Returns a new <see cref="IFactory"/> instance.</returns>
        public IFactory CreateFactory(Type aTypeToCreate, Type[] aTypes)
        {
            if (aTypeToCreate.IsAbstract)
            {
                if (_logger.IsInfoEnabled)
                {
                    _logger.Info("Create a stub IFactory for abstract type " + aTypeToCreate.Name);
                }
                return new AbstractFactory(aTypeToCreate);
            }
            else
            {
                Type _innerType = this.CreateFactoryType(aTypeToCreate, aTypes);
                ConstructorInfo _ctor = _innerType.GetConstructor(Type.EmptyTypes);
                return (IFactory)_ctor.Invoke(new object[] { });
            }
        }


        /// <summary>
        /// Creates a <see cref="IFactory"/>.
        /// </summary>
        /// <param name="aTypeToCreate">The type instance to create.</param>
        /// <param name="aArgumentTypes">The types.</param>
        /// <returns>The <see cref="IFactory"/></returns>
        private Type? CreateFactoryType(Type aTypeToCreate, Type[] aArgumentTypes)
        {
            string _argumentTypeNames = string.Empty;
            for (int i = 0; i < aArgumentTypes.Length; i++)
            {
                _argumentTypeNames += aArgumentTypes[i].Name.Replace("[]", string.Empty) + i.ToString();
            }

            string _typeNameToCreate = "EmitObjectFactory.CreateFor" + aTypeToCreate.Name + _argumentTypeNames;
            TypeBuilder _typeBuilder = this.moduleBuilder.DefineType(_typeNameToCreate, TypeAttributes.Public);

            //adds an interface that this type implements
            _typeBuilder.AddInterfaceImplementation(typeof(IFactory));

            //实现CreateInstance()方法
            this.ImplementCreateInstance(_typeBuilder, aTypeToCreate, aArgumentTypes);
            return _typeBuilder.CreateType();
        }

        /// <summary>
        /// Implements the "CreateInstance" method.
        /// </summary>
        /// <param name="aTypeBuilder">The type builder.</param>
        /// <param name="aTypeToCreate">The type to create.</param>
        /// <param name="aArgumentTypes">The argument types.</param>
        private void ImplementCreateInstance(TypeBuilder aTypeBuilder, Type aTypeToCreate, Type[] aArgumentTypes)
        {
            //object CreateInstance(object[] parameters);
            MethodBuilder _method = aTypeBuilder.DefineMethod("CreateInstance", CREATE_METHOD_ATTRIBUTES, typeof(object), new Type[] { typeof(object[]) });
            ILGenerator _il = _method.GetILGenerator();

            // Add test if contructeur not public
            ConstructorInfo? _constructor = aTypeToCreate.GetConstructor(VISIBILITY, null, aArgumentTypes, null);
            if (_constructor == null || !_constructor.IsPublic)
            {
                throw new ProbeException(
                    $"Unable to optimize create instance. Cause : Could not find public constructor matching specified arguments for type \"{aTypeToCreate.Name}\".");
            }
            // new typeToCreate() or new typeToCreate(... arguments ...)
            this.EmitArgsIL(_il, aArgumentTypes);
            //OpCodes.Newobj:创建一个值类型的新对象或新实例，并将对象引用（O 类型）推送到计算堆栈上
            // 1、参数arg1-argn被按顺序压入堆栈
            // 2、参数argn-arg1从堆栈中弹出，并传递给构造函数用于对象创建。
            // 3、新对象的引用被压入堆栈。
            _il.Emit(OpCodes.Newobj, _constructor);
            //OpCodes.Ret:从当前方法返回，并将返回值（如果存在）从被调用方的计算堆栈推送到调用方的计算堆栈上。
            _il.Emit(OpCodes.Ret);
        }

        /// <summary>   
        /// Emit parameter IL for a method call.   
        /// </summary>   
        /// <param name="aIl">IL generator.</param>   
        /// <param name="aArgumentTypes">Arguments type defined for a the constructor.</param>   
        private void EmitArgsIL(ILGenerator aIl, Type[] aArgumentTypes)
        {
            // Add args. Since all args are objects, value types are unboxed. 
            // Refs to value types are to be converted to values themselves.   
            for (int i = 0; i < aArgumentTypes.Length; i++)
            {
                // Push args array reference on the stack , followed by the index.   
                // Ldelem will resolve them to args[i].
                // 1、引用对象数组 array 被推送到堆栈上。
                // 2、索引值 index 被推送到堆栈上。
                // 3、索引值index和数组array都从堆栈中弹出; 查找存储在数组 array 中索引 index 处的值。
                // 4、该值被推送到堆栈上。

                //arg_0 表示类实例,此处不需要
                //将arg_1(索引1)的参数加载到计算堆栈上。
                aIl.Emit(OpCodes.Ldarg_1);   // Arg_1 is argument array.
                //将值 i 推送到堆栈上。
                aIl.Emit(OpCodes.Ldc_I4, i);
                //将位于指定数组 索引处的对象引用加载到计算堆栈的顶部
                aIl.Emit(OpCodes.Ldelem_Ref);

                // If param is a primitive/value type then we need to unbox it.
                // OpCodes.Unbox 取消装箱
                // 1、对象引用被推送到堆栈上。
                // 2、对象引用从堆栈弹出并取消装箱到值类型指针。
                // 3、值类型指针被推送到堆栈上。
                Type _paramType = aArgumentTypes[i];
                if (_paramType.IsValueType)
                {
                    if (_paramType.IsPrimitive || _paramType.IsEnum)
                    {
                        aIl.Emit(OpCodes.Unbox, _paramType);
                        //OpCodes.Ldind_I4:将 int32 类型的值作为 int32 间接加载到计算堆栈上
                        // 1、地址被推送到堆栈上。
                        // 2、地址从堆栈中弹出; 提取位于地址处的值。
                        // 3、提取的值被推送到堆栈上。
                        aIl.Emit(BoxingOpCodes.GetOpCode(_paramType));
                    }
                    else if (_paramType.IsValueType)
                    {
                        aIl.Emit(OpCodes.Unbox, _paramType);
                        //OpCodes.Ldobj 将地址指向的值类型对象复制到计算堆栈的顶部
                        // 1、值类型对象的地址被推送到堆栈中。
                        // 2、该地址从堆栈中弹出，查找该地址对应的实例
                        ///3、存储在该地址处的对象的值被推送到堆栈中。
                        aIl.Emit(OpCodes.Ldobj, _paramType);
                    }
                }
            }
        }

    }
}
