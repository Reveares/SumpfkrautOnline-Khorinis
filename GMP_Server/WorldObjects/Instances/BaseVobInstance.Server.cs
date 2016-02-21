﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Server.Network;
using GUC.Enumeration;
using RakNet;

namespace GUC.WorldObjects.Instances
{
    public abstract partial class BaseVobInstance
    {
        #region Networking

        internal void WriteCreate()
        {
            var stream = GameServer.SetupStream(NetworkIDs.InstanceCreateMessage);

            stream.Write((byte)this.VobType);
            this.WriteStream(stream);

            foreach (Client client in GameServer.GetValidClients())
                client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE, '\0');
        }

        internal void WriteDelete()
        {
            var stream = GameServer.SetupStream(NetworkIDs.InstanceDeleteMessage);

            stream.Write((ushort)this.ID);

            foreach (Client client in GameServer.GetValidClients())
                client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE, '\0');
        }

        #endregion
    }
}