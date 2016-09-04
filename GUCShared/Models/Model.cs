﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;
using GUC.Scripting;
using GUC.Animations;

namespace GUC.Models
{
    /// <summary>
    /// A GUC Model consists of a Gothic-Visual-String, a collection of Animation-Overlays and a collection of Animation-Jobs.
    /// </summary>
    public partial class Model : GameObject
    {
        #region ScriptObject

        public interface IScriptModel : IScriptGameObject
        {
            void Create();
            void Delete();

            void AddOverlay(Overlay overlay);
            void RemoveOverlay(Overlay overlay);

            void AddAniJob(AniJob aniJob);
            void RemoveAniJob(AniJob aniJob);
        }

        new public IScriptModel ScriptObject
        {
            get { return (IScriptModel)base.ScriptObject; }
            set { base.ScriptObject = value; }
        }

        #endregion

        #region Collection

        static StaticCollection<Model> idColl = new StaticCollection<Model>();
        static DynamicCollection<Model> models = new DynamicCollection<Model>();
        static DynamicCollection<Model> dynModels = new DynamicCollection<Model>();

        #region Create & Delete

        /// <summary> Checks whether this object is added to the static Model collection. </summary>
        public bool IsCreated { get { return this.isCreated; } }

        partial void pCreate();
        /// <summary> Adds this object to the static Model collection. </summary>
        public void Create()
        {
            if (this.isCreated)
                throw new ArgumentException("Model is already in the collection!");

            idColl.Add(this);

            models.Add(this, ref this.collID);

            if (!this.IsStatic)
            {
                dynModels.Add(this, ref this.dynID);
            }

            this.isCreated = true;

            pCreate();
        }

        partial void pDelete();
        /// <summary> Removes this object from the static Model collection. </summary>
        public void Delete()
        {
            if (!this.isCreated)
                throw new ArgumentException("Model is not in the collection!");
            
            pDelete();

            this.isCreated = false;

            idColl.Remove(this);
            models.Remove(ref this.collID);

            if (!this.IsStatic)
            {
                dynModels.Remove(ref this.dynID);
            }
        }

        #endregion

        #region Access

        /// <summary> Checks whether the static Model collection contains an object with the specified ID. </summary>
        public static bool Contains(int id)
        {
            return idColl.ContainsID(id);
        }

        /// <summary> Gets a Model by ID from the static Model collection. </summary>
        public static bool TryGet(int id, out Model model)
        {
            return idColl.TryGet(id, out model);
        }




        /// <summary> Loops through all Models in the static Model collection. </summary>
        public static void ForEach(Action<Model> action)
        {
            models.ForEach(action);
        }

        /// <summary> 
        /// Loops through all Models in the static Model collection. 
        /// Let the predicate return FALSE to BREAK the loop.
        /// </summary>
        public static void ForEachPredicate(Predicate<Model> predicate)
        {
            models.ForEachPredicate(predicate);
        }

        /// <summary> Gets the count of Models in the static Model collection. </summary>
        public static int Count { get { return models.Count; } }



        /// <summary> Loops through all dynamic Models in the static Model collection. </summary>
        public static void ForEachDynamic(Action<Model> action)
        {
            dynModels.ForEach(action);
        }

        /// <summary> 
        /// Loops through all dynamic Models in the static Model collection. 
        /// Let the predicate return FALSE to BREAK the loop.
        /// </summary>
        public static void ForEachDynamicPredicate(Predicate<Model> predicate)
        {
            dynModels.ForEachPredicate(predicate);
        }

        /// <summary> Gets the count of dynamic Models in the static Model collection. </summary>
        public static int CountDynamic { get { return dynModels.Count; } }

        #endregion

        #endregion

        #region Properties

        string visual = "";
        /// <summary>
        /// The Gothic visual of this Model. (case insensitive)
        /// </summary>
        public string Visual
        {
            get { return this.visual; }
            set
            {
                if (this.IsCreated)
                    throw new NotSupportedException("Can't change value when the Model is already created!");

                if (value == null)
                    this.visual = "";
                else
                    this.visual = value.ToUpper();
            }
        }

        #endregion

        #region Read & Write

        protected override void ReadProperties(PacketReader stream)
        {
            base.ReadProperties(stream);

            this.Visual = stream.ReadString();

            // overlays
            int count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                var ov = ScriptManager.Interface.CreateOverlay();
                ov.ReadStream(stream);
                this.ScriptObject.AddOverlay(ov);
            }

            // anijobs
            count = stream.ReadUShort();
            for (int i = 0; i < count; i++)
            {
                var job = ScriptManager.Interface.CreateAniJob();
                job.SetModel(this); // meh, so this AniJob can find its Overlays
                job.ReadStream(stream);
                job.SetModel(null);
                this.ScriptObject.AddAniJob(job);
            }
        }

        protected override void WriteProperties(PacketWriter stream)
        {
            base.WriteProperties(stream);

            stream.Write(this.Visual);

            // overlays
            stream.Write((byte)this.overlays.Count);
            dynOvs.ForEach(ov => ov.WriteStream(stream));

            // anijobs
            stream.Write((ushort)this.dynJobs.Count);
            dynJobs.ForEach(job => job.WriteStream(stream));
        }

        #endregion

        #region Animations

        /// <summary>
        /// The upper excluded limit for animations of a model. (ushort.MaxValue + 1)
        /// </summary>
        public const int MaxAnimations = 65536;

