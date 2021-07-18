using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_Hour : DrawingConditionTermPreset {
        public List<float> values = new List<float>();

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_Hour_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (float rhs in values) {
                    yield return rhs;
                }
            }
        }

        public override Type RHS_Type {
            get {
                return typeof(float);
            }
        }

        public override Type LHS_Type {
            get {
                return typeof(float);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                DCTP_Hour dctp = new DCTP_Hour();
                dctp.values = new List<float>();
                foreach (float rhs in this.values) {
                    dctp.values.Add(rhs);
                }
                return dctp;
            }
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            yield return GenLocalDate.HourFloat(p);
        }

        public override void AddRHS(object rhs) {
            values.Add((float)rhs);
        }

        public override void RemoveAtRHS(int index) {
            values.RemoveAt(index);
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref values, "values");
        }
    }
}
