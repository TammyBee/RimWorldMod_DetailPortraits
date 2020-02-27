using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_MentalState : DrawingConditionTermPreset {
        private List<string> rhsMentalStateDefName = new List<string>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_MentalState_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsMentalStateDefName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(MentalStateDef);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(MentalStateDef);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_MentalState dctp = new DCTP_MentalState();
                dctp.rhsMentalStateDefName = new List<string>();
                foreach (string rhs in this.rhsMentalStateDefName) {
                    dctp.rhsMentalStateDefName.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<Def> Defs {
            get {
                foreach(Def def in DefDatabase<MentalStateDef>.AllDefsListForReading) {
                    yield return def;
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsMentalStateDefName, "rhsJobDefName");
        }

        public override void AddRHS(object rhs) {
            Def def = rhs as Def;
            rhsMentalStateDefName.Add(def.defName);
        }

        public override void RemoveAtRHS(int index) {
            rhsMentalStateDefName.RemoveAt(index);
        }

        public override string ConvertDefToString(Def def) {
            return def.LabelCap;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p?.MentalStateDef != null) {
                yield return p.MentalStateDef.defName;
            } else {
                yield return "";
            }
        }
    }
}
