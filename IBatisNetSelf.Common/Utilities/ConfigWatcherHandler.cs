using IBatisNetSelf.Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBatisNetSelf.Common.Utilities
{
    /// <summary>
    /// Represents the method that handles calls from Configure.
    /// </summary>
    /// <remarks>
    /// obj is a null object in a DaoManager context.
    /// obj is the reconfigured sqlMap in a SqlMap context.
    /// </remarks>
    public delegate void ConfigureHandler(object obj);

    /// <summary>
    /// 
    /// </summary>
    public struct StateConfig
    {
        /// <summary>
        /// Master Config File name.
        /// </summary>
        public string FileName;
        /// <summary>
        /// Delegate called when a file is changed, use it to rebuild.
        /// </summary>
        public ConfigureHandler ConfigureHandler;
    }

    /// <summary>
    /// Class used to watch config files.
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="FileSystemWatcher"/> to monitor
    /// changes to a specified file. Because multiple change notifications
    /// may be raised when the file is modified, a timer is used to
    /// compress the notifications into a single event. The timer
    /// waits for the specified time before delivering
    /// the event notification. If any further <see cref="FileSystemWatcher"/>
    /// change notifications arrive while the timer is waiting it
    /// is reset and waits again for the specified time to
    /// elapse.
    /// </remarks>
    public sealed class ConfigWatcherHandler
    {
        #region Fields
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The timer used to compress the notification events.
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// A list of configuration files to watch.
        /// </summary>
        private static ArrayList filesToWatch = new ArrayList();

        /// <summary>
        /// The list of FileSystemWatcher.
        /// </summary>
        private static ArrayList filesWatcher = new ArrayList();

        /// <summary>
        /// The default amount of time to wait after receiving notification
        /// before reloading the config file.
        /// </summary>
        private const int TIMEOUT_MILLISECONDS = 500;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        ///-
        /// </summary>
        /// <param name="state">
        /// Represent the call context of the SqlMap or DaoManager ConfigureAndWatch method call.
        /// </param>
        /// <param name="onWhatchedFileChange"></param>
        public ConfigWatcherHandler(TimerCallback onWhatchedFileChange, StateConfig state)
        {
            for (int index = 0; index < filesToWatch.Count; index++)
            {
                FileInfo configFile = (FileInfo)filesToWatch[index];

                this.AttachWatcher(configFile);

                // Create the timer that will be used to deliver events. Set as disabled
                // callback  : A TimerCallback delegate representing a method to be executed. 
                // state : An object containing information to be used by the callback method, or a null reference 
                // dueTime : The amount of time to delay before callback is invoked, in milliseconds. Specify Timeout.Infinite to prevent the timer from starting. Specify zero (0) to start the timer immediately
                // dueTime : 在调用回调之前延迟的时间，以毫秒为单位。指定Timeout.Infinite以防止计时器启动。指定0(0)立即启动计时器
                // period : The time interval between invocations of callback, in milliseconds. Specify Timeout.Infinite to disable periodic signaling
                // period : 回调调用之间的时间间隔，以毫秒为单位。指定Timeout.Infinite禁用周期性信令
                this.timer = new Timer(onWhatchedFileChange, state, Timeout.Infinite, Timeout.Infinite);
            }
        }
        #endregion

        #region Methods

        private void AttachWatcher(FileInfo configFile)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher _watcher = new FileSystemWatcher();

            _watcher.Path = configFile.DirectoryName;
            _watcher.Filter = configFile.Name;

            // Set the notification filters
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

            // Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
            _watcher.Changed += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _watcher.Created += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _watcher.Renamed += new RenamedEventHandler(ConfigWatcherHandler_OnRenamed);

            // Begin watching.
            _watcher.EnableRaisingEvents = true;

            filesWatcher.Add(_watcher);
        }

        /// <summary>
        /// Add a file to be monitored.
        /// </summary>
        /// <param name="configFile"></param>
        public static void AddFileToWatch(FileInfo configFile)
        {
            if (_logger.IsDebugEnabled)
            {
                // TODO: remove Path.GetFileName?
                _logger.Debug("Adding file [" + Path.GetFileName(configFile.FullName) + "] to list of watched files.");
            }

            filesToWatch.Add(configFile);
        }

        /// <summary>
        /// Reset the list of files being monitored.
        /// </summary>
        public static void ClearFilesMonitored()
        {
            filesToWatch.Clear();

            // Kill all FileSystemWatcher
            for (int index = 0; index < filesWatcher.Count; index++)
            {
                FileSystemWatcher fileWatcher = (FileSystemWatcher)filesWatcher[index];

                fileWatcher.EnableRaisingEvents = false;
                fileWatcher.Dispose();
            }
        }

        /// <summary>
        /// Event handler used by <see cref="ConfigWatcherHandler"/>.
        /// </summary>
        /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
        /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
        /// <remarks>
        /// This handler reloads the configuration from the file when the event is fired.
        /// </remarks>
        private void ConfigWatcherHandler_OnChanged(object source, FileSystemEventArgs e)
        {
            //if (_logger.IsDebugEnabled)
            //{
            //	_logger.Debug("ConfigWatcherHandler : " + e.ChangeType + " [" + e.Name + "]");
            //}

            // timer will fire only once
            this.timer.Change(TIMEOUT_MILLISECONDS, Timeout.Infinite);
        }

        /// <summary>
        /// Event handler used by <see cref="ConfigWatcherHandler"/>.
        /// </summary>
        /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
        /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
        /// <remarks>
        /// This handler reloads the configuration from the file when the event is fired.
        /// </remarks>
        private void ConfigWatcherHandler_OnRenamed(object source, RenamedEventArgs e)
        {
            //if (_logger.IsDebugEnabled)
            //{
            //	_logger.Debug("ConfigWatcherHandler : " + e.ChangeType + " [" + e.OldName + "/" + e.Name + "]");
            //}

            // timer will fire only once
            this.timer.Change(TIMEOUT_MILLISECONDS, Timeout.Infinite);
        }
        #endregion

    }
}
