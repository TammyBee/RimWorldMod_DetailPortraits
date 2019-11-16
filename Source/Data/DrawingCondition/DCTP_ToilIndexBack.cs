using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_ToilIndexBack : DrawingConditionTermPreset {
        private List<int> toilIndexes = new List<int>();

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_ToilIndexBack_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (int rhs in toilIndexes) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(int);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(int);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_ToilIndexBack dctp = new DCTP_ToilIndexBack();
                dctp.toilIndexes = new List<int>();
                foreach (int rhs in this.toilIndexes) {
                    dctp.toilIndexes.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p.jobs?.curDriver != null) {
                List<Toil> toils = Traverse.Create(p.jobs.curDriver).Field("toils").GetValue<List<Toil>>();
                yield return toils.Count - p.jobs.curDriver.CurToilIndex - 1;
            } else {
                yield return 0;
            }
        }

        public override void AddRHS(object rhs) {
            toilIndexes.Add((int)rhs);
        }

        public override void RemoveAtRHS(int index) {
            toilIndexes.RemoveAt(index);
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref toilIndexes, "toilIndexes");
        }
    }
}
