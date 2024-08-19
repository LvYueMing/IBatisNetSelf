using IBatisNetSelf.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities.Objects
{
    /// <summary>
    /// A wrapper arround an <see cref="IFactory"/> implementation which logs argument type and value
    /// when CreateInstance is called.
    /// </summary>
    public class FactoryLogAdapter : IFactory
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IFactory factory = null;
        private string typeName = string.Empty;
        private string parametersTypeName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryLogAdapter"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="paramtersTypes">The paramters types.</param>
        /// <param name="factory">The factory.</param>
        public FactoryLogAdapter(Type type, Type[] paramtersTypes, IFactory factory)
        {
            this.factory = factory;
            this.typeName = type.FullName;
            this.parametersTypeName = GenerateParametersName(paramtersTypes);
        }

        #region IFactory Members

        /// <summary>
        /// Create a new instance with the specified parameters
        /// </summary>
        /// <param name="parameters">An array of values that matches the number, order and type
        /// of the parameters for this constructor.</param>
        /// <returns>A new instance</returns>
        /// <remarks>
        /// If you call a constructor with no parameters, pass null.
        /// Anyway, what you pass will be ignore.
        /// </remarks>
        public object CreateInstance(object[] parameters)
        {
            object _newObject = null;

            try
            {
                _newObject = this.factory.CreateInstance(parameters);
            }
            catch
            {
                _logger.Debug("Enabled to create instance for type '" + typeName);
                _logger.Debug("  using parameters type : " + parametersTypeName);
                _logger.Debug("  using parameters value : " + GenerateLogInfoForParameterValue(parameters));
                throw;
            }

            return _newObject;
        }

        #endregion

        /// <summary>
        /// Generates the a string containing all parameter type names.
        /// </summary>
        /// <param name="arguments">The types of the constructor arguments</param>
        /// <returns>The string.</returns>
        private string GenerateParametersName(object[] arguments)
        {
            StringBuilder names = new StringBuilder();
            if ((arguments != null) && (arguments.Length != 0))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    names.Append("[").Append(arguments[i]).Append("] ");
                }
            }
            return names.ToString();
        }

        /// <summary>
        /// Generates the a string containing all parameters value.
        /// </summary>
        /// <param name="arguments">The arguments</param>
        /// <returns>The string.</returns>
        private string GenerateLogInfoForParameterValue(object[] arguments)
        {
            StringBuilder values = new StringBuilder();
            if ((arguments != null) && (arguments.Length != 0))
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] != null)
                    {
                        values.Append("[").Append(arguments[i].ToString()).Append("] ");
                    }
                    else
                    {
                        values.Append("[null] ");
                    }
                }
            }
            return values.ToString();
        }
    }
}
