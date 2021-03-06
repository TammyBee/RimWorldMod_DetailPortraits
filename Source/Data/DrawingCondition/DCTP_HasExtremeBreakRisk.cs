﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_HasExtremeBreakRisk : DCTP_Base_Bool {
        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_HasExtremeBreakRisk_Label".Translate();
            }
        }

        public override string GroupStringKey {
            get {
                return "DetailPortraits.GroupDCTP_HasBreakRisk";
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_HasExtremeBreakRisk();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return !p.Downed && p.mindState?.mentalBreaker != null && p.mindState.mentalBreaker.CurMood < p.mindState.mentalBreaker.BreakThresholdExtreme;
        }
    }
}
