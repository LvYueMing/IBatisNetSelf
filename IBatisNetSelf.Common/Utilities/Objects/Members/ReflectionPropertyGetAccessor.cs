using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects.Members
{
    /// <summary>
    /// The <see cref="ReflectionPropertyGetAccessor"/> class provides an reflection get access   
    /// to a property of a specified target class.
    /// </summary>
    public sealed class ReflectionPropertyGetAccessor : IGetAccessor
    {
        private PropertyInfo propertyInfo = null;
        private string propertyName = string.Empty;
        private Type targetType = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionPropertyGetAccessor"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ReflectionPropertyGetAccessor(Type targetType, string propertyName)
        {
            ReflectionInfo reflectionCache = ReflectionInfo.GetInstance(targetType);
            this.propertyInfo = (PropertyInfo)reflectionCache.GetGetter(propertyName);

            this.targetType = targetType;
            this.propertyName = propertyName;
        }


        #region IAccessor Members

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name=>this.propertyInfo.Name;

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public Type MemberType=>this.propertyInfo.PropertyType;

        #endregion

        #region IGet Members

        /// <summary>
        /// Gets the value stored in the property for 
        /// the specified target.
        /// </summary>
        /// <param name="target">Object to retrieve the property from.</param>
        /// <returns>Property value.</returns>
        public object Get(object target)
        {
            if (this.propertyInfo.CanRead)
            {
                return this.propertyInfo.GetValue(target, null);
            }
            else
            {
                throw new NotSupportedException(
                    $"Property \"{this.propertyName}\" on type {this.targetType} doesn't have a get method.");
            }
        }

        #endregion
    }
}
