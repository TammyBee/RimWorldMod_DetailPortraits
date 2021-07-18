using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_Need : DrawingConditionTermPreset {
        private NeedDef needDef;
        private string needDefName;
        public List<float> levels = new List<float>();

        public override List<DrawingConditionTermPreset> Generators {
            get {
                List<DrawingConditionTermPreset> generators = new List<DrawingConditionTermPreset>();
                foreach (NeedDef needDef in DefDatabase<NeedDef>.AllDefsListForReading) {
                    generators.Add(new DCTP_Need(needDef));
                }
                return generators;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_Need_Label".Translate(needDef.label);
            }
        }

        public override string GroupStringKey {
            get {
                return "DetailPortraits.GroupDCTP_Need";
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (float rhs in levels) {
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
                DCTP_Need dctp = new DCTP_Need(this.needDef);
                dctp.levels = new List<float>();
                foreach (float rhs in this.levels) {
                    dctp.levels.Add(rhs);
                }
                return dctp;
            }
        }

        public DCTP_Need() { }

        public DCTP_Need(NeedDef needDef) {
            this.needDef = needDef;
            this.needDefName = this.needDef.defName;
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            Need need = p.needs?.TryGetNeed(this.needDef);
            if (need != null) {
                yield return need.CurLevelPercentage;
            } else {
                yield return 0f;
            }
        }

        public override void AddRHS(object rhs) {
            levels.Add((float)rhs);
        }

        public override void RemoveAtRHS(int index) {
            levels.RemoveAt(index);
        }

        public override void ExposeDataInternal() {
            Scribe_Values.Look(ref this.needDefName, "needDef");
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs) {
                needDef = DefDatabase<NeedDef>.GetNamed(this.needDefName, false);
            }
            Scribe_Collections.Look(ref levels, "levels");
        }
    }
}
