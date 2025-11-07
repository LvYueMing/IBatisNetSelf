using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.TypesResolver
{
    /// <summary>
    /// 通过类型名称解析出对应的 <see cref="System.Type"/> 实例。
    /// </summary>
    /// <remarks>
    /// <p>
    /// 设计此类的初衷是为了扩展 System.Type.GetType 方法的能力，
    /// 实现一个更强大、集中式的类型解析机制。
    /// </p>
    /// </remarks>
    internal class TypeResolver : ITypeResolver
    {
        // 常量：System.Nullable 的类型名称（目前未在代码中使用）
        private const string NULLABLE_TYPE = "System.Nullable";

        #region ITypeResolver Members

        /// <summary>
        /// 将指定的 <paramref name="typeName"/> 字符串解析为一个 <see cref="System.Type"/> 实例。
        /// </summary>
        /// <param name="typeName">待解析的类型名称字符串</param>
        /// <returns>解析成功的 Type 实例</returns>
        /// <exception cref="System.TypeLoadException">如果无法解析该类型，将抛出异常</exception>
        public virtual Type Resolve(string typeName)
        {
            return ResolveType(typeName.Replace(" ", string.Empty));
        }

        #endregion

        /// <summary>
        /// 实际处理类型解析的内部方法
        /// </summary>
        private Type ResolveType(string typeName)
        {
            // 参数合法性检查
            if (typeName == null || typeName.Trim().Length == 0)
            {
                throw BuildTypeLoadException("\"\"或null");
            }

            // 解析类型名称和程序集名
            TypeAssemblyInfo _typeInfo = new TypeAssemblyInfo(typeName);
            Type? _type = null;
            try
            {
                // 如果是带程序集限定名的（例如 TypeName, AssemblyName）
                // 直接从指定的程序集加载，否则遍历所有已加载程序集查找类型
                _type = (_typeInfo.IsAssemblyQualified) ? LoadTypeDirectlyFromAssembly(_typeInfo) : LoadTypeByIteratingOverAllLoadedAssemblies(_typeInfo);
            }
            catch (Exception ex)
            {
                throw BuildTypeLoadException(typeName, ex);
            }
            if (_type == null)
            {
                throw BuildTypeLoadException(typeName);
            }
            return _type;

        }

        /// <summary>
        /// 从指定的程序集直接加载类型（用于带程序集限定名的情况）
        /// </summary>
        /// <param name="typeInfo">封装了类型名称与程序集名称的对象</param>
        /// <returns>返回 Type 实例，若失败则为 null</returns>
        private static Type? LoadTypeDirectlyFromAssembly(TypeAssemblyInfo typeInfo)
        {
            if (string.IsNullOrWhiteSpace(typeInfo.AssemblyName))
            {
                return null;
            }

            try
            {
                // 构造程序集路径，例如 "MyAssembly" => "AppContext.BaseDirectory\MyAssembly.dll"
                string assemblyFileName = typeInfo.AssemblyName + ".dll";
                string assemblyPath = Path.Combine(AppContext.BaseDirectory, assemblyFileName);

                Assembly assembly = null;
                // 使用 AssemblyLoadContext 默认上下文加载程序集
                if (File.Exists(assemblyPath))
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                }
                else
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(typeInfo.AssemblyName));
                }


                // 加载类型，忽略大小写，不抛异常
                return assembly.GetType(typeInfo.TypeName, throwOnError: true, ignoreCase: true);
            }
            catch (Exception ex)
            {
                // 可选：记录日志或包装异常
                throw new TypeLoadException(
                    $"无法从程序集 '{typeInfo.AssemblyName}' 加载类型 '{typeInfo.TypeName}'。", ex);
            }
        }

        /// <summary>
        /// 遍历当前应用程序域中所有已加载的程序集以查找类型
        /// </summary>
        /// <param name="typeInfo">封装了类型名称的对象</param>
        /// <returns>找到的 Type，或 null</returns>
        private static Type? LoadTypeByIteratingOverAllLoadedAssemblies(TypeAssemblyInfo typeInfo)
        {
            Type? _type = null;
            Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in _assemblies)
            {
                _type = assembly.GetType(typeInfo.TypeName, false, false);
                if (_type != null)
                {
                    break;
                }
            }
            return _type;
        }



        #region Exception
        private static TypeLoadException BuildTypeLoadException(string typeName)
        {
            return new TypeLoadException($"Could not load type from string value '{typeName}'.");
        }

        private static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
        {
            return new TypeLoadException($"Could not load type from string value '{typeName}'.", ex);
        }
        #endregion
    }
}
