﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Utilities
{
    public class LockTimer
    {
        long duration;
        /// <summary> Lock duration in ticks (0.0001 ms). </summary>
        public long Duration { get { return this.duration; } }

        long nextTime = 0;
        /// <summary> The next time this LockTimer will be ready. </summary>
        public long NextReadyTime { get { return this.nextTime; } }
        
        /// <param name="duration"> Lock duration in milliseconds. </param>
        public LockTimer(int duration)
        {
            SetDuration(duration);
        }
        
        /// <param name="newDuration"> New lock duration in milliseconds. </param>
        public void SetDuration(int newDuration)
        {
            this.duration = newDuration * TimeSpan.TicksPerMillisecond;
        }

        /// <summary> Checks if the lock duration is over and restarts the timer if true. </summary>
        public bool IsReady
        {
            get
            {
                bool rdy = this.nextTime <= GameTime.Ticks;
                if (rdy)
                {
                    this.nextTime = GameTime.Ticks + duration;
                }
                return rdy;
            }
        }

        /// <summary> Sets the next ready time from now. </summary>
        public void Trigger()
        {
            this.nextTime = GameTime.Ticks + duration;
        }
    }
}
