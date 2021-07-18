using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_IsHungry : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_IsHungry_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_IsHungry();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p.needs?.food != null && p.needs.food.Starving;
        }
    }
}
