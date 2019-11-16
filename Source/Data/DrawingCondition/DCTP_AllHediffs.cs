using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_AllHediffs : DrawingConditionTermPreset {
        protected List<string> rhsHediffDefName = new List<string>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_AllHediffs_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsHediffDefName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(HediffDef);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(HediffDef);
            }
        }

        public override bool IsListLHS {
            get {
                return true;
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_AllHediffs dctp = new DCTP_AllHediffs();
                dctp.rhsHediffDefName = new List<string>();
                foreach (string rhs in this.rhsHediffDefName) {
                    dctp.rhsHediffDefName.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<Def> Defs {
            get {
                foreach(Def def in DefDatabase<HediffDef>.AllDefsListForReading) {
                    yield return def;
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsHediffDefName, "rhsHediffDefName");
        }

        public override void AddRHS(object rhs) {
            Def def = rhs as Def;
            rhsHediffDefName.Add(def.defName);
        }

        public override void RemoveAtRHS(int index) {
            rhsHediffDefName.RemoveAt(index);
        }

        public override string ConvertDefToString(Def def) {
            return def.label;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.health?.hediffSet != null) {
                foreach (Hediff hediff in p.health.hediffSet.hediffs) {
                    yield return hediff.def.defName;
                }
            }
        }
    }
}
