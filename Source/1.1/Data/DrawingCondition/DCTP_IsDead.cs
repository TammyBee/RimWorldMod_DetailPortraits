using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_IsDead : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_IsDead_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_IsDead();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            Log.Message("DCTP_IsDead " + p.ToStringSafe() + ":" + p.Dead);
            yield return p.Dead;
        }
    }
}
