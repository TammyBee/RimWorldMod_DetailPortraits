using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_JobTargetThing : DrawingConditionTermPreset {
        private TargetIndex targetIndex = TargetIndex.None;
        private List<string> rhsTargetDefName = new List<string>();

        public override List<DrawingConditionTermPreset> Generators {
            get {
                List<DrawingConditionTermPreset> generators = new List<DrawingConditionTermPreset>();
                generators.Add(new DCTP_JobTargetThing(TargetIndex.A));
                generators.Add(new DCTP_JobTargetThing(TargetIndex.B));
                generators.Add(new DCTP_JobTargetThing(TargetIndex.C));
                return generators;
            }
        }

        public override string GroupStringKey {
            get {
                return "DetailPortraits.GroupDCTP_JobTargetThing";
            }
        }

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_JobTargetThing_Label".Translate(targetIndex.ToString());
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsTargetDefName) {
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

        public override bool IsCustomType {
            get {
                return true;
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_JobTargetThing dctp = new DCTP_JobTargetThing();
                dctp.targetIndex = this.targetIndex;
                dctp.rhsTargetDefName = new List<string>();
                foreach (string rhs in this.rhsTargetDefName) {
                    dctp.rhsTargetDefName.Add(rhs);
                }
                return dctp;
            }
        }



        public override IEnumerable<object> RHS_Items {
            get {
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
                    yield return def.label;
                }
            }
        }

        public DCTP_JobTargetThing() { }

        public DCTP_JobTargetThing(TargetIndex targetIndex) {
            this.targetIndex = targetIndex;
        }

        public override void ExposeDataInternal() {
            Scribe_Values.Look(ref this.targetIndex, "targetIndex");
            Scribe_Collections.Look(ref rhsTargetDefName, "rhsTargetDefName");
        }

        public override void AddRHS(object rhs) {
            rhsTargetDefName.Add((string)rhs);
        }

        public override void RemoveAtRHS(int index) {
            rhsTargetDefName.RemoveAt(index);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.CurJob != null && this.targetIndex != TargetIndex.None) {
                LocalTargetInfo target = p.CurJob.GetTarget(this.targetIndex);
                if (target.HasThing) {
                    yield return target.Thing.def.label;
                } else {
                    yield return "";
                }
            } else {
                yield return "";
            }
        }
    }
}
