using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IBatisNetSelf.DataMapper.Configuration.Cache
{
    /// <summary>
    /// Summary description for FlushInterval.
    /// </summary>
    [Serializable]
    [XmlRoot("flushInterval")]
    public class FlushInterval
    {

        #region Fields 

        private int hours = 0;
        private int minutes = 0;
        private int seconds = 0;
        private int milliseconds = 0;
        private long interval = CacheModel.NO_FLUSH_INTERVAL;

        #endregion

        #region Properties
        /// <summary>
        /// Flush interval in hours
        /// </summary>
        [XmlAttribute("hours")]
        public int Hours
        {
            get=>this.hours;
            set=>this.hours = value;
        }


        /// <summary>
        /// Flush interval in minutes
        /// </summary>
        [XmlAttribute("minutes")]
        public int Minutes
        {
            get=>this.minutes;
            set=>this.minutes = value;
        }


        /// <summary>
        /// Flush interval in seconds
        /// </summary>
        [XmlAttribute("seconds")]
        public int Seconds
        {
            get=>this.seconds;
            set=>this.seconds = value;

        }


        /// <summary>
        /// Flush interval in milliseconds
        /// </summary>
        [XmlAttribute("milliseconds")]
        public int Milliseconds
        {
            get=>this.milliseconds;
            set=>this.milliseconds = value;
        }


        /// <summary>
        /// Get the flush interval value
        /// </summary>
        [XmlIgnore]
        public long Interval=>this.interval;
        #endregion

        #region Methods
        /// <summary>
        /// Calcul the flush interval value in ticks
        /// </summary>
        public void Initialize()
        {
            long interval = 0;
            if (milliseconds != 0)
            {
                interval += (milliseconds * TimeSpan.TicksPerMillisecond);
            }
            if (seconds != 0)
            {
                interval += (seconds * TimeSpan.TicksPerSecond);
            }
            if (minutes != 0)
            {
                interval += (minutes * TimeSpan.TicksPerMinute);
            }
            if (hours != 0)
            {
                interval += (hours * TimeSpan.TicksPerHour);
            }

            if (interval == 0)
            {
                interval = CacheModel.NO_FLUSH_INTERVAL;
            }
            this.interval = interval;
        }
        #endregion

    }
}
