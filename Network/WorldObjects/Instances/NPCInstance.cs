﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;
using GUC.Enumeration;
using GUC.WorldObjects.Collections;

namespace GUC.WorldObjects.Instances
{
    public partial class NPCInstance : VobInstance
    {
        public partial interface IScriptNPCInstance : IScriptVobInstance
        {
        }

        new public const VobTypes sVobType = VobTypes.NPC;
        public override VobTypes VobType { get { return sVobType; } }
        public static readonly InstanceDictionary NPCInstances = VobInstance.AllInstances.GetDict(sVobType);

        public NPCInstance(PacketReader stream, IScriptNPCInstance scriptObj) : base(stream, scriptObj)
        {
        }

        #region Properties

        /// <summary>The default name of the NPC.</summary>
        public string Name = "";

        protected string bodyMesh = "";
        /// <summary>The body mesh of the NPC.</summary>
        public string BodyMesh
        {
            get { return bodyMesh; }
            set { bodyMesh = value.Trim().ToUpper(); }
        }

        /// <summary>The body texture of the NPC.</summary>
        public byte BodyTex = 0;

        protected string headMesh = "";
        /// <summary>The head mesh of the NPC.</summary>
        public string HeadMesh
        {
            get { return headMesh; }
            set { headMesh = value.Trim().ToUpper(); }
        }

        /// <summary>The default head texture of the NPC.</summary>
        public byte HeadTex = 0;
        #endregion

        new public IScriptNPCInstance ScriptObj { get; protected set; }

        internal override void WriteProperties(PacketWriter stream)
        {
            base.WriteProperties(stream);

            stream.Write(Name);
            stream.Write(BodyMesh);
            stream.Write(BodyTex);
            stream.Write(HeadMesh);
            stream.Write(HeadTex);
        }

        internal override void ReadProperties(PacketReader stream)
        {
            base.ReadProperties(stream);

            this.Name = stream.ReadString();
            this.BodyMesh = stream.ReadString();
            this.BodyTex = stream.ReadByte();
            this.HeadMesh = stream.ReadString();
            this.HeadTex = stream.ReadByte();
        }
    }
}