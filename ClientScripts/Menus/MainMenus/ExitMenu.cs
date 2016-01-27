﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Client.Scripts.Menus.MainMenus
{
    class ExitMenu : GUCMainMenu
    {
        public readonly static ExitMenu Menu = new ExitMenu();

        protected override void OnCreate()
        {
            preferredCursorItem = 1;
            Back.CreateTextCenterX("SumpfkrautOnline verlassen?", 100);
            AddButton("Ja", "Ja, ich möchte SumpfkrautOnline verlassen.", 200, CloseGothic);
            AddButton("Nein", "Nein, ich möchte weiterspielen.", 250, MainMenu.Menu.Open);
            OnEscape = MainMenu.Menu.Open;
        }

        void CloseGothic()
        {
            //Gothic.zClasses.CGameManager.ExitGameFunc(Program.Process);
        }
    }
}
