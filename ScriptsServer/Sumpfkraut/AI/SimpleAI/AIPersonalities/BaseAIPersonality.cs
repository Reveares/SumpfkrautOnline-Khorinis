﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Utilities;
using GUC.Scripts.Sumpfkraut.AI.SimpleAI.AIRoutines;

namespace GUC.Scripts.Sumpfkraut.AI.SimpleAI.AIPersonalities
{

    public abstract class BaseAIPersonality : ExtendedObject
    {

        new public static readonly string _staticName = "BaseAIPersonality (static) ";



        protected object bla;
        public object Bla { get { return bla; } }

        protected AIMemory aiMemory;
        public AIMemory AIMemory { get { return aiMemory; } }

        protected BaseAIRoutine aiRoutine;
        public BaseAIRoutine AIRoutine { get { return aiRoutine; } }



        public BaseAIPersonality ()
        {
            SetObjName("BaseAIPersonality (default)");
        }



        abstract public void Init (AIMemory aiMemory, BaseAIRoutine aiRoutine);
        abstract public void ProcessActions (AIAgent aiAgent);
        abstract public void ProcessObservations (AIAgent aiAgent);
        
    }

}
