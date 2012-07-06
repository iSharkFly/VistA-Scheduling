/* Written by Sam Habiel in May 2011
 * Licensed under LGPL */

using System;
using System.Collections.Generic;
using System.Text;
using IndianHealthService.BMXNet;
using System.Diagnostics;

namespace IndianHealthService.ClinicalScheduling
{
    /// <summary>
    /// Class to Log Calls to RPMS/VISTA back and forth. Implements BMXNet.[I]Log interaface.
    /// Logger is implemented as a Queue Collection of class EventToLog
    /// </summary>
    public class RPCLogger: Log
    {
        /// <summary>
        /// Max size of Log
        /// </summary>
        const int maxsize = 1000;

        /// <summary>
        /// Stop Watch to keep track of time between calls.
        /// </summary>
        Stopwatch _watch;

        /// <summary>
        /// ctor
        /// </summary>
        public RPCLogger()
        {
            _logger = new List<EventToLog>(maxsize);
            _watch = new Stopwatch();
            _watch.Start();
        }

        public bool IsLogging { get; set; }

        //Event to notify interested controls that we have more data
        public event EventHandler<EventToLog> HaveMoreData;

        private List<EventToLog> _logger;
        
        public List<EventToLog> Logger
        {
        get { return _logger; } 
        }

        /// <summary>
        /// Data Structure to Log
        /// </summary>
        public class EventToLog: EventArgs
        {
            public DateTime EventTime { get; set; }
            public long ElapasedTime { get; set; }
            public string Class { get; set; }
            public string Category { get; set; }
            public string Lines { get; set; }
            public Exception Exception { get; set; }

            public override string ToString()
            {
                return EventTime.TimeOfDay + "\t" + Category + "\t" + Class + "\t" + ElapasedTime + " ms";
            }
        }

        /// <summary>
        /// Chained to Below
        /// </summary>
        public void Log(string aClass, string aCategory, params string[] lines)
        {
            Log(aClass, aCategory, null, lines);
        }

        /// <summary>
        /// Adds Log entry to queue object
        /// </summary>
        public void Log(string aClass, string aCategory, Exception anException, params string[] lines)
        {
            if (_logger.Count >= maxsize - 1) _logger.RemoveAt(_logger.Count - 1);

            EventToLog _e = new EventToLog
            {
                EventTime = DateTime.Now,
                Class = aClass,
                Category = aCategory,
                Lines = String.Join("\r\n", lines),
                Exception = anException,
                ElapasedTime = _watch.ElapsedMilliseconds
            };

            _logger.Add(_e);

            _watch.Reset();
            _watch.Start();

            //Tell subscribers to this event that we want attention!!!!
            if (HaveMoreData != null) HaveMoreData(this, _e);
        }
    }
}
