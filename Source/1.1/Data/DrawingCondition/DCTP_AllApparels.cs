using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_AllApparels : DrawingConditionTermPreset {
        private List<string> rhsApparelDefName = new List<string>();

        public override bool AllowEmpty {
            get {
                return true;
            }
        }

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_AllApparels_Label".Translate();
            }
        }

        public override IEnumerable<object> RHS {
            get {
                foreach (string rhs in rhsApparelDefName) {
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
                DCTP_AllApparels dctp = new DCTP_AllApparels();
                dctp.rhsApparelDefName = new List<string>();
                foreach (string rhs in this.rhsApparelDefName) {
                    dctp.rhsApparelDefName.Add(rhs);
                }
                return dctp;
            }
        }



        public override IEnumerable<object> RHS_Items {
            get {
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading) {
                    if (def.IsApparel) {
                        yield return def.label;
                    }
                }
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Collections.Look(ref rhsApparelDefName, "rhsApparelDefName");
        }

        public override void AddRHS(object rhs) {
            rhsApparelDefName.Add((string)rhs);
        }

        public override void RemoveAtRHS(int index) {
            rhsApparelDefName.RemoveAt(index);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            if (p?.Drawer?.renderer?.graphics?.apparelGraphics != null) {
                foreach (ApparelGraphicRecord record in p.Drawer.renderer.graphics.apparelGraphics) {
                    yield return record.sourceApparel.def.label;
                }
            }
        }
    }
}
