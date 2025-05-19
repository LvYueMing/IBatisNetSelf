using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.DataMapper.Scope
{
    /// <summary>
    /// An error context to help us create meaningful error messages.
    /// </summary>
    public class ErrorContext
    {
        #region Fields

        private string resource = string.Empty;
        private string activity = string.Empty;
        private string objectId = string.Empty;
        private string moreInfo = string.Empty;
        #endregion

        #region Properties

        /// <summary>
        /// The resource causing the problem
        /// </summary>
        public string Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        /// <summary>
        /// The activity that was happening when the error happened
        /// </summary>
        public string Activity
        {
            get { return activity; }
            set { activity = value; }
        }

        /// <summary>
        /// The object ID where the problem happened
        /// </summary>
        public string ObjectId
        {
            get { return objectId; }
            set { objectId = value; }
        }

        /// <summary>
        /// More information about the error
        /// </summary>
        public string MoreInfo
        {
            get { return moreInfo; }
            set { moreInfo = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the error context
        /// </summary>
        public void Reset()
        {
            resource = string.Empty;
            activity = string.Empty;
            objectId = string.Empty;
            moreInfo = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder _message = new StringBuilder();

            // activity
            if (activity != null && activity.Length > 0)
            {
                _message.Append(Environment.NewLine);
                _message.Append("- The error occurred while ");
                _message.Append(activity);
                _message.Append(".");
            }

            // more info
            if (moreInfo != null && moreInfo.Length > 0)
            {
                _message.Append(Environment.NewLine);
                _message.Append("- ");
                _message.Append(moreInfo);
            }

            // resource
            if (resource != null && resource.Length > 0)
            {
                _message.Append(Environment.NewLine);
                _message.Append("- The error occurred in ");
                _message.Append(resource);
                _message.Append(".");
            }

            // object
            if (objectId != null && objectId.Length > 0)
            {
                _message.Append("  ");
                _message.Append(Environment.NewLine);
                _message.Append("- Check the ");
                _message.Append(objectId);
                _message.Append(".");
            }

            return _message.ToString();
        }

        #endregion
    }
}
