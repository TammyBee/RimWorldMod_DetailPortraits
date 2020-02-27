using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_Job : DrawingConditionTermPreset {
        private List<string> rhsJobDefName = new List<string>();

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_Job_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsJobDefName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(JobDef);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(JobDef);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_Job dctp = new DCTP_Job();
                dctp.rhsJobDefName = new List<string>();
                foreach (string rhs in this.rhsJobDefName) {
                    dctp.rhsJobDefName.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<Def> Defs {
            get {
                foreach(Def def in DefDatabase<JobDef>.AllDefsListForReading) {
                    yield return def;
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsJobDefName, "rhsJobDefName");
        }

        public override void AddRHS(object rhs) {
            Def def = rhs as Def;
            rhsJobDefName.Add(def.defName);
        }

        public override void RemoveAtRHS(int index) {
            rhsJobDefName.RemoveAt(index);
        }

        public override string ConvertDefToString(Def def) {
            JobDef jobDef = def as JobDef;
            return jobDef.reportString;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.CurJobDef != null) {
                yield return p.CurJobDef.defName;
            } else {
                yield return "";
            }
        }
    }
}
