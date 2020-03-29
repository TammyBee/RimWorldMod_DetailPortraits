using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_HasMajorBreakRisk : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_HasMajorBreakRisk_Label".Translate();
            }
        }

        public override string GroupStringKey {
            get {
                return "DetailPortraits.GroupDCTP_HasBreakRisk";
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_HasMajorBreakRisk();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return !p.Downed && p.mindState?.mentalBreaker != null && p.mindState.mentalBreaker.CurMood < p.mindState.mentalBreaker.BreakThresholdMajor;
        }
    }
}
