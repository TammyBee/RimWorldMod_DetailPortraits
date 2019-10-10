using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace DetailPortraits.Data {
    public class TextureData : IExposable {
        public List<string> texturePaths = new List<string>();

        private List<string> cacheCandidatePaths = new List<string>();
        private GraphicData cacheGraphicData;

        public List<string> CandidatePaths {
            get {
                if (this.cacheCandidatePaths.NullOrEmpty()) {
                    RefreshCandidatePaths();
                }
                return this.cacheCandidatePaths;
            }
        }

        public Graphic GetGraphic(float scale) {
            if (this.cacheGraphicData == null) {
                RefreshGraphic(scale);
            }
            return this.cacheGraphicData.Graphic;
        }

        public void RefreshGraphic(float scale) {
            string texturePath = CandidatePaths.RandomElement();
            this.cacheGraphicData = new GraphicData();
            this.cacheGraphicData.graphicClass = typeof(Graphic_Single);
            this.cacheGraphicData.texPath = texturePath;
            this.cacheGraphicData.shaderType = ShaderTypeDefOf.Transparent;
            this.cacheGraphicData.drawSize = Vector2.one * scale;
            this.cacheGraphicData.color = Color.white;
        }

        public void RefreshCandidatePaths() {
            this.cacheCandidatePaths = new List<string>();
            this.cacheCandidatePaths.AddRange(this.texturePaths);
        }

        public void ExposeData() {
            Scribe_Collections.Look(ref texturePaths, "texturePaths");
        }
    }
}
