﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class ItemInst
    {
        partial void pSpawn()
        {
            this.BaseInst.gVob.Name.Set(this.Definition.Name);
        }
    }
}
