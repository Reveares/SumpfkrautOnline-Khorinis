﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Client.GUI;

namespace GUC.Client.Scripts.GUI.MainMenu
{
    abstract class MainMenuItem : GUCView
    {
        public string HelpText;
        public Action OnActivate;

        public abstract void Select();
        public abstract void Deselect();

        protected bool enabled = true;
        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }
    }
}
