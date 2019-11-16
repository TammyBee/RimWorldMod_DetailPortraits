using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_Inspiration : DrawingConditionTermPreset {
        private List<string> rhsInspirationDefName = new List<string>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_Inspiration_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsInspirationDefName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(InspirationDef);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(InspirationDef);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_Inspiration dctp = new DCTP_Inspiration();
                dctp.rhsInspirationDefName = new List<string>();
                foreach (string rhs in this.rhsInspirationDefName) {
                    dctp.rhsInspirationDefName.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<Def> Defs {
            get {
                foreach(Def def in DefDatabase<InspirationDef>.AllDefsListForReading) {
                    yield return def;
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsInspirationDefName, "rhsInspirationDefName");
        }

        public override void AddRHS(object rhs) {
            Def def = rhs as Def;
            rhsInspirationDefName.Add(def.defName);
        }

        public override void RemoveAtRHS(int index) {
            rhsInspirationDefName.RemoveAt(index);
        }

        public override string ConvertDefToString(Def def) {
            return def.label;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            InspirationDef def = p.InspirationDef;
            if (def != null) {
                yield return def.defName;
            }
        }
    }
}
