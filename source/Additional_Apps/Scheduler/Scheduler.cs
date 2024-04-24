using System;
using System.Collections.Generic;
using System.Timers;
using Meadow;

namespace MeadowApp
{
    public class Scheduler
    {
        public event EventHandler<SchedulerEventArgs> EventElapsed;
        protected List<EventTime> _eventTimes;
        protected Timer _timer;
        
        public Scheduler()
        {
            _eventTimes = new List<EventTime>();
        }
        
        public void AddEventUtc(string eventId, int hour, int min, int sec, bool dailyRecurring = false)
        {
            var et = new EventTime()
            {
                Id = eventId,
                Hour = hour,
                Minute = min,
                Second = sec,
                Recurring = dailyRecurring
            };
            _eventTimes.Add(et);
            Resolver.Log.Trace($"scheduler: added \"{eventId}\" for {et.ToTimeString()}");
        }

        public void Start()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += (sender, args) =>
            {
                foreach (var ev in _eventTimes)
                {
                    var now = DateTime.UtcNow;
                    if (ev.Hour == now.Hour && ev.Minute == now.Minute && ev.Second == now.Second)
                    {
                        EventElapsed?.Invoke(this, new SchedulerEventArgs()
                        {
                            EventId = ev.Id
                        });
                        
                        if (!ev.Recurring)
                        {
                            _eventTimes.Remove(ev);
                        }
                    }
                }
            };
            
            _timer.Start();
            _timer.AutoReset = true;
            
            Resolver.Log.Trace("Scheduler started");
        }

        public void Stop()
        {
            _timer.Stop();
            Resolver.Log.Trace("Scheduler stopped");
        }
        
        protected class EventTime
        {
            public string Id { get; set; }
            public int Hour { get; set; }
            public int Minute { get; set; }
            public int Second { get; set; }
            public bool Recurring { get; set; }

            public string ToTimeString()
            {
                return
                    $"{Hour.ToString().PadLeft(2, '0')}:{Minute.ToString().PadLeft(2, '0')}:{Second.ToString().PadLeft(2, '0')}";
            }
        }
    }

    public class SchedulerEventArgs : EventArgs
    {
        public string EventId { get; set; }
    }
}