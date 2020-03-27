using RimWorld;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits.Data {
    public class PortraitData : IExposable {
        public RenderMode renderMode = RenderMode.Default;
        public Vector2 globalPosition = Vector2.zero;
        public float globalScale = 1f;
        public float globalScaleH = 1f;
        public List<LayerData> layers = new List<LayerData>();
        public int refreshTick = 240;
        public bool hideIcon = false;
        public string rootPath = "";

        public Pawn pawn;

        public int lastRefreshTick = 0;

        List<LayerData> cacheRenderableLayers = new List<LayerData>();

        public PortraitData() {
        }

        public PortraitData(Pawn pawn) {
            this.pawn = pawn;
        }

        public PortraitData(PortraitData src,Pawn pawn) {
            CopyFrom(src);
            this.pawn = pawn;
        }

        public List<LayerData> RenderableLayers {
            get {
                if (this.cacheRenderableLayers == null) {
                    RefreshRenderableLayers();
                }
                return this.cacheRenderableLayers;
            }
        }

        public void CopyFrom(PortraitData src) {
            this.renderMode = src.renderMode;
            this.globalPosition = src.globalPosition;
            this.globalScale = src.globalScale;
            this.globalScaleH = src.globalScaleH;
            this.layers = src.layers.ConvertAll(layer => new LayerData(layer));
            this.refreshTick = src.refreshTick;
            this.hideIcon = src.hideIcon;
            this.rootPath = src.rootPath;
        }

        public void RefreshRenderableLayers() {
            List<int> filledLayerNumbers = new List<int>();
            this.cacheRenderableLayers = new List<LayerData>();
            foreach (LayerData layer in this.layers) {
                if (!filledLayerNumbers.Contains(layer.layerNumber) && layer.CanRender(this.pawn)) {
                    filledLayerNumbers.Add(layer.layerNumber);
                    layer.Refresh(this.globalScale, this.globalScaleH, this.rootPath);
                    this.cacheRenderableLayers.Add(layer);
                }
            }
            this.cacheRenderableLayers.SortBy(layer => layer.layerNumber);
            PortraitsCache.SetDirty(pawn);
            //Log.Message("[RefreshRenderableLayers]\n" + string.Join("\n", this.cacheRenderableLayers.ConvertAll(l => l.ToString()).ToArray()));
            this.lastRefreshTick = Find.TickManager.TicksGame;
        }

        public void Render() {
            foreach (LayerData layer in RenderableLayers) {
                layer.Render(this.globalPosition, this.globalScale, this.globalScaleH, this.rootPath);
            }
        }

        public void Tick() {
            if (Find.TickManager.TicksGame - this.lastRefreshTick > this.refreshTick) {
                RefreshRenderableLayers();
                this.lastRefreshTick = Find.TickManager.TicksGame;
            }
        }

        public bool HasDCTP(Type dctp_type) {
            foreach (LayerData layerData in layers) {
                foreach (DrawingCondition.DrawingConditionData c in layerData.drawingConditions) {
                    if (c.lhsPreset.GetType() == dctp_type) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ExposeData() {
            Scribe_Values.Look(ref renderMode, "renderMode");
            Scribe_Values.Look(ref globalPosition, "globalPosition");
            float x = 0,y = 0;
            if (Scribe.mode == LoadSaveMode.Saving) {
                x = globalPosition.x;
                y = globalPosition.y;
            }
            Scribe_Values.Look(ref x, "globalPositionX", globalPosition.x, true);
            Scribe_Values.Look(ref y, "globalPositionY", globalPosition.y, true);
            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                globalPosition.x = x;
                globalPosition.y = y;
            }
            Scribe_Values.Look(ref globalScale, "globalScale");
            Scribe_Values.Look(ref globalScaleH, "globalScaleH", globalScale);
            Scribe_Collections.Look(ref layers, "layers", LookMode.Deep);
            Scribe_Values.Look(ref refreshTick, "refreshTick");
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref hideIcon, "hideIcon", false);
            Scribe_Values.Look(ref rootPath, "rootPath", ""); 

            Scribe_Values.Look(ref lastRefreshTick, "lastRefreshTick");

            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                foreach(LayerData layer in layers) {
                    layer.parent = this;
                }
            }
        }
    }
}
