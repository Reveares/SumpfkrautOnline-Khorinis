﻿using GUC.Scripts.Sumpfkraut.Utilities.Functions.Enumeration;
using GUC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUC.Scripts.Sumpfkraut.Utilities.Functions
{

    public class TimedFunction : ExtendedObject
    {

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

        // option whether to guarentee due calls of the function although it might have reached it's end DateTime
        protected bool preserveDueInvocations;
        public bool GetPreserveDueInvocations () { lock (_lock) { return preserveDueInvocations; } }
        public void SetPreserveDueInvocations (bool value) { lock (_lock) { preserveDueInvocations = value; } }

        // how many times the TimedFunction was invoked already
        protected long invocations;
        public long GetInvocations () { lock (_lock) { return invocations; } }
        public void SetInvocations (long value) { lock (_lock) { invocations = value; } }
        public void IterateInvocations (int it) { lock (_lock) { invocations += it; } }
        public bool HasInvocationsLeft
        {
            get
            {
                lock (_lock)
                {
                    if (!HasMaxInvocations) { return true; }
                    return (maxInvocations - invocations) > 0;
                }
            }
        }
        public bool TryGetInvocationsLeft (out long left)
        {
            lock (_lock)
            {
                if (!HasMaxInvocations)
                {
                    left = long.MaxValue;
                    return true;
                }
                left = maxInvocations - invocations;
                left = left < 0 ? 0 : left;
                if (left > 0) { return true; }
                else { return false; }
            }
        }

        // max limit to invocations
        protected bool hasMaxInvocations;
        public bool HasMaxInvocations { get { return hasMaxInvocations; } }
        protected long maxInvocations;
        public long GetMaxInvocations () { return maxInvocations; }

        // crisp DateTimes at which to call the function
        protected bool hasSpecifiedTimes;
        public bool HasSpecifiedTimes { get { return hasSpecifiedTimes; } }
        protected DateTime[] specifiedTimes;
        public DateTime[] GetSpecifiedTimes () { return specifiedTimes; }
        public bool TryGetLastSpecifiedTime (out DateTime last)
        {
            lock (_lock)
            {
                if ((!HasSpecifiedTimes) || (!HasSpecifiedTimesLeft))
                {
                    last = DateTime.MinValue;
                    return false;
                }

                last = specifiedTimes[lastSpecifiedTimeIndex];
                return true;
            }
        }
        public bool TryGetNextSpecifiedTime (out DateTime next)
        {
            lock (_lock)
            {
                if ((!HasSpecifiedTimes) || (!HasSpecifiedTimesLeft))
                {
                    next = DateTime.MaxValue;
                    return false;
                }

                // push the index only further to return it to it's previous state after receiving the residing value
                IterateSpecifiedTimeIndex(1);
                next = specifiedTimes[lastSpecifiedTimeIndex];
                IterateSpecifiedTimeIndex(-1);
                return true;
            }
        }

        // to remember which specified times have already been processed
        protected int lastSpecifiedTimeIndex;
        public int GetLastSpecifiedTimeIndex () { lock (_lock) { return lastSpecifiedTimeIndex; } }
        public int SpecifiedTimesLeft ()
        {
            var amount = 0;
            lock (_lock)
            {
                amount = (specifiedTimes.Length - 1) - lastSpecifiedTimeIndex;
                amount = amount < 0 ? 0 : amount;
            }

            return amount;
        }
        public bool HasSpecifiedTimesLeft { get { lock (_lock) { return SpecifiedTimesLeft() > 0; } } }
        public int IterateSpecifiedTimeIndex (int it)
        {
            int index = -1;
            lock (_lock)
            {
                index = lastSpecifiedTimeIndex + it;
                if (!(index < specifiedTimes.Length)) { index = specifiedTimes.Length - 1; }
                lastSpecifiedTimeIndex = index;
            }

            return index;
        }

        // repeating intervals (can be broke out of by using maxInvocations, startEnd or self-delting TimedFunction-bodies)
        protected bool hasIntervals;
        public bool HasIntervals { get { return hasIntervals; } }
        protected TimeSpan[] intervals;
        public TimeSpan[] GetIntervals () { return intervals; }
        public bool TryGetLastInterval (out TimeSpan last)
        {
            lock (_lock)
            {
                if (!HasIntervals)
                {
                    last = TimeSpan.MinValue;
                    return false;
                }

                last =  intervals[lastIntervalIndex];
                return true;
            }
        }
        public bool TryGetNextInterval(out TimeSpan next)
        {
            lock (_lock)
            {
                if (!HasIntervals)
                {
                    next = TimeSpan.MinValue;
                    return false;
                }

                // push the index only further to return it to it's previous state after receiving the residing value
                IterateIntervalIndex(1);
                next =  intervals[lastIntervalIndex];
                IterateIntervalIndex(-1);
                return true;
            }
        }

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
        public int IterateIntervalIndex (int it)
        {
            int index = -1;
            lock (_lock)
            {
                index = lastIntervalIndex + it;
                if (!(index < intervals.Length)) { index = 0; }
                lastIntervalIndex = index;
            }

            return index;
        }

        // crisp DateTimes at which to start and end the use of that TimedFunction
        protected bool hasStartEnd;
        public bool HasStartEnd { get { return hasStartEnd; } }
        protected Tuple<DateTime, DateTime> startEnd;
        public Tuple<DateTime, DateTime> GetStartEnd () { return startEnd; }
        public DateTime GetStart () { return startEnd.Item1; }
        public DateTime GetEnd () { return startEnd.Item2; }
        public bool HasStarted () { return HasStarted(DateTime.Now); }
        public bool HasStarted (DateTime referenceTime)
        {
            return ((!HasStartEnd) || (referenceTime <= GetStart()));
        }
        public bool HasExpired () { return HasExpired(DateTime.Now); }
        public bool HasExpired (DateTime referenceTime)
        {
            if (!hasStartEnd) { return false; }
            return referenceTime > GetEnd();
        }

        protected InvocationType lastInvocationType;
        public InvocationType GetLastInvocationType () { lock (_lock) { return lastInvocationType; } }
        public void SetLastInvocationType (InvocationType value) { lock (_lock) { lastInvocationType = value; } }

        protected InvocationType nextInvocationType;
        public InvocationType GetNextInvocationType () { lock (_lock) { return nextInvocationType; } }
        public void SetNextInvocationType (InvocationType value) { lock (_lock) { nextInvocationType = value; } }



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

            preserveDueInvocations = true;

            if (specifiedTimes != null)
            {
                hasSpecifiedTimes = true;
                this.specifiedTimes = specifiedTimes;
            }
            if (intervals != null)
            {
                hasIntervals = true;
                this.intervals = intervals;
                lastIntervalIndex = 0;
                lastIntervalTime = DateTime.Now;
            }
            if (startEnd != null)
            {
                hasStartEnd = true;
                this.startEnd = startEnd;
            }
        }



        public bool TryIterateNextInvocation (DateTime referenceTime)
        {
            var success = false;
            var nextInvocationType = InvocationType.Undefined;
            // set to max. DateTime first to ease comparisons later
            var nextTime = DateTime.MaxValue;

            lock (_lock)
            {
                // detect invocation limit
                if (!HasInvocationsLeft) { return false; }

                // time limits
                var hasExpired = HasExpired(referenceTime);
                var preserveExpired = GetPreserveDueInvocations();
                // if expiration date reached and no intent to preserve possible left out invocations
                if (hasExpired && (!preserveExpired)) { return false; }

                // determine possible next specified time
                success = TryGetNextSpecifiedTime(out nextTime);
                if (success) { nextInvocationType = InvocationType.SpecifiedTime; }

                // detect interval and compare with possible previous specified time
                TimeSpan lastInterval;
                if (TryGetLastInterval(out lastInterval))
                {
                    var lastIntervalTime = GetLastIntervalTime();
                    var nextIntervalTime = lastIntervalTime + lastInterval;
                    Print(lastIntervalTime);
                    // only take intervals into account which would have been invocated in the meantime
                    if (nextIntervalTime <= GetEnd())
                    {
                        if (nextIntervalTime < nextTime)
                        {
                            nextTime = nextIntervalTime;
                            nextInvocationType = InvocationType.SpecifiedTime;
                            success = true;
                        }
                    }
                }

                if (nextInvocationType != InvocationType.Undefined)
                {
                    switch (nextInvocationType)
                    {
                        case InvocationType.SpecifiedTime:
                            IterateSpecifiedTimeIndex(1);
                            break;
                        case InvocationType.Interval:
                            IterateIntervalIndex(1);
                            break;
                        default:
                            break;
                    }
                    SetLastInvocationType(GetNextInvocationType());
                    SetNextInvocationType(nextInvocationType);
                }
            }

            return success;
        }



        //// find next possible invocation time and type and iterate necessary entries 
        //// while NOT invoking the function itself
        //public bool TryPrepareNextInvocation (DateTime referenceTime, out DateTime nextTime)
        //{
        //    var success = false;
        //    var nextInvocationType = InvocationType.Undefined;
        //    // set to max. DateTime first to ease comparisons later
        //    nextTime = DateTime.MaxValue;

        //    lock (_lock)
        //    {
        //        // detect invocation limit
        //        if (!HasInvocationsLeft) { return success; }

        //        // time limits
        //        var hasExpired = HasExpired(referenceTime);
        //        var preserveExpired = GetPreserveDueInvocations();
        //        // if expiration date reached and no intent to preserve possible left out invocations
        //        if (hasExpired && (!preserveExpired)) { return success; }

        //        // determine possible next specified time
        //        success = TryGetNextSpecifiedTime(out nextTime);
        //        if (success) { nextInvocationType = InvocationType.SpecifiedTime; }

        //        // detect interval and compare with possible previous specified time
        //        TimeSpan lastInterval;
        //        if (TryGetLastInterval(out lastInterval))
        //        {
        //            var lastIntervalTime = GetLastIntervalTime();
        //            var nextIntervalTime = lastIntervalTime + lastInterval;
        //            Print(lastIntervalTime);
        //            // only take intervals into account which would have been invocated in the meantime
        //            if (nextIntervalTime <= GetEnd())
        //            {
        //                if (nextIntervalTime < nextTime)
        //                {
        //                    nextTime = nextIntervalTime;
        //                    nextInvocationType = InvocationType.SpecifiedTime;
        //                    success = true;
        //                }
        //            }
        //        }

        //        if (nextInvocationType != InvocationType.Undefined)
        //        {
        //            switch (nextInvocationType)
        //            {
        //                case InvocationType.SpecifiedTime:
        //                    IterateSpecifiedTimeIndex(1);
        //                    break;
        //                case InvocationType.Interval:
        //                    IterateIntervalIndex(1);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            SetLastInvocationType(GetNextInvocationType());
        //            SetNextInvocationType(nextInvocationType);
        //        }
        //    }

        //    return success;
        //}

    }

}
