﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Enumeration
{
    /** 
    * Types of network messages (data) which can be exchanged between clients and server.
    */
    internal enum NetworkIDs : byte
    {
        // general
        ConnectionMessage,
        
        // instances
        InstanceCreateMessage,
        InstanceDeleteMessage,

        // models
        ModelCreateMessage,
        ModelDeleteMessage,
        
        SpectatorMessage,
        PlayerControlMessage, // npc control
        LoadWorldMessage, // for changing the world

        // Messages for Scripts
        ScriptMessage,
        ScriptVobMessage,

        SpecPosMessage,
        SpecDirMessage,
        SpecPosDirMessage,

        // world & spawns
        WorldCellMessage, // when switching world cells
        WorldJoinMessage, // only vobs to spawn
        WorldLeaveMessage, // remove all spawned vobs
        WorldSpawnMessage, // for spawning a vob in the world
        WorldDespawnMessage, // for despawning a vob in the world
        WorldTimeMessage, // to set the world's time
        WorldTimeStartMessage, // to start/stop the world's time
        WorldWeatherMessage, // to set the world's weather
        WorldWeatherTypeMessage, // rain & snow

        //vobs
        VobPosMessage,
        VobDirMessage,
        VobPosDirMessage, // updating position and direction of a vob

        // npcs 
        NPCStateMessage,
        NPCEquipMessage,
        NPCEquipSwitchMessage,
        NPCUnequipMessage,
        NPCApplyOverlayMessage,
        NPCRemoveOverlayMessage,
        NPCAniStartMessage,
        NPCAniStartWithArgsMessage,
        NPCAniStopMessage,
        NPCHealthMessage,
        NPCSetFightModeMessage,
        NPCUnsetFightModeMessage,

        //inventory
        InventoryAddMessage, // add item to player inventory
        InventoryAmountMessage,
        InventoryRemoveMessage, // remove item from player inventory
        InventoryEquipMessage,
        InventoryUnequipMessage,

        // GuidedVobs
        GuideAddMessage,

    }
}
