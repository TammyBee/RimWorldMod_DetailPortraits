using DetailPortraits.Data;
using DetailPortraits.Data.DrawingCondition;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DetailPortraits.Dialog {
    [StaticConstructorOnStartup]
    public class Dialog_EditDrawingCondition : Window {
        public static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D Edit = ContentFinder<Texture2D>.Get("UI/Buttons/OpenSpecificTab", true);

        private DrawingConditionData data;

        private string buffer;
        private FloatField floatField;
        private object rhsFieldValue;
        private string rhsBuffer;

        private Vector2 scrollPosition;

        public Dialog_EditDrawingCondition() {
            this.data = new DrawingConditionData();
            Setup();
        }

        public Dialog_EditDrawingCondition(DrawingConditionData data) {
            this.data = data;
            Setup();
        }

        public override Vector2 InitialSize {
            get {
                return new Vector2(600f, 400f);
            }
        }

        public override void PostClose() {
            base.PostClose();
        }

        private void Setup() {
            this.forcePause = true;
            this.doCloseButton = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.soundAppear = SoundDefOf.InfoCard_Open;
            this.soundClose = SoundDefOf.InfoCard_Close;

            this.buffer = "";
            this.floatField = new FloatField(0f);

            this.scrollPosition = Vector2.zero;

            this.rhsBuffer = "";
        }

        public override void DoWindowContents(Rect inRect) {
            Rect rect = new Rect(inRect);
            rect = rect.ContractedBy(18f);
            rect.height = 34f;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, "DetailPortraits.Dialog_EditDrawingConditionTitle".Translate());
            Text.Font = GameFont.Small;

            Rect rect2 = new Rect(inRect);
            rect2.yMin = rect.yMax;
            rect2.yMax -= 38f;

            Widgets.Label(new Rect(rect2.x, rect2.y, 120f, 24), "DetailPortraits.Label_LHS".Translate());
            if (Widgets.ButtonText(new Rect(rect2.x + 120f, rect2.y, 240f, 24), data.lhsPreset.PresetLabel)) {
                List<FloatMenuOption> listFloatMenu = new List<FloatMenuOption>();
                foreach (DrawingConditionTermPreset preset in DrawingConditionTermPresetManager.Presets) {
                    DrawingConditionTermPreset localPreset = preset;
                    listFloatMenu.Add(new FloatMenuOption(localPreset.PresetLabel, delegate {
                        data.lhsPreset = localPreset.Copy;
                        RefreshOperator();
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(listFloatMenu));
            }
            rect2.y += 32f;

            if (!data.lhsPreset.IsBoolPreset) {
                Widgets.Label(new Rect(rect2.x, rect2.y, 120f, 24), "DetailPortraits.Label_Operator".Translate());
                if (Widgets.ButtonText(new Rect(rect2.x + 120f, rect2.y, 240f, 24), data.op.GetLabel())) {
                    List<FloatMenuOption> listFloatMenu = new List<FloatMenuOption>();
                    foreach (DrawingConditionOperator op in data.lhsPreset.AvailableOperators) {
                        DrawingConditionOperator localOperator = op;
                        listFloatMenu.Add(new FloatMenuOption(localOperator.GetLabel(), delegate {
                            data.op = localOperator;
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    Find.WindowStack.Add(new FloatMenu(listFloatMenu));
                }
                rect2.y += 32f;

                if (data.op != DrawingConditionOperator.IsEmpty) {
                    Widgets.Label(new Rect(rect2.x, rect2.y, 120f, 24), "DetailPortraits.Label_RHS".Translate());
                    DrawRHSField(new Rect(rect2.x + 120f, rect2.y, 240f, 24f));
                    if (Widgets.ButtonImage(new Rect(rect2.x + 364f, rect2.y, 24f, 24f), PortraitWidget.Plus)) {
                        data.lhsPreset.AddRHS(rhsFieldValue);
                        RefreshOperator();
                    }

                    rect2.y += 32f;
                    Rect rectRHSs = new Rect(rect2.x + 120f, rect2.y, 268f, 140f);
                    Rect rectViewRHSs = new Rect(rect2.x + 120f, rect2.y, 252f, data.lhsPreset.RHS.Count() * 28f);
                    Widgets.DrawWindowBackground(rectRHSs.ExpandedBy(2f));
                    Widgets.BeginScrollView(rectRHSs, ref scrollPosition, rectViewRHSs, true);
                    float num = 0f;
                    int deleteIndex = -1;
                    for (int i = 0; i < data.lhsPreset.RHS.Count(); i++) {
                        object rhs = data.lhsPreset.RHS.ToList()[i];
                        Rect rectLabel = new Rect(rectRHSs.x + 4f, rectRHSs.y + num, rectViewRHSs.width - 28f, 24f);
                        string label = rhs.ToString();
                        Def def = rhs as Def;
                        if (def != null) {
                            label = def.LabelCap;
                        }
                        Widgets.Label(rectLabel, label);

                        if (Widgets.ButtonInvisible(rectLabel)) {
                            rhsFieldValue = rhs;
                        }
                        if (Widgets.ButtonImage(new Rect(rectRHSs.x + rectViewRHSs.width - 24f, rectRHSs.y + num, 24f, 24f), PortraitWidget.DeleteX)) {
                            SoundDefOf.Click.PlayOneShotOnCamera(null);
                            deleteIndex = i;
                        }
                        num += 28f;
                    }
                    if (deleteIndex != -1) {
                        data.lhsPreset.RemoveAtRHS(deleteIndex);
                        RefreshOperator();
                    }
                    Widgets.EndScrollView();
                    rect2.y += 148f;
                }
            }

            Widgets.Label(new Rect(rect2.x, rect2.y, 120f, 24), "DetailPortraits.Label_Reversed".Translate());
            Widgets.Checkbox(new Vector2(rect2.x + 120f, rect2.y), ref data.isReversed);
        }

        private void DrawRHSField(Rect rect) {
             if (data.lhsPreset.IsCustomType && data.lhsPreset.RHS_Items != null) {
                string str = "";
                if (rhsFieldValue == null || !rhsFieldValue.GetType().Equals(data.lhsPreset.RHS_Type)) {
                    rhsFieldValue = data.lhsPreset.RHS_Items.First();
                }
                str = data.lhsPreset.ConvertObjectToString(rhsFieldValue);
                if (Widgets.ButtonText(rect, str)) {
                    List<FloatMenuOption> listFloatMenu = new List<FloatMenuOption>();
                    foreach (object item in data.lhsPreset.RHS_Items) {
                        object localItem = item;
                        listFloatMenu.Add(new FloatMenuOption(data.lhsPreset.ConvertObjectToString(localItem), delegate {
                            rhsFieldValue = localItem;
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    Find.WindowStack.Add(new FloatMenu(listFloatMenu));
                }
            }else if (data.lhsPreset.RHS_Type == typeof(int)) {
                int val;
                try {
                    val = (int)rhsFieldValue;
                } catch {
                    val = 0;
                }
                Widgets.TextFieldNumeric<int>(rect,ref val, ref buffer);
                rhsFieldValue = val;
            } else if (data.lhsPreset.RHS_Type == typeof(float)) {
                if (floatField.Update(rect)) {
                    rhsFieldValue = floatField.Value;
                }
            } else if (data.lhsPreset.RHS_Type == typeof(string)) {
                rhsBuffer = Widgets.TextField(rect, rhsBuffer);
                rhsFieldValue = rhsBuffer;
            } else if (data.lhsPreset.RHS_Type.IsSubclassOf(typeof(Def)) && data.lhsPreset.Defs != null) {
                string str = "";
                Def def = rhsFieldValue as Def;
                if (def != null) {
                    str = data.lhsPreset.ConvertDefToString(def);
                } else {
                    rhsFieldValue = data.lhsPreset.Defs.First();
                    str = data.lhsPreset.ConvertDefToString(data.lhsPreset.Defs.First());
                }
                if (Widgets.ButtonText(rect, str)) {
                    List<FloatMenuOption> listFloatMenu = new List<FloatMenuOption>();
                    foreach (Def def2 in data.lhsPreset.Defs) {
                        Def localDef = def2;
                        listFloatMenu.Add(new FloatMenuOption(data.lhsPreset.ConvertDefToString(localDef), delegate {
                            rhsFieldValue = localDef;
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    Find.WindowStack.Add(new FloatMenu(listFloatMenu));
                }
            }
        }

        private void RefreshOperator() {
            if (!data.lhsPreset.AvailableOperators.Contains(data.op)) {
                data.op = data.lhsPreset.AvailableOperators.First();
            }
        }
    }
}
