using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Description for BaseTagHandler.
    /// </summary>
    public abstract class BaseTagHandler : ISqlTagHandler
    {

        #region Const
        /// <summary>
        /// BODY TAG
        /// </summary>
        public const int SKIP_BODY = 0;
        /// <summary>
        /// 
        /// </summary>
        public const int INCLUDE_BODY = 1;
        /// <summary>
        /// 
        /// </summary>
        public const int REPEAT_BODY = 2;
        #endregion

        private AccessorFactory accessorFactory = null;


        #region ISqlTagHandler Members

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsPostParseRequired
        {
            get
            {
                return false;
            }
        }


        #endregion
        /// <summary>
        /// The factory which build <see cref="IAccessor"/>
        /// </summary>
        public AccessorFactory AccessorFactory => this.accessorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTagHandler"/> class.
        /// </summary>
        /// <param name="aAccessorFactory">The accessor factory.</param>
        public BaseTagHandler(AccessorFactory aAccessorFactory)
        {
            this.accessorFactory = aAccessorFactory;
        }

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public virtual int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            return BaseTagHandler.INCLUDE_BODY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        public virtual int DoEndFragment(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent)
        {
            return BaseTagHandler.INCLUDE_BODY;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aContext"></param>
        /// <param name="aSqlTag"></param>
        /// <param name="aParameterObject"></param>
        /// <param name="aBodyContent"></param>
        public virtual void DoPrepend(SqlTagContext aContext, SqlTag aSqlTag, object aParameterObject, StringBuilder aBodyContent)
        {
            if (aSqlTag.IsPrependAvailable)
            {
                if (aBodyContent.ToString().Trim().Length > 0)
                {
                    if (aContext.IsOverridePrepend && aSqlTag == aContext.FirstNonDynamicTagWithPrepend)
                    {
                        aContext.IsOverridePrepend = false;
                    }
                    else
                    {
                        aBodyContent.Insert(0, aSqlTag.Prepend);
                    }
                }
                else
                {
                    if (aContext.FirstNonDynamicTagWithPrepend != null)
                    {
                        aContext.FirstNonDynamicTagWithPrepend = null;
                        aContext.IsOverridePrepend = true;
                    }
                }
            }
        }


        #endregion

    }
}
