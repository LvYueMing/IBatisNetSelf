using IBatisNetSelf.Common.Utilities.Objects.Members;
using IBatisNetSelf.Common.Utilities.Objects;
using IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Configuration.Sql.Dynamic.Handlers
{
    /// <summary>
    /// Summary description for IterateTagHandler.
    /// </summary>
    public sealed class IterateTagHandler : BaseTagHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IterateTagHandler"/> class.
        /// </summary>
        /// <param name="accessorFactory">The accessor factory.</param>
        public IterateTagHandler(AccessorFactory accessorFactory)
            : base(accessorFactory)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <returns></returns>
        public override int DoStartFragment(SqlTagContext ctx, SqlTag tag, object parameterObject)
        {
            IterateContext iterate = (IterateContext)ctx.GetAttribute(tag);
            if (iterate == null)
            {
                string propertyName = ((BaseTag)tag).Property;
                object collection;
                if (propertyName != null && propertyName.Length > 0)
                {
                    collection = ObjectProbe.GetMemberValue(parameterObject, propertyName, this.AccessorFactory);
                }
                else
                {
                    collection = parameterObject;
                }
                iterate = new IterateContext(collection);
                ctx.AddAttribute(tag, iterate);
            }
            if (iterate != null && iterate.HasNext)
            {
                return BaseTagHandler.INCLUDE_BODY;
            }
            else
            {
                return BaseTagHandler.SKIP_BODY;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <param name="bodyContent"></param>
        public override void DoPrepend(SqlTagContext ctx, SqlTag tag, object parameterObject, StringBuilder bodyContent)
        {
            IterateContext iterate = (IterateContext)ctx.GetAttribute(tag);
            if (iterate.IsFirst)
            {
                base.DoPrepend(ctx, tag, parameterObject, bodyContent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag"></param>
        /// <param name="parameterObject"></param>
        /// <param name="bodyContent"></param>
        /// <returns></returns>
        public override int DoEndFragment(SqlTagContext ctx, SqlTag tag,
            object parameterObject, StringBuilder bodyContent)
        {
            IterateContext iterate = (IterateContext)ctx.GetAttribute(tag);

            if (iterate.MoveNext())
            {
                string propertyName = ((BaseTag)tag).Property;
                if (propertyName == null)
                {
                    propertyName = "";
                }

                string find = propertyName + "[]";
                string replace = propertyName + "[" + iterate.Index + "]";//Parameter-index-Dynamic
                Replace(bodyContent, find, replace);

                if (iterate.IsFirst)
                {
                    string open = ((Iterate)tag).Open;
                    if (open != null)
                    {
                        bodyContent.Insert(0, open);
                        bodyContent.Insert(0, ' ');
                    }
                }
                if (!iterate.IsLast)
                {
                    string conjunction = ((Iterate)tag).Conjunction;
                    if (conjunction != null)
                    {
                        bodyContent.Append(conjunction);
                        bodyContent.Append(' ');
                    }
                }
                if (iterate.IsLast)
                {
                    string close = ((Iterate)tag).Close;
                    if (close != null)
                    {
                        bodyContent.Append(close);
                    }
                }

                return BaseTagHandler.REPEAT_BODY;
            }
            else
            {
                return BaseTagHandler.INCLUDE_BODY;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        private static void Replace(StringBuilder buffer, string find, string replace)
        {
            int start = buffer.ToString().IndexOf(find);
            int length = find.Length;
            while (start > -1)
            {
                buffer = buffer.Replace(find, replace, start, length);
                start = buffer.ToString().IndexOf(find);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsPostParseRequired
        {
            get
            {
                return true;
            }
        }
    }
}
