using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public abstract class DrawingConditionTermPreset : IExposable {
        public virtual bool IsBoolPreset {
            get {
                return false;
            }
        }

        public virtual bool AllowEmpty {
            get {
                return false;
            }
        }

        public virtual List<DrawingConditionTermPreset> Generators {
            get {
                return null;
            }
        }

        public virtual string GroupStringKey {
            get {
                return null;
            }
        }

        public abstract Type LHS_Type { get; }

        public virtual bool IsListLHS {
            get {
                return false;
            }
        }

        public abstract Type RHS_Type { get; }

        public abstract IEnumerable<object> RHS { get; }

        public abstract string PresetLabel { get; }

        public virtual bool IsCustomType {
            get {
                return false;
            }
        }

        public bool IsAvailable {
            get {
                return RHS.Any();
            }
        }

        public virtual string RHSLabel {
            get {
                List<object> rhs = RHS.ToList();
                if (rhs == null) {
                    return RHS.ToString();
                }
                string s = string.Join(",", rhs.ConvertAll(obj => ConvertObjectToString(obj)).ToArray());
                if (rhs.Count >= 2) {
                    return "[" + s + "]";
                }
                return s;
            }
        }

        public abstract DrawingConditionTermPreset Copy { get; }

        public void ExposeData() {
            ExposeDataInternal();
        }

        public abstract void ExposeDataInternal();

        public abstract IEnumerable<object> GetValue(Pawn p);

        public List<DrawingConditionOperator> AvailableOperators {
            get {
                return Utils.AvailableOperators(this.LHS_Type, this.RHS_Type, this.IsListLHS, this.RHS, this.AllowEmpty);
            }
        }

        public virtual IEnumerable<Def> Defs {
            get {
                return null;
            }
        }

        public virtual IEnumerable<object> RHS_Items {
            get {
                return null;
            }
        }

        // AddRHSとGetValueのobjectの型は同じである必要がある
        public abstract void AddRHS(object rhs);

        public abstract void RemoveAtRHS(int index);

        public virtual string ConvertDefToString(Def def) {
            return def.defName;
        }

        public virtual string ConvertObjectToString(Object obj) {
            if (obj == null) {
                return "";
            }
            Def def = obj as Def;
            if (def != null) {
                return def.defName;
            }
            return obj.ToString();
        }
    }
}
