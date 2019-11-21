using DetailPortraits.Data.DrawingCondition;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
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
        }

        public bool CanRender(Pawn p) {
            if (drawingConditions.NullOrEmpty()) {
                return true;
            }
            return drawingConditions.All(c => c.IsSatisfied(p));
        }

        public void Render(Vector2 globalPosition,float globalScale) {
            Vector2 position = globalPosition + this.localPosition;
            float scale = globalScale * this.localScale;

            Graphic graphic = textureData.GetGraphic(scale);
            if (graphic != null) {
                Quaternion quaternion = Quaternion.AngleAxis(0f, Vector3.up);
                Mesh mesh = MeshPool.humanlikeBodySet.MeshAt(Rot4.South);
                GenDraw.DrawMeshNowOrLater(graphic.MeshAt(Rot4.South), new Vector3(position.x, LayerBaseY + layerNumber * 0.01f, position.y), quaternion, graphic.MatSingle, true);
            }
        }

        private static void DrawTexturePart(Rect drawRect, Rect uvRect, Texture tex) {
            uvRect.y = 1f - uvRect.y - uvRect.height;
            GUI.DrawTextureWithTexCoords(drawRect, tex, uvRect);
        }

        public void Refresh(float globalScale) {
            textureData.RefreshGraphic(this.localScale * globalScale);
        }

        public void ExposeData() {
            Scribe_Values.Look(ref layerNumber, "layerNumber");
            Scribe_Values.Look(ref layerName, "layerName");
            Scribe_Collections.Look(ref drawingConditions, "drawingConditions", LookMode.Deep);
            Scribe_Deep.Look(ref textureData, "textureData");
            Scribe_Values.Look(ref localPosition, "localPosition");
            Scribe_Values.Look(ref localScale, "localScale");
        }

        public override string ToString() {
            return string.Format("[{0}] {1}", layerNumber,layerName);
        }
    }
}
