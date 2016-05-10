﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Types;
using Gothic.Objects;
using GUC.Enumeration;
using GUC.Animations;
using Gothic.Types;
using WinApi;

namespace GUC.WorldObjects
{
    public partial class NPC
    {
        #region ScriptObject

        public partial interface IScriptNPC : IScriptVob
        {
            void StartAnimation(Animation ani, object[] netArgs);
        }

        #endregion

        public partial class ClimbingLedge
        {
            internal ClimbingLedge(zTLedgeInfo li)
            {
                this.loc = (Vec3f)li.Position;
                this.norm = (Vec3f)li.Normal;
                this.cont = (Vec3f)li.Cont;
                this.maxMoveFwd = li.MaxMoveForward;
            }

            internal void SetLedgeInfo(zTLedgeInfo li)
            {
                li.Position.X = this.loc.X;
                li.Position.Y = this.loc.Y;
                li.Position.Z = this.loc.Z;

                li.Normal.X = this.norm.X;
                li.Normal.Y = this.norm.Y;
                li.Normal.Z = this.norm.Z;

                li.Cont.X = this.cont.X;
                li.Cont.Y = this.cont.Y;
                li.Cont.Z = this.cont.Z;

                li.MaxMoveForward = this.maxMoveFwd;
            }
        }

        public ClimbingLedge DetectClimbingLedge()
        {
            ClimbingLedge result;

            var ai = this.gVob.HumanAI;

            using (var dummy = zVec3.Create())
                ai.DetectClimbUpLedge(dummy, true);

            var li = ai.GetLedgeInfo();
            if (li.Address != 0)
            {
                result = new ClimbingLedge(li);
            }
            else
            {
                result = null;
            }
            ai.ClearFoundLedge();

            return result;
        }

        public void SetGClimbingLedge(ClimbingLedge ledge)
        {
            int ai = Process.Alloc(4).ToInt32();
            Process.Write(this.gVob.HumanAI.Address, ai);

            Process.THISCALL<NullReturnCall>(0x8D44E0, 0x512310, (IntArg)ai);
            if ( Process.THISCALL<BoolArg>(0x8D44E0, 0x5123E0, (IntArg)ai))
            {
                var li = Process.THISCALL<zTLedgeInfo>(0x8D44E0, 0x512310, (IntArg)ai);
                ledge.SetLedgeInfo(li);
            }
            else
            {
                throw new Exception("SetGClimbingLedge: GetDataDangerous failed!");
            }
        }


        public partial interface IScriptNPC : IScriptVob
        {
            void OnTick(long now);
        }

        public const long PositionUpdateTime = 1200000; //120ms
        public const long DirectionUpdateTime = PositionUpdateTime + 100000;

        new public oCNpc gVob { get { return (oCNpc)base.gVob; } }

        internal long nextStateUpdate = 0;

        EnvironmentState GetEnvState()
        {
            var ai = this.gVob.HumanAI;
            var collObj = this.gvob.CollObj;

            float waterY = collObj.WaterLevel;
            if (waterY > ai.HeadY)
            {
                return EnvironmentState.Diving;
            }
            else if (waterY > ai.FeetY)
            {
                float waterDepth = waterY - collObj.GroundLevel;
                if (waterDepth > 0)
                {
                    if (waterDepth > ai.WaterDepthSwim)
                        return EnvironmentState.Swimming;
                    else if (waterDepth > ai.WaterDepthWade)
                        return EnvironmentState.Wading;
                    return EnvironmentState.InWater;
                }
            }
            else if ((this.gvob.BitField1 & zCVob.BitFlag0.physicsEnabled) != 0 || ai.AboveFloor > ai.StepHeight)
            {
                return EnvironmentState.InAir;
            }

            return EnvironmentState.None;
        }

        #region Animations

        #region Overlays

        partial void pAddOverlay(Overlay overlay)
        {
            if (this.gvob != null)
                this.gVob.ApplyOverlay(overlay.Name);
        }

        partial void pRemoveOverlay(Overlay overlay)
        {
            if (this.gvob != null)
                this.gVob.ApplyOverlay(overlay.Name);
        }

        #endregion

        partial void pStartAnimation(Animation ani)
        {
            if (this.gvob != null)
            {
                var gModel = this.gVob.GetModel();
                int aniID = gModel.GetAniIDFromAniName(ani.AniJob.Name);
                if (aniID > 0)
                {
                    gModel.StartAni(aniID, 0);
                    gModel.GetActiveAni(aniID).SetActFrame(ani.StartFrame);
                }
            }
        }

