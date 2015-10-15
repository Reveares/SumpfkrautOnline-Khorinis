﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GUC.Enumeration;

namespace GUC.Server.WorldObjects
{
    public class NPCInstance : AbstractInstance
    {
        new protected static ushort idCount = 0;

        new protected static Dictionary<string, AbstractInstance> instanceDict = new Dictionary<string, AbstractInstance>();
        new protected static Dictionary<ushort, AbstractInstance> instanceList = new Dictionary<ushort, AbstractInstance>();

        #region Server fields
        public ushort AttrHealthMax = 100;
        public ushort AttrManaMax = 10;
        public ushort AttrStaminaMax = 100;
        public ushort AttrStrength = 10;
        public ushort AttrDexterity = 10;
        #endregion

        //Things which the client knows too
        #region Client fields
        /// <summary>The standard name of the NPC.</summary>
        public string name = "";
        /// <summary>The .MDS-Visual of the NPC.</summary>
        public string visual = "";
        /// <summary>The body mesh of the NPC.</summary>
        public string bodyMesh = "";
        /// <summary>The body texture of the NPC.</summary>
        public byte bodyTex = 0;
        /// <summary>The head mesh of the NPC.</summary>
        public string headMesh = "";
        /// <summary>The head texture of the NPC.</summary>
        public byte headTex = 0;

        /// <summary>The standard body height of the NPC in percent. Default: 100</summary>
        public byte bodyHeight = 100;
        /// <summary>The standard body width (x & z) of the NPC in percent. Default: 100</summary>
        public byte bodyWidth = 100;
        /// <summary>The standard fatness of the NPC in percent. Default: 0</summary>
        public short fatness = 0;

        /// <summary>The voice index of the NPC. Only used for humans. Default: None</summary>
        public HumVoice voice = HumVoice.None;
        #endregion

        protected override void Write(BinaryWriter bw)
        {
            bw.Write(ID);

            bw.Write(name);
            bw.Write(visual);
            bw.Write(bodyMesh);
            bw.Write(bodyTex);
            bw.Write(headMesh);
            bw.Write(headTex);
            bw.Write(bodyHeight);
            bw.Write(bodyWidth);
            bw.Write(fatness);
            bw.Write((byte)voice);
        }

        #region Constructors
        public NPCInstance(string instanceName) : base(instanceName)
        {
        }

        public NPCInstance(ushort ID, string instanceName) : base(ID, instanceName)
        {
        }
        #endregion

        //meh
        public static NPCInstance Get(string instanceName)
        {
            return (NPCInstance)UncastedGet(instanceName);
        }

        public static NPCInstance Get(ushort id)
        {
            return (NPCInstance)UncastedGet(id);
        }
    }
}