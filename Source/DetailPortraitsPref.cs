using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Verse;

namespace DetailPortraits {
    public static class DetailPortraitsPref {
        public static List<DetailPortraitsPreset> presets = new List<DetailPortraitsPreset>();

        public static readonly string PrefFilePath = Path.Combine(GenFilePaths.ConfigFolderPath, "DetailPortraits.xml");

        public static void LoadPref() {
            if (!File.Exists(PrefFilePath)) {
                Log.Message(PrefFilePath + " is not found.");
                return;
            }

            try {
                Scribe.loader.InitLoading(PrefFilePath);
                try {
                    ScribeMetaHeaderUtility.LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode.None, true);
                    List<DetailPortraitsPreset> p = new List<DetailPortraitsPreset>();
                    Scribe_Collections.Look<DetailPortraitsPreset>(ref p, "presets", LookMode.Deep);
                    presets = p;
                    Scribe.loader.FinalizeLoading();
                } catch {
                    Scribe.ForceStop();
                    throw;
                }
            } catch (Exception ex) {
                Log.Error("Exception loading DetailPortraitsPref: " + ex.ToString());
                presets = new List<DetailPortraitsPreset>();
                Scribe.ForceStop();
            }
        }

        public static void SavePref() {
            try {
                SafeSaver.Save(PrefFilePath, "DetailPortraitsPref", delegate {
                    ScribeMetaHeaderUtility.WriteMetaHeader();
                    List<DetailPortraitsPreset> p = presets;
                    Scribe_Collections.Look<DetailPortraitsPreset>(ref p, "presets", LookMode.Deep);
                });
            } catch (Exception ex) {
                Log.Error("Exception while saving world: " + ex.ToString());
            }
        }
    }
}
