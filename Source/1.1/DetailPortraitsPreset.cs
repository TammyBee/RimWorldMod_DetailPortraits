using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DetailPortraits.Data;
using RimWorld;
using Verse;

namespace DetailPortraits {
    public class DetailPortraitsPreset : IExposable {
        public string name;
        public PortraitData portraitData;

        public DetailPortraitsPreset() {
            this.name = "";
            this.portraitData = new PortraitData();
        }

        public DetailPortraitsPreset(string name, PortraitData portraitData) {
            this.name = name;
            this.portraitData = portraitData;
        }

        public void ExposeData() {
            Scribe_Values.Look<string>(ref this.name, "name");
            Scribe_Deep.Look<PortraitData>(ref this.portraitData, "portraitData");
        }

        public string GetToolTip() {
            StringBuilder sb = new StringBuilder();
            sb.Append("DetailPortraits.Label_RenderMode".Translate() + ":");
            if (portraitData.renderMode == Data.RenderMode.Default) {
                sb.AppendLine("DetailPortraits.Radio_RenderModeDefault".Translate());
            } else if (portraitData.renderMode == Data.RenderMode.DetailPortrait) {
                sb.AppendLine("DetailPortraits.Radio_RenderModeDetailPortrait".Translate());
            } else if (portraitData.renderMode == Data.RenderMode.Both) {
                sb.AppendLine("DetailPortraits.Radio_RenderModeBoth".Translate());
            }
            sb.AppendLine("DetailPortraits.Label_Position".Translate() + ":" + portraitData.globalPosition);
            sb.AppendLine("DetailPortraits.Label_Scale".Translate() + ":" + portraitData.globalScale);
            sb.AppendLine("DetailPortraits.Label_HideIcon".Translate() + ":" + portraitData.hideIcon);
            sb.AppendLine("DetailPortraits.Label_RefreshTick".Translate() + ":" + portraitData.refreshTick);
            sb.AppendLine("DetailPortraits.Label_Layers".Translate() + ":[" + string.Join(",", portraitData.layers.ConvertAll(l => l.layerName).ToArray()) + "]");
            sb.AppendLine("DetailPortraits.Label_RootPath".Translate() + ":" + portraitData.rootPath);

            return sb.ToString();
        }
    }
}
