using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class ThoughtData : IExposable {
        private ThoughtDef thoughtDef;
        private string thoughtDefName;
        private int stageIndex;

        public ThoughtDef ThoughtDef {
            get {
                return this.thoughtDef;
            }
        }

        public int StageIndex {
            get {
                return this.stageIndex;
            }
        }

        public ThoughtData() {}

        public ThoughtData(ThoughtDef thoughtDef, int stageIndex) {
            this.thoughtDef = thoughtDef;
            this.thoughtDefName = this.thoughtDef.defName;
            this.stageIndex = stageIndex;
        }

        public void ExposeData() {
            Scribe_Values.Look(ref this.thoughtDefName, "hediffDefName");
            if (Scribe.mode == LoadSaveMode.LoadingVars) {
                thoughtDef = DefDatabase<ThoughtDef>.GetNamed(this.thoughtDefName, false);
            }
            Scribe_Values.Look(ref stageIndex, "stageIndex");
        }

        public override string ToString() {
            if (!thoughtDef.stages.NullOrEmpty() && stageIndex < thoughtDef.stages.Count && thoughtDef.stages[stageIndex]?.label != null) {
                return thoughtDef.stages[stageIndex].label;
            }
            return thoughtDef.label;
        }

        public override bool Equals(object obj) {
            if (obj == null || this.GetType() != obj.GetType()) {
                return false;
            }

            ThoughtData thoughtData = obj as ThoughtData;
            return thoughtData != null && thoughtDef == thoughtData.thoughtDef && stageIndex == thoughtData.stageIndex;
        }

        public override int GetHashCode() {
            return thoughtDef.defName.GetHashCode() + stageIndex;
        }
    }
}
