using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// Abstract base class for member accessor
    /// </summary>
    public abstract class BaseAccessor
    {
        /// <summary>
        /// The property name
        /// </summary>
        protected string propertyName = string.Empty;

        /// <summary>
        /// The target type
        /// </summary>
        protected Type targetType = null;

        /// <summary>
        /// The null internal value used by this member type 
        /// </summary>
        protected object nullInternal = null;

        /// <summary>
        /// List of type-opCode
		/// </summary>
		protected static IDictionary typeToOpcode = new HybridDictionary();

        /// <summary>
        /// Static constructor
        /// "Initialize a private IDictionary with type-opCode pairs 
        /// </summary>
        static BaseAccessor()
        {
            typeToOpcode[typeof(sbyte)] = OpCodes.Ldind_I1;
            typeToOpcode[typeof(byte)] = OpCodes.Ldind_U1;
            typeToOpcode[typeof(char)] = OpCodes.Ldind_U2;
            typeToOpcode[typeof(short)] = OpCodes.Ldind_I2;
            typeToOpcode[typeof(ushort)] = OpCodes.Ldind_U2;
            typeToOpcode[typeof(int)] = OpCodes.Ldind_I4;
            typeToOpcode[typeof(uint)] = OpCodes.Ldind_U4;
            typeToOpcode[typeof(long)] = OpCodes.Ldind_I8;
            typeToOpcode[typeof(ulong)] = OpCodes.Ldind_I8;
            typeToOpcode[typeof(bool)] = OpCodes.Ldind_I1;
            typeToOpcode[typeof(double)] = OpCodes.Ldind_R8;
            typeToOpcode[typeof(float)] = OpCodes.Ldind_R4;
        }


        /// <summary>
        /// Gets the property info.
        /// </summary>
        /// <param name="target">The target type.</param>
        /// <returns></returns>
        protected PropertyInfo GetPropertyInfo(Type target)
        {
            PropertyInfo propertyInfo = null;

            propertyInfo = target.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (propertyInfo == null)
            {
                if (target.IsInterface)
                {
                    // JIRA 210
                    // Fix for interface inheriting
                    // Loop through interfaces of the type
                    foreach (Type interfaceType in target.GetInterfaces())
                    {
                        // Get propertyinfo and if found the break out of loop
                        propertyInfo = GetPropertyInfo(interfaceType);
                        if (propertyInfo != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // deals with Overriding a property using new and reflection
                    // http://blogs.msdn.com/thottams/archive/2006/03/17/553376.aspx
                    propertyInfo = target.GetProperty(propertyName);
                }
            }

            return propertyInfo;
        }

        /// <summary>
        /// Get the null value for a given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected object GetNullInternal(Type type)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    return GetNullInternal(Enum.GetUnderlyingType(type));
                }

                if (type.IsPrimitive)
                {
                    if (type == typeof(Int32)) { return 0; }
                    if (type == typeof(Double)) { return (Double)0; }
                    if (type == typeof(Int16)) { return (Int16)0; }
                    if (type == typeof(SByte)) { return (SByte)0; }
                    if (type == typeof(Int64)) { return (Int64)0; }
                    if (type == typeof(Byte)) { return (Byte)0; }
                    if (type == typeof(UInt16)) { return (UInt16)0; }
                    if (type == typeof(UInt32)) { return (UInt32)0; }
                    if (type == typeof(UInt64)) { return (UInt64)0; }
                    if (type == typeof(UInt64)) { return (UInt64)0; }
                    if (type == typeof(Single)) { return (Single)0; }
                    if (type == typeof(Boolean)) { return false; }
                    if (type == typeof(char)) { return '\0'; }
                }
                else
                {
                    //DateTime : 01/01/0001 00:00:00
                    //TimeSpan : 00:00:00
                    //Guid : 00000000-0000-0000-0000-000000000000
                    //Decimal : 0

                    if (type == typeof(DateTime)) { return DateTime.MinValue; }
                    if (type == typeof(Decimal)) { return 0m; }
                    if (type == typeof(Guid)) { return Guid.Empty; }
                    if (type == typeof(TimeSpan)) { return new TimeSpan(0, 0, 0); }
                }
            }

            return null;
        }
    }
}
