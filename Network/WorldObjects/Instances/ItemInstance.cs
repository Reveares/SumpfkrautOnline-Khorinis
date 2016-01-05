﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Enumeration;
using GUC.Network;

namespace GUC.WorldObjects.Instances
{
    public partial class ItemInstance : VobInstance
    {
        public static readonly Collections.InstanceDictionary ItemInstances = VobInstance.AllInstances.GetDict(Enumeration.VobTypes.Item);

        #region Fists
        public static readonly ItemInstance FistInstance = CreateFists();
        static ItemInstance CreateFists()
        {
            ItemInstance fists = new ItemInstance();
            fists.Name = "Fäustedummy";
            return fists;
        }
        #endregion

        #region Properties

        /// <summary>The standard name of this item.</summary>
        public String Name = "";

        /// <summary>The type of this item.</summary>
        public ItemTypes Type = ItemTypes.Misc;

        /// <summary>The material of this item. Controls the dropping sound.</summary>
        public ItemMaterials Material = ItemMaterials.Wood;

        string visualChange = "";
        /// <summary>The ASC-Mesh for armors which is put over the NPC's character model.</summary>
        public String VisualChange
        {
            get { return visualChange; }
            set { visualChange = value.Trim().ToUpper(); }
        }

        string effect = "";
        /// <summary>Magic Effect. See Scripts/System/VisualFX/VisualFxInst.d</summary>
        public String Effect
        {
            get { return effect; }
            set { effect = value.Trim().ToUpper(); }
        }   

        #endregion

        internal ItemInstance()
        {
            this.VobType = VobTypes.Item;
        }

        internal override void ReadProperties(PacketReader stream)
        {
            base.ReadProperties(stream);

            this.Name = stream.ReadString();
            this.Type = (ItemTypes)stream.ReadByte();
            this.Material = (ItemMaterials)stream.ReadByte();
            this.VisualChange = stream.ReadString();
            this.Effect = stream.ReadString();
        }

        internal override void WriteProperties(PacketWriter stream)
        {
            base.WriteProperties(stream);

            stream.Write(Name);
            stream.Write((byte)Type);
            stream.Write((byte)Material);
            stream.Write(VisualChange);
            stream.Write(Effect);
        }
    }
}