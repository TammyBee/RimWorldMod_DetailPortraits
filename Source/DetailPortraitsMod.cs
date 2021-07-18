using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DetailPortraits {
    public class DetailPortraitsMod : Mod {
        public static DetailPortraitsSettings Settings {
            get {
                return LoadedModManager.GetMod<DetailPortraitsMod>().settings;
            }
        }

        public DetailPortraitsSettings settings;

        public DetailPortraitsMod(ModContentPack content) : base(content) {
            this.settings = GetSettings<DetailPortraitsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = (float)((inRect.width - 34.0) / 2.0);
            listing_Standard.Begin(inRect);
            bool oldNormalizeScale = Settings.normalizeScale;
            listing_Standard.CheckboxLabeled("DetailPortraits.NormalizeScale".Translate(),ref Settings.normalizeScale);
            if (oldNormalizeScale != Settings.normalizeScale) {
                GameComponent_DetailPortraits comp = Current.Game?.GetComponent<GameComponent_DetailPortraits>();
                if (comp != null) {
                    comp.Refresh();
                }
            }
            listing_Standard.CheckboxLabeled("DetailPortraits.DCTPGrouped".Translate(), ref Settings.dctpGrouped);
            listing_Standard.End();
        }

        public override string SettingsCategory() {
            return "Detail Portraits";
        }
    }
}
