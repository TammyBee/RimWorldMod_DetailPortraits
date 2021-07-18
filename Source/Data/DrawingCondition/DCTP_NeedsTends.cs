using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_NeedsTends : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_NeedsTends_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_NeedsTends();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p.health != null && p.health.HasHediffsNeedingTendByPlayer(false);
        }
    }
}
