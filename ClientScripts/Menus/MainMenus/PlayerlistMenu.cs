﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Client.Scripts.Menus.MainMenus
{
    class PlayerlistMenu : GUCMainMenu
    {
        public readonly static PlayerlistMenu Menu = new PlayerlistMenu();

        protected override void OnCreate()
        {
            Back.CreateTextCenterX("Spielerliste", 100);
            //list of player names
            AddButton("Zurück", "Zurück zum Hauptmenü.", 350, MainMenu.Menu.Open);
            OnEscape = MainMenu.Menu.Open;
        }
    }
}
