using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_BleedRate : DrawingConditionTermPreset {
        public List<float> bleedRates = new List<float>();

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_BleedRate_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (float rhs in bleedRates) {
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
                return new DCTP_BleedRate();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.health?.hediffSet != null) {
                yield return p.health.hediffSet.BleedRateTotal;
            } else {
                yield return 0f;
            }
        }

        public override void AddRHS(object rhs) {
            bleedRates.Add((float)rhs);
        }

        public override void RemoveAtRHS(int index) {
            bleedRates.RemoveAt(index);
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref bleedRates, "bleedRates");
        }
    }
}
