using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DrawingConditionData : IExposable {
        public DrawingConditionTermPreset lhsPreset;
        public DrawingConditionOperator op;

        public bool isReversed;

        public bool IsAvailable {
            get {
                return lhsPreset.IsAvailable || op == DrawingConditionOperator.IsEmpty;
            }
        }

        public DrawingConditionData() {
            this.lhsPreset = DrawingConditionTermPresetManager.Presets[0].Copy;
            this.op = this.lhsPreset.AvailableOperators[0];
            this.isReversed = false;
        }

        public DrawingConditionData(DrawingConditionTermPreset lhsPreset, DrawingConditionOperator op, bool isReversed) {
            this.lhsPreset = lhsPreset.Copy;
            this.op = op;
            this.isReversed = isReversed;
        }

        public bool IsSatisfied(Pawn p) {
            if (!IsAvailable) {
                return false;
            }
            return IsSatisfiedInternal(p) ^ isReversed;
        }

        private bool IsSatisfiedInternal(Pawn p) {
            if (p == null) {
                return lhsPreset is DCTP_IsDead;
            }
            IEnumerable<object> lhs = lhsPreset.GetValue(p);
            if (op == DrawingConditionOperator.Equal) {
                return !lhs.EnumerableNullOrEmpty() && !lhsPreset.RHS.EnumerableNullOrEmpty() && lhs.First().Equals(lhsPreset.RHS.First());
            } else if (op == DrawingConditionOperator.GT) {
                float lhsValue = (float)lhs.First();
                float rhsValue = (float)lhsPreset.RHS.First();
                return lhsValue > rhsValue;
            } else if (op == DrawingConditionOperator.GTE) {
                float lhsValue = (float)lhs.First();
                float rhsValue = (float)lhsPreset.RHS.First();
                return lhsValue >= rhsValue;
            } else if (op == DrawingConditionOperator.LT) {
                float lhsValue = (float)lhs.First();
                float rhsValue = (float)lhsPreset.RHS.First();
                return lhsValue < rhsValue;
            } else if (op == DrawingConditionOperator.LTE) {
                float lhsValue = (float)lhs.First();
                float rhsValue = (float)lhsPreset.RHS.First();
                return lhsValue <= rhsValue;
            } else if (op == DrawingConditionOperator.In) {
                return !lhs.EnumerableNullOrEmpty() && !lhsPreset.RHS.EnumerableNullOrEmpty() && lhsPreset.RHS.Contains(lhs.First());
            } else if (op == DrawingConditionOperator.Contains) {
                return !lhs.EnumerableNullOrEmpty() && !lhsPreset.RHS.EnumerableNullOrEmpty() && lhsPreset.RHS.All(rhs => lhs.Contains(rhs));
            } else if (op == DrawingConditionOperator.Shared) {
                return !lhs.EnumerableNullOrEmpty() && !lhsPreset.RHS.EnumerableNullOrEmpty() && lhsPreset.RHS.SharesElementWith(lhs);
            } else if (op == DrawingConditionOperator.IsEmpty) {
                return lhs.EnumerableNullOrEmpty();
            }
            return true;
        }

        public string GetLabel() {
            string strReverse = "";
            if (isReversed) {
                strReverse = "DetailPortraits.IsReverseLabel".Translate();
            }
            string rhsLabel = lhsPreset.RHSLabel;
            if (op == DrawingConditionOperator.IsEmpty) {
                rhsLabel = "";
            }
            return "DetailPortraits.DrawingConditionDataLabel".Translate(lhsPreset.PresetLabelInLayerList, op.GetLabel(), rhsLabel, strReverse);
        }

        public void ExposeData() {
            Scribe_Deep.Look(ref lhsPreset, "lhsPreset");
            Scribe_Values.Look(ref op, "op");
            Scribe_Values.Look(ref isReversed, "isReversed");
        }
    }
}
