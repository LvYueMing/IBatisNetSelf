using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="ReflectionPropertySetAccessor"/> class provides an reflection set access   
    /// to a property of a specified target class.
    /// </summary>
    public sealed class ReflectionPropertySetAccessor : ISetAccessor
    {
        private PropertyInfo propertyInfo = null;
        private string propertyName = string.Empty;
        private Type targetType = null;

        #region IAccessor Members

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name=> this.propertyName;

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public Type MemberType=>this.propertyInfo.PropertyType;

        #endregion

        #region ISet Members

        /// <summary>
        /// Sets the value for the property of the specified target.
        /// </summary>
        /// <param name="aTarget">Object to set the property on.</param>
        /// <param name="aValue">Property value.</param>
        public void Set(object aTarget, object aValue)
        {
            if (this.propertyInfo.CanWrite)
            {
                this.propertyInfo.SetValue(aTarget, aValue, null);
            }
            else
            {
                throw new NotSupportedException(
                    $"Property \"{propertyName}\" on type {targetType} doesn't have a set method.");
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionPropertySetAccessor"/> class.
        /// </summary>
        /// <param name="aTargetType">Type of the target.</param>
        /// <param name="aPropertyName">Name of the property.</param>
		public ReflectionPropertySetAccessor(Type aTargetType, string aPropertyName)
        {
            ReflectionInfo _reflectionCache = ReflectionInfo.GetInstance(aTargetType);
            this.propertyInfo = (PropertyInfo)_reflectionCache.GetSetter(aPropertyName);

            this.targetType = aTargetType;
            this.propertyName = aPropertyName;
        }

    }
}
