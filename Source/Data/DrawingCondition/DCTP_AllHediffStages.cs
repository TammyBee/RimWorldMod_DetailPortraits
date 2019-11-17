using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_AllHediffStages : DrawingConditionTermPreset {
        private List<HediffData> rhsHediffDefAndStageName = new List<HediffData>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_AllHediffStages_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (HediffData rhs in rhsHediffDefAndStageName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(HediffData);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(HediffData);
            }
        }

        public override bool IsListLHS {
            get {
                return true;
            }
        }

        public override bool IsCustomType {
            get {
                return true;
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_AllHediffStages dctp = dctp = new DCTP_AllHediffStages();
                dctp.rhsHediffDefAndStageName = new List<HediffData>();
                foreach (HediffData rhs in this.rhsHediffDefAndStageName) {
                    dctp.rhsHediffDefAndStageName.Add(new HediffData(rhs.HediffDef,rhs.StageIndex));
                }
                return dctp;
            }
        }

        public override IEnumerable<object> RHS_Items {
            get {
                foreach(HediffDef def in DefDatabase<HediffDef>.AllDefsListForReading) {
                    if (def.stages.NullOrEmpty()) {
                        yield return new HediffData(def,0);
                    } else {
                        for(int i=0; i < def.stages.Count;i++) {
                            yield return new HediffData(def, i);
                        }
                    }
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsHediffDefAndStageName, "rhsHediffDefName");
        }

        public override void AddRHS(object rhs) {
            rhsHediffDefAndStageName.Add((HediffData)rhs);
        }

        public override void RemoveAtRHS(int index) {
            rhsHediffDefAndStageName.RemoveAt(index);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.health?.hediffSet != null) {
                foreach (Hediff hediff in p.health.hediffSet.hediffs) {
                    yield return new HediffData(hediff.def, hediff.CurStageIndex);
                }
            }
        }
    }
}
