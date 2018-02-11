﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Scripts.Sumpfkraut.AI.SimpleAI.AIRoutines;

namespace GUC.Scripts.Sumpfkraut.AI.SimpleAI.AIPersonalities
{

    public class GroupAIPersonality : BaseAIPersonality
    {

        protected List<BaseAIPersonality> groupMembers;
        protected object memberLock;



        public void Init (AIMemory aIMemory, BaseAIRoutine aiRoutine, List<BaseAIPersonality> groupMembers)
        {
            Init(aiMemory, aiRoutine);
            this.groupMembers = groupMembers ?? new List<BaseAIPersonality>();
            this.memberLock = new object();
        }

        public override void Init (AIMemory aiMemory, BaseAIRoutine aiRoutine)
        {
            this.aiMemory = aiMemory ?? new AIMemory();
            this.aiRoutine = aiRoutine ?? new SimpleAIRoutine();
            this.lastTick = DateTime.Now;
        }



        /// <summary>
        /// Add more members to be influenced by the group.
        /// </summary>
        /// <param name="groupMember"></param>
        /// <returns></returns>
        public int AddGroupMember (BaseAIPersonality groupMember)
        {
            lock (memberLock) { groupMembers.Add(groupMember); }
            return groupMembers.Count;
        }



        public override void MakeActiveObservation(AIAgent aiAgent)
        {
            
        }

        public override void ProcessActions(AIAgent aiAgent)
        {
            throw new NotImplementedException();
        }

        public override void ProcessObservations(AIAgent aiAgent)
        {
            throw new NotImplementedException();
        }

    }

}
