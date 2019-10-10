using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits.Data.DrawingCondition {
    public static class DrawingConditionTermPresetManager {
        private static List<DrawingConditionTermPreset> cachePresets = new List<DrawingConditionTermPreset>();

        public static List<DrawingConditionTermPreset> Presets {
            get {
                return DrawingConditionTermPresetManager.cachePresets;
            }
        }

        static DrawingConditionTermPresetManager() {
            cachePresets = new List<DrawingConditionTermPreset>();
            foreach (Type type in typeof(DrawingConditionTermPreset).AllSubclassesNonAbstract()) {
                DrawingConditionTermPreset item = (DrawingConditionTermPreset)Activator.CreateInstance(type, new object[]{});
                if (!item.Generators.NullOrEmpty()) {
                    foreach (DrawingConditionTermPreset preset in item.Generators) {
                        cachePresets.Add(preset);
                    }
                } else {
                    cachePresets.Add(item);
                }
            }
        }
    }
}
