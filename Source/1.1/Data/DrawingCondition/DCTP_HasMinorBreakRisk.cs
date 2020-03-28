﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_HasMinorBreakRisk : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_HasMinorBreakRisk_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_HasMinorBreakRisk();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return !p.Downed && p.mindState?.mentalBreaker != null && p.mindState.mentalBreaker.CurMood < p.mindState.mentalBreaker.BreakThresholdMinor;
        }
    }
}
