using IBatisNetSelf.DataMapper.Configuration.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Statements
{
    /// <summary>
    /// Summary description for ISql.
    /// </summary>
    public interface IStatement
    {

        #region Properties
        /// <summary>
        /// Identifier used to identify the statement amongst the others.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Allow remapping of dynamic SQL
        /// </summary>
        bool AllowRemapping { get; set; }


        /// <summary>
        /// The type of the statement (text or procedure).
        /// </summary>
        CommandType  commandType { get; }


        /// <summary>
        /// Extend statement attribute
        /// </summary>
        string ExtendStatement { get; set; }


        /// <summary>
        /// The sql statement to execute.
        /// </summary>
        ISql Sql { get; set; }


        /// <summary>
        /// The ResultMaps used by the statement.
        /// </summary>
        ResultMapCollection ResultsMap { get;}


        /// <summary>
        /// The parameterMap used by the statement.
        /// </summary>
        ParameterMap ParameterMap { get; set; }


        /// <summary>
        /// The CacheModel used by this statement.
        /// </summary>
     //   CacheModel CacheModel { get;set;}


        /// <summary>
        /// The CacheModel name to use.
        /// </summary>
        string CacheModelName { get;set;}


        /// <summary>
        /// The list class type to use for strongly typed collection.
        /// </summary>
        Type ListClass { get;}


        /// <summary>
        /// The result class type to used.
        /// </summary>
        Type ResultClass { get;}


        /// <summary>
        /// The parameter class type to used.
        /// </summary>
        Type ParameterClass { get;}

        #endregion

        #region Methods

        /// <summary>
        /// Create an instance of 'IList' class.
        /// </summary>
        /// <returns>An object which implement IList.</returns>
        IList CreateInstanceOfListClass();

        #endregion

    }
}
