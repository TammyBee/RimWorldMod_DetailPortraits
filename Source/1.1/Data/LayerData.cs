using DetailPortraits.Data.DrawingCondition;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits.Data {
    public class LayerData : IExposable {
        private const float LayerBaseY = 2f;

        public int layerNumber = 0;
        public string layerName = "";
        public List<DrawingConditionData> drawingConditions = new List<DrawingConditionData>();
        public TextureData textureData = new TextureData();
        public Vector2 localPosition = Vector2.zero;
        public float localScale = 1f;
        public float localScaleH = 1f;
        public bool suspended;

        public PortraitData parent;

        public bool IsAvailable {
            get {
                return drawingConditions.NullOrEmpty() || drawingConditions.All(dc => dc.IsAvailable);
            }
        }

        public LayerData() {

        }

        public LayerData(PortraitData parent) {
            this.parent = parent;
        }

        public LayerData(LayerData src) {
            this.parent = src.parent;
            this.layerNumber = src.layerNumber;
            this.layerName = src.layerName;
            this.drawingConditions = new List<DrawingConditionData>();
            foreach (DrawingConditionData dcd in src.drawingConditions) {
                this.drawingConditions.Add(new DrawingConditionData(dcd.lhsPreset,dcd.op,dcd.isReversed));
            }
            this.textureData = src.textureData.Copy;
            this.localPosition = src.localPosition;
            this.localScale = src.localScale;
            this.localScaleH = src.localScaleH;
            this.suspended = src.suspended;
        }

        public bool CanRender(Pawn p) {
            if (this.suspended) {
                return false;
            }
            if (this.textureData.CandidatePaths.NullOrEmpty()) {
                return false;
            }
            if (drawingConditions.NullOrEmpty()) {
                return true;
            }
            return drawingConditions.All(c => c.IsSatisfied(p));
        }

        public void Render(Vector2 globalPosition, float globalScale, float globalScaleH, string rootPath) {
            Vector2 position = globalPosition + this.localPosition;
            float scale = globalScale * this.localScale;
            float scaleH = globalScaleH * this.localScaleH;

            Graphic graphic = textureData.GetGraphic(scale, scaleH, rootPath);
            if (graphic != null) {
                Quaternion quaternion = Quaternion.AngleAxis(0f, Vector3.up);
                Mesh mesh = null;
                if (DetailPortraitsMod.Settings.normalizeScale) {
                    float w = graphic.MatSingle.mainTexture.width;
                    float h = graphic.MatSingle.mainTexture.height;
                    mesh = MeshPool.GridPlane(new Vector2(1f * scale, (h / w) * scaleH));
                } else {
                    mesh = graphic.MeshAt(Rot4.South);
                }
                GenDraw.DrawMeshNowOrLater(mesh, new Vector3(position.x, LayerBaseY + layerNumber * 0.01f, position.y), quaternion, graphic.MatSingle, true);
            }
        }

        private static void DrawTexturePart(Rect drawRect, Rect uvRect, Texture tex) {
            uvRect.y = 1f - uvRect.y - uvRect.height;
            GUI.DrawTextureWithTexCoords(drawRect, tex, uvRect);
        }

        public void Refresh(float globalScale, float globalScaleH, string rootPath) {
            textureData.RefreshGraphic(this.localScale * globalScale, this.localScaleH * globalScaleH, rootPath);
        }

        public void ExposeData() {
            Scribe_Values.Look(ref layerNumber, "layerNumber");
            Scribe_Values.Look(ref layerName, "layerName");
            Scribe_Collections.Look(ref drawingConditions, "drawingConditions", LookMode.Deep);
            Scribe_Deep.Look(ref textureData, "textureData");
            Scribe_Values.Look(ref localPosition, "localPosition");
            float x = 0, y = 0;
            if (Scribe.mode == LoadSaveMode.Saving) {
                x = localPosition.x;
                y = localPosition.y;
            }
            Scribe_Values.Look(ref x, "localPositionX", localPosition.x, true);
            Scribe_Values.Look(ref y, "localPositionY", localPosition.y, true);
            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                localPosition.x = x;
                localPosition.y = y;
            }
            Scribe_Values.Look(ref localScale, "localScale");
            Scribe_Values.Look(ref localScaleH, "localScaleH", localScale);
            Scribe_Values.Look(ref suspended, "suspended");
        }

        public override string ToString() {
            return string.Format("[{0}] {1}", layerNumber,layerName);
        }
    }
}
