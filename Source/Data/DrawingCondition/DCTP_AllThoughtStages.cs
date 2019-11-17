using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_AllThoughtStages : DrawingConditionTermPreset {
        private List<ThoughtData> rhsThoughtData = new List<ThoughtData>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_AllThoughtStages_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (ThoughtData rhs in rhsThoughtData) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(ThoughtData);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(ThoughtData);
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
                DCTP_AllThoughtStages dctp = new DCTP_AllThoughtStages();
                dctp.rhsThoughtData = new List<ThoughtData>();
                foreach (ThoughtData rhs in this.rhsThoughtData) {
                    dctp.rhsThoughtData.Add(new ThoughtData(rhs.ThoughtDef,rhs.StageIndex));
                }
                return dctp;
            }
        }

        public override IEnumerable<object> RHS_Items {
            get {
                foreach (ThoughtDef def in DefDatabase<ThoughtDef>.AllDefsListForReading) {
                    if (!def.IsSocial) {
                        if (def.stages.NullOrEmpty()) {
                            yield return new ThoughtData(def, 0);
                        } else {
                            for (int i = 0; i < def.stages.Count; i++) {
                                yield return new ThoughtData(def, i);
                            }
                        }
                    }
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsThoughtData, "rhsThoughtData");
        }

        public override void AddRHS(object rhs) {
            rhsThoughtData.Add((ThoughtData)rhs);
        }

        public override void RemoveAtRHS(int index) {
            rhsThoughtData.RemoveAt(index);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            List<Thought> thoughts = new List<Thought>();
            p.needs.mood.thoughts.GetDistinctMoodThoughtGroups(thoughts);
            foreach (Thought thought in thoughts) {
                yield return new ThoughtData(thought.def, thought.CurStageIndex);
            }
        }
    }
}
