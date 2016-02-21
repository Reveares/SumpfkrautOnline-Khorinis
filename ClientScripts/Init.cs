﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GUC.Log;
using GUC.Scripting;
using System.Reflection;
using System.IO;
using GUC.Client.Scripts.Sumpfkraut.Menus;
using GUC.Client.Scripts.Sumpfkraut.Menus.MainMenus;
using GUC.Client.Scripts.Sumpfkraut;
using GUC.Network;
using GUC.Enumeration;
using GUC.WorldObjects.Instances;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts
{
    public class Init : ScriptInterface
    {
        public static bool Ingame = false;

        public Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            Logger.Log("SumpfkrautOnline ClientScripts loaded!");
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Substring(0, args.Name.IndexOf(','));

            Assembly asm = Assembly.LoadFrom(Path.GetFullPath("System\\Multiplayer\\UntoldChapters\\SumpfkrautOnline\\" + name + ".dll"));
            if (asm == null)
            {
                asm = Assembly.LoadFrom(Path.GetFullPath("Multiplayer\\UntoldChapters\\SumpfkrautOnline\\" + name + ".dll"));
            }
            return asm;
        }

        public void Update(long ticks)
        {
            GUCMenu.UpdateMenus(ticks);
        }

        public void StartOutgame()
        {
            InputControl.Init();
            MainMenu.Menu.Open();
            Logger.Log("Outgame started.");
        }

        public void StartIngame()
        {
            InputControl.Init();
            MainMenu.CloseActiveMenus();
            Ingame = true;
            Logger.Log("Ingame started.");
        }

        public void OnReadMenuMsg(PacketReader steam) { }
        public void OnReadIngameMsg(PacketReader steam) { }

        public void OnCreateInstanceMsg(VobTypes type, PacketReader stream)
        {
            BaseVobDef def;
            switch (type)
            {
                case VobTypes.Vob:
                    def = new VobDef(stream);
                    break;
                case VobTypes.NPC:
                    def = new NPCDef(stream);
                    break;
                case VobTypes.Item:
                    def = new ItemDef(stream);
                    break;
                default:
                    Logger.LogError("Unknown VobDef-Type! " + type);
                    return;
            }

            def.Create();
        }

        public void OnDeleteInstanceMsg(BaseVobInstance instance)
        {

        }
    }
}