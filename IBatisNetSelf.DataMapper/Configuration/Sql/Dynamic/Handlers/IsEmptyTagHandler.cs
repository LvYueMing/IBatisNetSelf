using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// IsEmptyTagHandler represent a isEmpty tag element in a dynamic mapped statement.
    /// </summary>
    public class IsEmptyTagHandler : ConditionalTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEmptyTagHandler"/> class.
        /// </summary>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public IsEmptyTagHandler(AccessorFactory aAccessorFactory)
            : base(aAccessorFactory)
        {
        }

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aTag"></param>
        /// <param name="aParameterObject"></param>
        /// <returns></returns>
        public override bool IsCondition(SqlTagContext aContext, SqlTag aTag, object aParameterObject)
        {
            if (aParameterObject == null)
            {
                return true;
            }
            else
            {
                string _propertyName = ((BaseTag)aTag).Property;
                object _value = null;
                if (_propertyName != null && _propertyName.Length > 0)
                {
                    _value = ObjectProbe.GetMemberValue(aParameterObject, _propertyName, this.AccessorFactory);
                }
                else
                {
                    _value = aParameterObject;
                }
                if (_value is ICollection)
                {
                    return ((_value == null) || (((ICollection)_value).Count < 1));
                }
                else if (_value != null && typeof(Array).IsAssignableFrom(_value.GetType())) //value.GetType().IsArray
                {
                    return ((Array)_value).GetLength(0) == 0;
                }
                else
                {
                    return ((_value == null) || (System.Convert.ToString(_value).Equals("")));
                }
            }
        }
        #endregion

    }
}
