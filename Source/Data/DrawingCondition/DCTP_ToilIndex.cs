using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_ToilIndex : DrawingConditionTermPreset {
        private List<int> toilIndexes = new List<int>();

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_ToilIndex_Label".Translate();
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
                return new DCTP_ToilIndex();
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return p.jobs.curDriver.CurToilIndex;
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