        partial void pStopAnimation(Animation ani, bool fadeOut)
        {
            if (this.gvob != null)
            {
                var gModel = gVob.GetModel();
                int id = gModel.GetAniIDFromAniName(ani.AniJob.Name);
                var activeAni = gModel.GetActiveAni(id);

                if (fadeOut)
                {
                    gModel.StopAni(activeAni);
                }
                else
                {
                    gModel.FadeOutAni(activeAni);
                }
            }
        }

        partial void pEndAni(Animation ani)
        {
            var gModel = gVob.GetModel();
            int id = gModel.GetAniIDFromAniName(ani.AniJob.Name);
            var activeAni = gModel.GetActiveAni(id);
            if (activeAni.Address == 0)
                return;

            if (activeAni.ModelAni.Layer == 1)
            {
                if ((this.gvob.BitField1 & zCVob.BitFlag0.physicsEnabled) == 0) // gothic prob has already handled this anyway
                {
                    if (this.movement == MoveState.Forward)
                        gModel.StartAni(this.gVob.AniCtrl._s_walkl, 0);
                    else
                        gModel.StartAni(this.gVob.AniCtrl._s_walk, 0);
                }
            }
            else
            {
                gModel.StopAni(activeAni);
            }
        }

        #endregion

        #region Spawn

        public override void Spawn(World world, Vec3f position, Vec3f direction)
        {
            base.Spawn(world, position, direction);

            gVob.HP = this.hp;
            gVob.HPMax = this.hpmax;
            gVob.InitHumanAI();
            gVob.Enable(pos.X, pos.Y, pos.Z);
            if (this.Name == "Scavenger") gVob.SetToFistMode();
            if (overlays != null)
                for (int i = 0; i < overlays.Count; i++)
                    this.gVob.ApplyOverlay(overlays[i].Name);
            gVob.Name.Set(this.Name);
        }

        partial void pDespawn()
        {
            gVob.Disable();
        }

        #endregion

        partial void pSetHealth()
        {
            if (this.gVob == null)
                return;

            var gModel = gVob.GetModel();
            var gAniCtrl = gVob.AniCtrl;

            gModel.StopAni(gAniCtrl._t_strafel);
            gModel.StopAni(gAniCtrl._t_strafer);

            this.gVob.HPMax = this.hpmax;
            this.gVob.HP = this.hp;
        }

        partial void pSetMovement(MoveState state)
        {
            if (this.gVob == null)
                return;

            if (!this.IsInAnimation())
                if (this.movement == MoveState.Right || this.movement == MoveState.Left)
                {
                    if (state == MoveState.Forward)
                        this.gVob.GetModel().StartAni(this.gVob.AniCtrl._s_walkl, 0);
                    else
                        this.gVob.GetModel().StartAni(this.gVob.AniCtrl._s_walk, 0);
                }

            this.Update(GameTime.Ticks);
        }


        internal void Update(long now)
        {
            /*if (turning) //turn!
            {
                float diff = (float)(DateTime.UtcNow.Ticks - lastDirTime) / (float)DirectionUpdateTime;

                if (diff < 1.0f)
                {
                    Direction = lastDir + (nextDir - lastDir) * diff;
                }
                else
                {
                    StopTurnAnis();
                }
            }*/
            this.envState = GetEnvState();

            this.ScriptObject.OnTick(now);

            if (this.IsDead || this.IsInAnimation())
                return;

            switch (Movement)
            {
                case MoveState.Forward:
                    var gModel = this.gVob.GetModel();
                    if (gModel.IsAnimationActive("T_JUMP_2_STAND") != 0)
                    {
                        gModel.StartAni(this.gVob.AniCtrl._s_walkl, 0);
                    }
                    gVob.AniCtrl._Forward();
                    break;
                case MoveState.Backward:
                    gVob.AniCtrl._Backward();
                    break;
                case MoveState.Right:
                    if (this.envState <= EnvironmentState.Wading && !gVob.GetModel().IsAniActive(gVob.GetModel().GetAniFromAniID(gVob.AniCtrl._t_strafer)))
                    {
                        gVob.GetModel().StartAni(gVob.AniCtrl._t_strafer, 0);
                    }
                    break;
                case MoveState.Left:
                    if (this.envState <= EnvironmentState.Wading && !gVob.GetModel().IsAniActive(gVob.GetModel().GetAniFromAniID(gVob.AniCtrl._t_strafel)))
                    {
                        gVob.GetModel().StartAni(gVob.AniCtrl._t_strafel, 0);
                    }
                    break;
                case MoveState.Stand:
                    gVob.AniCtrl._Stand();
                    break;
                default:
                    break;
            }
        }

    }
}
