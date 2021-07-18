using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_AllThoughts : DrawingConditionTermPreset {
        private List<string> rhsThoughtDefName = new List<string>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_AllThoughts_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsThoughtDefName) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(string);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(string);
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
                DCTP_AllThoughts dctp = new DCTP_AllThoughts();
                dctp.rhsThoughtDefName = new List<string>();
                foreach (string rhs in this.rhsThoughtDefName) {
                    dctp.rhsThoughtDefName.Add(rhs);
                }
                return dctp;
            }
        }



        public override IEnumerable<object> RHS_Items {
            get {
                foreach (ThoughtDef def in DefDatabase<ThoughtDef>.AllDefsListForReading) {
                    if (!def.IsSocial) {
                        yield return GetDefLabel(def);
                    }
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsThoughtDefName, "rhsThoughtDefName");
        }

        public override void AddRHS(object rhs) {
            rhsThoughtDefName.Add((string)rhs);
        }

        public override void RemoveAtRHS(int index) {
            rhsThoughtDefName.RemoveAt(index);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p?.needs?.mood?.thoughts != null) {
                List<Thought> thoughts = new List<Thought>();
                p.needs.mood.thoughts.GetDistinctMoodThoughtGroups(thoughts);
                foreach (Thought thought in thoughts) {
                    yield return GetDefLabel(thought.def);
                }
            }
        }

        private static string GetDefLabel(ThoughtDef def) {
            string label = "";
            foreach (ThoughtStage stage in def.stages) {
                if (stage != null && !stage.label.NullOrEmpty()) {
                    if (label.NullOrEmpty()) {
                        label = stage.label;
                    } else {
                        label += "DetailPortraits.CustomDataListPostfix".Translate();
                        break;
                    }
                }
            }
            return label;
        }
    }
}
