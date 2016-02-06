﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;
using GUC.Enumeration;

namespace GUC.WorldObjects.Instances
{
    public abstract partial class BaseInstance : GameObject
    {
        #region ScriptObject

        public partial interface IScriptBaseInstance : IScriptGameObject
        {
        }

        public new IScriptBaseInstance ScriptObject
        {
            get { return (IScriptBaseInstance)base.ScriptObject; }
        }

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Instance with the given ID or [-1] a free ID.
        /// </summary>
        protected BaseInstance(IScriptBaseInstance scriptObject, int id = -1) : base(scriptObject, id)
        {
        }

        /// <summary>
        /// Creates a new Instance by reading a networking stream.
        /// </summary>
        protected BaseInstance(IScriptBaseInstance scriptObject, PacketReader stream) : base(scriptObject, stream)
        {
        }
        #endregion
    }
}
