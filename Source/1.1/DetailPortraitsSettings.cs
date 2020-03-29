using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DetailPortraits {
    public class DetailPortraitsSettings : ModSettings {
        public bool normalizeScale = true;
        public bool dctpGrouped = true;

        public override void ExposeData() {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref this.normalizeScale, "normalizeScale", true);
            Scribe_Values.Look<bool>(ref this.dctpGrouped, "dctpGrouped", true);
        }
    }
}
