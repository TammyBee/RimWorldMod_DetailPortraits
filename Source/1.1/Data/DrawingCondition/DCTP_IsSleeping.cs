using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_IsSleeping : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_IsSleeping_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_IsSleeping();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p?.CurJob != null && p?.jobs?.curDriver != null && p.jobs.curDriver.asleep;
        }
    }
}
