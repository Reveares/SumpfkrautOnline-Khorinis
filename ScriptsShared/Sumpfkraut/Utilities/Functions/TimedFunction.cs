﻿using GUC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUC.Scripts.Sumpfkraut.Utilities.Functions
{

    public class TimedFunction : ExtendedObject
    {

        new public static readonly string _staticName = "TimedFunction (s)";



        // used to lock changes on the TimedAction-object
        protected object _lock;

        // Action to be performed with any number of parameters provided 
        // (convertion of objects to their respective types necessary for
        // effective use => avoid when possible and / or use surrounding
        // function scope to define parameters which then can be used by the
        // Action as inner function with access to that scope
        // ... which prevents garbage collection / closing of the surrounding scope)
        protected Func<object[], object[]> func;
        public Func<object[], object[]> GetFunc () { lock (_lock) { return func; } }
        public void SetFunc (Func<object[], object[]> value) { lock (_lock) { func = value; } }

        // state of parameters used by the surrounded function as well as passed to it if invoked again
        // (provide starter set of parameters if necessary)
        protected object[] parameters;
        public object[] GetParameters () { lock (_lock) { return parameters; } }
        public void SetParameters (object[] value) { lock (_lock) { parameters = value; } }

        protected long invocations;
        public long GetInvocations () { lock (_lock) { return invocations; } }
        public void SetInvocations (long value) { lock (_lock) { invocations = value; } }
        public void IterateInvocations () { lock (_lock) { invocations++; } }

        protected bool hasMaxInvocations;
        public bool HasMaxInvocations { get { return hasMaxInvocations; } }
        protected long maxInvocations;
        public long GetMaxInvocations () { return maxInvocations; }

        protected bool hasSpecifiedTimes;
        public bool HasSpecifiedTimes { get { return hasSpecifiedTimes; } }
        protected DateTime[] specifiedTimes;
        public DateTime[] GetSpecifiedTimes () { return specifiedTimes; }

        protected bool hasIntervals;
        public bool HasIntervals { get { return hasIntervals; } }
        protected TimeSpan[] intervals;
        public TimeSpan[] GetIntervals () { return intervals; }

        protected DateTime lastIntervalTime;
        public DateTime GetLastIntervalTime () { lock (_lock) { return lastIntervalTime; } }
        public void SetLastIntervalTime (DateTime value) { lock (_lock) { lastIntervalTime = value; } }

        protected int lastIntervalIndex;
        public int GetLastIntervalIndex () { lock (_lock) { return lastIntervalIndex; } }
        public void SetLastIntervalIndex (int value)
        {
            lock (_lock)
            {
                if (value < 0) { lastIntervalIndex = intervals.Length - 1; }
                else if (value < intervals.Length) { lastIntervalIndex = value; }
                else { lastIntervalIndex = 0; }
            }
        }
        public int NextIntervalIndex ()
        {
            int index;
            lock (_lock)
            {
                index = lastIntervalIndex + 1;
                if (index >= intervals.Length) { index = 0; }
            }

            return index;
        }

        protected bool hasStartEnd;
        public bool HasStartEnd { get { return hasStartEnd; } }
        protected Tuple<DateTime, DateTime> startEnd;
        public Tuple<DateTime, DateTime> GetStartEnd () { return startEnd; }
        public DateTime GetStart () { return startEnd.Item1; }
        public DateTime GetEnd () { return startEnd.Item2; }



        // run the action as soon as possible a single time
        public TimedFunction ()
            : this(null, null, null)
        { }

        // run at specified times until they all passed away
        public TimedFunction (DateTime[] specifiedTimes)
            : this(specifiedTimes, null, null)
        { }

        // run at specified times in a certain time range
        public TimedFunction (DateTime[] specifiedTimes, Tuple<DateTime, DateTime> startEnd)
            : this(specifiedTimes, null, startEnd)
        { }

        // run endlessly at given intervals
        public TimedFunction (TimeSpan[] intervals)
            : this(null, intervals, null)
        { }

        // run at given intervals in a certain time range
        public TimedFunction (TimeSpan[] intervals, Tuple<DateTime, DateTime> startEnd)
            : this(null, intervals, startEnd)
        { }

        // general constructor
        public TimedFunction (DateTime[] specifiedTimes, TimeSpan[] intervals,
            Tuple<DateTime, DateTime> startEnd)
        {
            SetObjName("TimedFunction");
            _lock = new object();
            if (specifiedTimes != null)
            {
                hasSpecifiedTimes = true;
                this.specifiedTimes = specifiedTimes;
            }
            if (intervals != null)
            {
                hasIntervals = true;
                this.intervals = intervals;
            }
            if (startEnd != null)
            {
                hasStartEnd = true;
                this.startEnd = startEnd;
            }
        }



        // TimedFunctions creates copy of itself
        public TimedFunction CreateCopy ()
        {
            TimedFunction copy;
            lock (_lock)
            {
                copy = new TimedFunction(specifiedTimes, intervals, startEnd);
                copy.SetObjName(GetObjName());
            }
            return copy;
        }



        public static bool HaveEqualAttributes (TimedFunction tf1, TimedFunction tf2) 
        {
            if (    (tf1.GetType()              != tf2.GetType()) 
                ||  (tf1.GetObjName()           != tf2.GetObjName())
                ||  (tf1.GetFunc()              != tf2.GetFunc())
                ||  (tf1.GetParameters()        != tf2.GetParameters())
                ||  (tf1.GetMaxInvocations()    != tf2.GetMaxInvocations())
                ||  (tf1.GetInvocations()       != tf2.GetInvocations())
                ||  (tf1.GetStart()             != tf2.GetStart())
                ||  (tf1.GetEnd()               != tf2.GetEnd())
                ||  (tf1.GetSpecifiedTimes()    != tf2.GetSpecifiedTimes())
                ||  (tf1.GetIntervals()         != tf2.GetIntervals())
                ||  (tf1.GetLastIntervalIndex() != tf2.GetLastIntervalIndex())
                ||  (tf1.GetLastIntervalTime()  != tf2.GetLastIntervalTime()) )
            {
                return false;
            }
            return true;
        }

        public bool HasEqualAttributes (TimedFunction tf)
        {
            return HaveEqualAttributes(this, tf);
        }

    }

}
