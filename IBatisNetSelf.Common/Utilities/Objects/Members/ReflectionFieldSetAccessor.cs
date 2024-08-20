using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="ReflectionFieldSetAccessor"/> class provides an reflection set access   
    /// to a field of a specified target class.
    /// </summary>
    public sealed class ReflectionFieldSetAccessor : ISetAccessor
    {
        private FieldInfo fieldInfo = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionFieldSetAccessor"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="fieldName">Name of the field.</param>
        public ReflectionFieldSetAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo reflectionCache = ReflectionInfo.GetInstance(targetType);
            fieldInfo = (FieldInfo)reflectionCache.GetGetter(fieldName);
        }

        #region ISetAccessor Members

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string Name=>fieldInfo.Name;

        /// <summary>
        /// Gets the type of this member, such as field, property.
        /// </summary>
        public Type MemberType=>fieldInfo.FieldType;

        /// <summary>
        /// Sets the value for the field of the specified target.
        /// </summary>
        /// <param name="target">Object to set the property on.</param>
        /// <param name="value">Property value.</param>
        public void Set(object target, object value)
        {
            fieldInfo.SetValue(target, value);
        }

        #endregion
    }
}
