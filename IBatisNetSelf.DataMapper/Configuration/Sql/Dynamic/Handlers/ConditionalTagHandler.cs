using IBatisNetSelf.Common.Exceptions;
using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Description for ConditionalTagHandler.
    /// </summary>
    public abstract class ConditionalTagHandler : BaseTagHandler
    {

        #region Const
        /// <summary>
        /// 
        /// </summary>
        public const long NOT_COMPARABLE = long.MinValue;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalTagHandler"/> class.
        /// </summary>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public ConditionalTagHandler(AccessorFactory aAccessorFactory): base(aAccessorFactory)
        {
        }

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public abstract bool IsCondition(SqlTagContext ctx, SqlTag tag, object parameterObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aTag"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        public override int DoStartFragment(SqlTagContext aContext, SqlTag aTag, Object aParameterObject)
        {
            if (IsCondition(aContext, aTag, aParameterObject))
            {
                return BaseTagHandler.INCLUDE_BODY;
            }
            else
            {
                return BaseTagHandler.SKIP_BODY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        public override int DoEndFragment(SqlTagContext ctx, SqlTag tag, Object parameterObject, StringBuilder bodyContent)
        {
            return BaseTagHandler.INCLUDE_BODY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aSqlTag"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        protected long Compare(SqlTagContext aContext, SqlTag aSqlTag, object aParameterObject)
        {
            Conditional _tag = (Conditional)aSqlTag;
            string _propertyName = _tag.Property;
            string _comparePropertyName = _tag.CompareProperty;
            string _compareValue = _tag.CompareValue;

            object _value1 = null;
            Type _type = null;
            if (_propertyName != null && _propertyName.Length > 0)
            {
                _value1 = ObjectProbe.GetMemberValue(aParameterObject, _propertyName, this.AccessorFactory);
                _type = _value1.GetType();
            }
            else
            {
                _value1 = aParameterObject;
                if (_value1 != null)
                {
                    _type = aParameterObject.GetType();
                }
                else
                {
                    _type = typeof(object);
                }
            }
            if (_comparePropertyName != null && _comparePropertyName.Length > 0)
            {
                object _value2 = ObjectProbe.GetMemberValue(aParameterObject, _comparePropertyName, this.AccessorFactory);
                return CompareValues(_type, _value1, _value2);
            }
            else if (_compareValue != null && _compareValue != "")
            {
                return CompareValues(_type, _value1, _compareValue);
            }
            else
            {
                throw new DataMapperException("Error comparing in conditional fragment.  Uknown 'compare to' values.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aType"></param>
        /// <param name="aValue1"></param>
        /// <param name="aValue2"></param>
        /// <returns></returns>
        protected long CompareValues(Type aType, object aValue1, object aValue2)
        {
            long result = NOT_COMPARABLE;

            if (aValue1 == null || aValue2 == null)
            {
                result = aValue1 == aValue2 ? 0 : NOT_COMPARABLE;
            }
            else
            {
                if (aValue2.GetType() != aType)
                {
                    aValue2 = ConvertValue(aType, aValue2.ToString());
                }
                if (aValue2 is string && aType != typeof(string))
                {
                    aValue1 = aValue1.ToString();
                }
                if (!(aValue1 is IComparable && aValue2 is IComparable))
                {
                    aValue1 = aValue1.ToString();
                    aValue2 = aValue2.ToString();
                }
                result = ((IComparable)aValue1).CompareTo(aValue2);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected object ConvertValue(Type type, string value)
        {

            if (type == typeof(String))
            {
                return value;
            }
            else if (type == typeof(bool))
            {
                return System.Convert.ToBoolean(value);
            }
            else if (type == typeof(Byte))
            {
                return System.Convert.ToByte(value);
            }
            else if (type == typeof(Char))
            {
                return System.Convert.ToChar(value.Substring(0, 1));//new Character(value.charAt(0));
            }
            else if (type == typeof(DateTime))
            {
                try
                {
                    return System.Convert.ToDateTime(value);
                }
                catch (Exception e)
                {
                    throw new DataMapperException("Error parsing date. Cause: " + e.Message, e);
                }
            }
            else if (type == typeof(Decimal))
            {
                return System.Convert.ToDecimal(value);
            }
            else if (type == typeof(Double))
            {
                return System.Convert.ToDouble(value);
            }
            else if (type == typeof(Int16))
            {
                return System.Convert.ToInt16(value);
            }
            else if (type == typeof(Int32))
            {
                return System.Convert.ToInt32(value);
            }
            else if (type == typeof(Int64))
            {
                return System.Convert.ToInt64(value);
            }
            else if (type == typeof(Single))
            {
                return System.Convert.ToSingle(value);
            }
            else
            {
                return value;
            }

        }
        #endregion

    }
}
