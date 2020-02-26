using RimWorld;
using System.Collections.Generic;
using Verse;

namespace DetailPortraits {
    public class DetailPortraitsPrefData : IExposable {
        public List<DetailPortraitsPreset> presets = new List<DetailPortraitsPreset>();

        public void ExposeData() {
            Scribe_Collections.Look<DetailPortraitsPreset>(ref this.presets, "presets", LookMode.Deep);
        }
    }
}
