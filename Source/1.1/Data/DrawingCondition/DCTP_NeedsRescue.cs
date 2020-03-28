using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_NeedsRescue : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_NeedsRescue_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_NeedsRescue();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return Alert_ColonistNeedsRescuing.NeedsRescue(p);
        }
    }
}
