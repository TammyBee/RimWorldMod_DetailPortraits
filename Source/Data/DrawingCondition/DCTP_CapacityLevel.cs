using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_CapacityLevel : DrawingConditionTermPreset {
        private PawnCapacityDef capacityDef;
        private string capacityDefName;
        public List<float> levels = new List<float>();

        public override List<DrawingConditionTermPreset> Generators {
            get {
                List<DrawingConditionTermPreset> generators = new List<DrawingConditionTermPreset>();
                foreach (PawnCapacityDef def in DefDatabase<PawnCapacityDef>.AllDefsListForReading) {
                    generators.Add(new DCTP_CapacityLevel(def));
                }
                return generators;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_CapacityLevel_Label".Translate(capacityDef.label);
            }
        }

        public override string GroupStringKey {
            get {
                return "DetailPortraits.GroupDCTP_CapacityLevel";
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (float rhs in levels) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(float);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(float);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_CapacityLevel dctp = new DCTP_CapacityLevel(this.capacityDef);
                dctp.levels = new List<float>();
                foreach (float rhs in this.levels) {
                    dctp.levels.Add(rhs);
                }
                return dctp;
            }
        }

        public DCTP_CapacityLevel() { }

        public DCTP_CapacityLevel(PawnCapacityDef capacityDef) {
            this.capacityDef = capacityDef;
            this.capacityDefName = this.capacityDef.defName;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.health?.hediffSet != null && capacityDef != null) {
                yield return PawnCapacityUtility.CalculateCapacityLevel(p.health.hediffSet, capacityDef, null, false);
            } else {
                yield return 0f;
            }
        }

        public override void AddRHS(object rhs) {
            levels.Add((float)rhs);
        }

        public override void RemoveAtRHS(int index) {
            levels.RemoveAt(index);
        }

        public override void ExposeDataInternal() {
            Scribe_Values.Look(ref this.capacityDefName, "capacityDef");
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs) {
                capacityDef = DefDatabase<PawnCapacityDef>.GetNamed(this.capacityDefName, false);
            }
            Scribe_Collections.Look(ref levels, "levels");
        }
    }
}
