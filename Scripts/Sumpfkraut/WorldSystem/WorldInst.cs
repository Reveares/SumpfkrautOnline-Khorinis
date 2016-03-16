﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.WorldSystem
{
    public partial class WorldInst : ScriptObject, WorldObjects.World.IScriptWorld
    {
        WorldObjects.World baseWorld;
        public WorldObjects.World BaseWorld { get { return baseWorld; } }

        WorldDef definition;
        public WorldDef Definition { get { return definition; } }

        public WorldInst(PacketReader stream) : this(new WorldObjects.World())
        {
            baseWorld.ReadStream(stream);
        }

        private WorldInst(WorldObjects.World baseWorld)
        {
            if (baseWorld == null)
                throw new ArgumentNullException("BaseWorld is null!");

            this.baseWorld = baseWorld;
        }

        public void OnWriteProperties(PacketWriter stream)
        {
            // write definition id
        }

        public void OnReadProperties(PacketReader stream)
        {
            // read definition id
        }

        public void Create()
        {
            this.baseWorld.Create();
        }

        public void Delete()
        {
            this.baseWorld.Delete();
        }
    }
}
