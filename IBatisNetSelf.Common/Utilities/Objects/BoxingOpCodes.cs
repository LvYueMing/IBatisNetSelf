using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>  
    /// Helper class that returns appropriate boxing opcode based on type  
    /// Helper类，根据类型返回适当的装箱操作码
    /// </summary>  
    /// <remarks>From Spring.NET</remarks>
    public sealed class BoxingOpCodes
    {
        private static IDictionary boxingOpCodes;

        /// <summary>
        /// Initializes the <see cref="BoxingOpCodes"/> class.
        /// </summary>
        static BoxingOpCodes()
        {
            boxingOpCodes = new Hashtable();
            boxingOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            boxingOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            boxingOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            boxingOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            boxingOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            boxingOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            boxingOpCodes[typeof(float)] = OpCodes.Ldind_R4;
            boxingOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            boxingOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            boxingOpCodes[typeof(bool)] = OpCodes.Ldind_I1;
        }

        /// <summary>
        /// Gets the <see cref="OpCode"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static OpCode GetOpCode(Type type)
        {
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }
            return (OpCode)boxingOpCodes[type];
        }

    }
}
