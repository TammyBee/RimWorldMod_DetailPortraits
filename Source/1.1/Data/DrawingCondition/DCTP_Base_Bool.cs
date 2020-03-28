using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public abstract class DCTP_Base_Bool : DrawingConditionTermPreset {
        public override IEnumerable<object> RHS {
            get {
                yield return true;
            }
        }

        public override bool IsBoolPreset {
            get {
                return true;
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(bool);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(bool);
            }
        }

        public override void ExposeDataInternal() {
        }

        public override void AddRHS(object rhs) {
        }

        public override void RemoveAtRHS(int index) {
        }
    }
}