        StaticCollection<AniJob> aniIDs = new StaticCollection<AniJob>();
        DynamicCollection<AniJob> aniJobs = new DynamicCollection<AniJob>();
        DynamicCollection<AniJob> dynJobs = new DynamicCollection<AniJob>();

        #region Add & Remove

        partial void pAddAniJob(AniJob job);
        /// <summary> Adds an AniJob to this Model. </summary>
        public void AddAniJob(AniJob job)
        {
            if (job == null)
                throw new ArgumentNullException("AniJob is null!");

            if (job.IsCreated)
                throw new ArgumentException("AniJob is already added to another Model!");

            if (job.NextAni != null && job.NextAni.Model != this)
                throw new ArgumentException("AniJob's NextAni is for a different Model!");
            
            aniIDs.Add(job);
            aniJobs.Add(job, ref job.collID);
            dynJobs.Add(job, ref job.dynID);

            pAddAniJob(job);

            job.SetModel(this);
        }

        partial void pRemoveAniJob(AniJob job);
        public void RemoveAniJob(AniJob job)
        {
            if (job == null)
                throw new ArgumentNullException("AniJob is null!");

            if (job.Model != this)
                throw new Exception("AniJob is not from this Model!");

            job.SetModel(null);

            aniIDs.Remove(job);
            aniJobs.Remove(ref job.collID);
            dynJobs.Remove(ref job.dynID);

            pRemoveAniJob(job);
        }

        #endregion

        #region Access

        public bool ContainsAni(int id)
        {
            if (id < 0 || id >= MaxAnimations)
            {
                throw new ArgumentOutOfRangeException("ID is out of range! 0.." + MaxAnimations);
            }
            return aniIDs.ContainsID(id);
        }

        public bool TryGetAni(int id, out AniJob job)
        {
            if (id < 0 || id >= MaxAnimations)
            {
                throw new ArgumentOutOfRangeException("ID is out of range! 0.." + MaxAnimations);
            }
            return aniIDs.TryGet(id, out job);
        }



        public void ForEachAni(Action<AniJob> action)
        {
            aniJobs.ForEach(action);
        }

        public void ForEachAni(Predicate<AniJob> predicate)
        {
            aniJobs.ForEachPredicate(predicate);
        }

        public int GetAniCount() { return aniJobs.Count; }



        public void ForEachDynamicAni(Action<AniJob> action)
        {
            dynJobs.ForEach(action);
        }

        public void ForEachDynamicAni(Predicate<AniJob> predicate)
        {
            dynJobs.ForEachPredicate(predicate);
        }

        public int GetDynamicAniCount() { return dynJobs.Count; }

        #endregion

        #endregion

        #region Overlays

        /// <summary>
        /// The upper excluded limit for overlays of a model. (byte.MaxValue + 1)
        /// </summary>
        public const int MaxOverlays = 256;

        StaticCollection<Overlay> ovIDs = new StaticCollection<Overlay>(MaxOverlays);
        DynamicCollection<Overlay> overlays = new DynamicCollection<Overlay>(MaxOverlays);
        DynamicCollection<Overlay> dynOvs = new DynamicCollection<Overlay>(MaxOverlays);

        #region Add & Remove

        partial void pAddOverlay(Overlay overlay);
        public void AddOverlay(Overlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("Overlay is null!");

            if (overlay.IsCreated)
                throw new ArgumentException("Overlay is already added to another Model!");

            ovIDs.Add(overlay);
            overlays.Add(overlay, ref overlay.collID);
            dynOvs.Add(overlay, ref overlay.dynID);

            pAddOverlay(overlay);

            overlay.SetModel(this);
        }

        partial void pRemoveOverlay(Overlay overlay);
        public void RemoveOverlay(Overlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("Overlay is null!");

            if (overlay.Model != this)
                throw new Exception("Overlay is not from this Model!");

            overlay.SetModel(null);

            ovIDs.Remove(overlay);
            overlays.Remove(ref overlay.collID);
            dynOvs.Remove(ref overlay.dynID);

            pRemoveOverlay(overlay);
        }

        #endregion

        #region Access

        public bool ContainsOverlay(int id)
        {
            if (id < 0 || id >= MaxOverlays)
            {
                throw new ArgumentOutOfRangeException("ID is out of range! 0.." + MaxOverlays);
            }
            return aniIDs.ContainsID(id);
        }

        public bool TryGetOverlay(int id, out Overlay overlay)
        {
            if (id < 0 || id >= MaxOverlays)
            {
                throw new ArgumentOutOfRangeException("ID is out of range! 0.." + MaxOverlays);
            }
            return ovIDs.TryGet(id, out overlay);
        }



        public void ForEachOverlay(Action<Overlay> action)
        {
            overlays.ForEach(action);
        }

        public void ForEachOverlay(Predicate<Overlay> predicate)
        {
            overlays.ForEachPredicate(predicate);
        }

        public int GetOverlayCount() { return overlays.Count; }



        public void ForEachDynamicOverlay(Action<Overlay> action)
        {
            dynOvs.ForEach(action);
        }

        public void ForEachDynamicOverlay(Predicate<Overlay> predicate)
        {
            dynOvs.ForEachPredicate(predicate);
        }

        public int GetDynamicOverlayCount() { return dynOvs.Count; }

        #endregion

        #endregion
    }
}
