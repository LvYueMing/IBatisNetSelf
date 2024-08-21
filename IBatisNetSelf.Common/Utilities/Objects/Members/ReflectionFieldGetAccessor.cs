using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="ReflectionFieldGetAccessor"/> class provides an reflection get access   
    /// to a field of a specified target class.
    /// </summary>
    public sealed class ReflectionFieldGetAccessor : IGetAccessor
    {
        private FieldInfo fieldInfo = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionFieldGetAccessor"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="fieldName">Name of the field.</param>
        public ReflectionFieldGetAccessor(Type targetType, string fieldName)
        {
            ReflectionInfo _reflectionCache = ReflectionInfo.GetInstance(targetType);
            this.fieldInfo = (FieldInfo)_reflectionCache.GetGetter(fieldName);
        }

        #region IGetAccessor Members

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string Name=>this.fieldInfo.Name;

        /// <summary>
        /// Gets the type of this member, such as field, property.
        /// </summary>
        public Type MemberType => this.fieldInfo.FieldType;

        /// <summary>
        /// Gets the value stored in the field for the specified target.       
        /// </summary>
        /// <param name="target">Object to retrieve the field/property from.</param>
        /// <returns>The field alue.</returns>
        public object Get(object target)
        {
            return this.fieldInfo.GetValue(target);
        }

        #endregion
    }
}
