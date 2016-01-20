﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;

namespace GUC.Server.WorldObjects.Instances
{
    public class MobSwitchInstance : MobInterInstance
    {
        new public readonly static Enumeration.VobTypes sVobType = Enumeration.VobTypes.MobSwitch;
        new public readonly static Collections.InstanceDictionary Instances = Network.Server.Instances.GetDict(sVobType);

        public MobSwitchInstance(string instanceName, object scriptObject)
            : this(0, instanceName, scriptObject)
        {
        }

        public MobSwitchInstance(ushort ID, string instanceName, object scriptObject)
            : base(ID, instanceName, scriptObject)
        {
            this.VobType = sVobType;
        }

        public new static Action<MobSwitchInstance, PacketWriter> OnWriteProperties;
        internal override void WriteProperties(PacketWriter stream)
        {
            base.WriteProperties(stream);

            if (MobSwitchInstance.OnWriteProperties != null)
            {
                MobSwitchInstance.OnWriteProperties(this, stream);
            }
        }
    }
}
