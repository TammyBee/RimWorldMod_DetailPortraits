using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_IsBored : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_IsBored_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_IsBored();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p.needs?.joy != null && (p.needs.joy.CurLevelPercentage < 0.24f || p.GetTimeAssignment() == TimeAssignmentDefOf.Joy) && p.needs.joy.tolerances.BoredOfAllAvailableJoyKinds(p);
        }
    }
}
