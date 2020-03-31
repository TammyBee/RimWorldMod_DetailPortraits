using DetailPortraits.Dialog;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public class DCTP_MTB : DCTP_Base_Bool {
        public float day = 1f;

        private FloatField floatFieldDay;

        public override string PresetLabel {
            get {
                return "DetailPortraits.DCTP_MTB_Label".Translate();
            }
        }

        public override string PresetLabelInLayerList {
            get {
                return "DetailPortraits.DCTP_MTB_LabelInLayerList".Translate(this.day);
            }
        }

        public override DrawingConditionTermPreset Copy {
            get {
                return new DCTP_MTB(this.day);
            }
        }

        public override int ArgumentsPanelHeight {
            get {
                return 32;
            }
        }

        public DCTP_MTB() {
            this.floatFieldDay = new FloatField(1f);
        }

        public DCTP_MTB(float day) {
            this.day = day;
            this.floatFieldDay = new FloatField(day);
        }

        public override IEnumerable<object> GetValue(Pawn p) {
            int checkDuration = Find.TickManager.TicksGame - p.GetPortraitData().lastRefreshTick;
            if (checkDuration <= 0) {
                yield return false;
            } else {
                yield return Rand.MTBEventOccurs(day, 60000f, checkDuration);
            }
        }

        public override void ArgumentsPanel(Rect rect) {
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, ArgumentsPanelHeight);
            Widgets.Label(new Rect(rect2.x, rect2.y, 120f, rect2.y), "DetailPortraits.DCTP_MTB_FloatFieldDay".Translate());
            if (floatFieldDay.Update(new Rect(rect2.x + 120f, rect2.y, 100f, 24f))) {
                this.day = floatFieldDay.Value;
            }
        }

        public override void ExposeDataInternal() {
            Scribe_Values.Look(ref this.day, "mtbDays", 1f);
        }
    }
}
