using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_IsBored : DrawingConditionTermPreset {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_IsBored_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                yield return true;
            }
        }

        public override bool IsBoolPreset {
            get {
                return true;
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(bool);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(bool);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_IsBored();
            }
        }

        public override void ExposeDataInternal() {
        }

        public override void AddRHS(object rhs) {
        }

        public override void RemoveAtRHS(int index) {
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p.needs?.joy != null && (p.needs.joy.CurLevelPercentage < 0.24f || p.GetTimeAssignment() == TimeAssignmentDefOf.Joy) && p.needs.joy.tolerances.BoredOfAllAvailableJoyKinds(p);
        }
    }
}
