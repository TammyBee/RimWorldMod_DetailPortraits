using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_VisibleHediffs : DCTP_AllHediffs {
        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_VisibleHediffs_Label".Translate();
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_VisibleHediffs dctp = new DCTP_VisibleHediffs();
                dctp.rhsHediffDefName = new List<string>();
                foreach (string rhs in this.rhsHediffDefName) {
                    dctp.rhsHediffDefName.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.health?.hediffSet != null) {
                foreach (Hediff hediff in p.health.hediffSet.hediffs) {
                    if (hediff.Visible) {
                        yield return hediff.def.defName;
                    }
                }
            }
        }
    }
}
