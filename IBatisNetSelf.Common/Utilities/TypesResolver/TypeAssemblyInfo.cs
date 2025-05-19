using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.TypesResolver
{
    /// <summary>
    /// Holds data about a <see cref="System.Type"/> and it's attendant <see cref="System.Reflection.Assembly"/>.
    /// </summary>
    internal class TypeAssemblyInfo
    {
        #region Constants

        /// <summary>
        /// The string that separates a <see cref="System.Type"/> name
        /// from the name of it's attendant <see cref="System.Reflection.Assembly"/>
        /// in an assembly qualified type name.
        /// </summary>
        public const string TYPE_ASSEMBLY_SEPARATOR = ",";
        public const string NULLABLE_TYPE = "System.Nullable";
        public const string NULLABLE_TYPE_ASSEMBLY_SEPARATOR = "]],";
        #endregion

        #region Fields

        private string unResolvedAssemblyName = string.Empty;
        private string unResolvedTypeName = string.Empty;

        #endregion

        #region Constructor (s) / Destructor

        /// <summary>
        /// Creates a new instance of the TypeAssemblyInfo class.
        /// </summary>
        /// <param name="unResolvedTypeName">
        /// The unresolved name of a <see cref="System.Type"/>.
        /// </param>
        public TypeAssemblyInfo(string unResolvedTypeName)
        {
            this.SplitTypeAndAssemblyNames(unResolvedTypeName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The (unresolved) type name portion of the original type name.
        /// </summary>
        public string TypeName
        {
            get { return unResolvedTypeName; }
        }

        /// <summary>
        /// The (unresolved, possibly partial) name of the attandant assembly.
        /// </summary>
        public string AssemblyName
        {
            get { return unResolvedAssemblyName; }
        }

        /// <summary>
        /// Is the type name being resolved assembly qualified?
        /// 类型名称是否为程序集限定解析的？
        /// </summary>
        public bool IsAssemblyQualified
        {
            get { return ISQualified(AssemblyName); }
        }

        #endregion

        #region Methods

        private bool ISQualified(string target)
        {
            if (target == null)
            {
                return false;
            }
            else
            {
                return (target != null && target.Trim().Length > 0);
            }
        }

        private void SplitTypeAndAssemblyNames(string aOriginalTypeName)
        {
            if (aOriginalTypeName.StartsWith(NULLABLE_TYPE))
            {
                int _typeAssemblyIndex = aOriginalTypeName.IndexOf(NULLABLE_TYPE_ASSEMBLY_SEPARATOR);
                if (_typeAssemblyIndex < 0)
                {
                    this.unResolvedTypeName = aOriginalTypeName;
                }
                else
                {
                    this.unResolvedTypeName = aOriginalTypeName.Substring(0, _typeAssemblyIndex + 2).Trim();
                    this.unResolvedAssemblyName = aOriginalTypeName.Substring(_typeAssemblyIndex + 3).Trim();
                }
            }
            else
            {
                int _typeAssemblyIndex = aOriginalTypeName.IndexOf(TYPE_ASSEMBLY_SEPARATOR);
                if (_typeAssemblyIndex < 0)
                {
                    this.unResolvedTypeName = aOriginalTypeName;
                }
                else
                {
                    this.unResolvedTypeName = aOriginalTypeName.Substring(0, _typeAssemblyIndex).Trim();
                    this.unResolvedAssemblyName = aOriginalTypeName.Substring(_typeAssemblyIndex + 1).Trim();
                }
            }
        }

        #endregion
    }
}
