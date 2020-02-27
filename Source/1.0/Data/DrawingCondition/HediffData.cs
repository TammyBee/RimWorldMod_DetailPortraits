using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class HediffData : IExposable {
        private HediffDef hediffDef;
        private string hediffDefName;
        private int stageIndex;

        public HediffDef HediffDef {
            get {
                return this.hediffDef;
            }
        }

        public int StageIndex {
            get {
                return this.stageIndex;
            }
        }

        public HediffData() {}

        public HediffData(HediffDef hediffDef,int stageIndex) {
            this.hediffDef = hediffDef;
            this.hediffDefName = this.hediffDef.defName;
            this.stageIndex = stageIndex;
        }

        public void ExposeData() {
            Scribe_Values.Look(ref this.hediffDefName, "hediffDefName");
            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                hediffDef = DefDatabase<HediffDef>.GetNamed(this.hediffDefName, false);
            }
            Scribe_Values.Look(ref stageIndex, "stageIndex");
        }

        public override string ToString() {
            if (!hediffDef.stages.NullOrEmpty() && stageIndex < hediffDef.stages.Count) {
                string inner = hediffDef.stages[stageIndex].label;
                if (!inner.NullOrEmpty()) {
                    return hediffDef.label + ((!inner.NullOrEmpty()) ? (" (" + inner + ")") : string.Empty);
                }
            }
            return hediffDef.label;
        }

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) {
                return false;
            }

            HediffData hediffData = obj as HediffData;
            return hediffData != null && hediffDef == hediffData.hediffDef && stageIndex == hediffData.stageIndex;
        }

        public override int GetHashCode() {
            return hediffDef.defName.GetHashCode() + stageIndex;
        }
    }
}
