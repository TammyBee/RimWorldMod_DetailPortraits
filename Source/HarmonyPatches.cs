using System;
using Verse;
using Harmony;
using System.Reflection;
using DetailPortraits.Data;
using RimWorld;
using UnityEngine;
using DetailPortraits.Dialog;
using Verse.AI;

namespace DetailPortraits {
    [StaticConstructorOnStartup]
    class HarmonyPatches {
        static HarmonyPatches() {
            var harmony = HarmonyInstance.Create("com.tammybee.detailportraits");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            DetailPortraitsPref.LoadPref();
        }
    }

    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(MainTabWindow_Inspect))]
    [HarmonyPatch("DoInspectPaneButtons")]
    class MainTabWindow_Inspect_DoInspectPaneButtons_Patch {
        public static readonly Texture2D EditPortrait = ContentFinder<Texture2D>.Get("UI/Buttons/EditPortrait", true);

        static void Prefix(MainTabWindow_Inspect __instance, Rect rect, ref float lineEndWidth) {
            if (Find.Selector.NumSelected == 1) {
                Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
                if (singleSelectedThing != null) {
                    Pawn pawn = singleSelectedThing as Pawn;
                    if (pawn != null && pawn.IsColonist) {
                        EditPortraitButton(rect.width - 96f, 0f, pawn);
                        lineEndWidth += 24f;
                    } else {
                        Corpse corpse = singleSelectedThing as Corpse;
                        if (corpse?.InnerPawn != null && corpse.InnerPawn.IsColonist) {
                            EditPortraitButton(rect.width - 96f, 0f, corpse.InnerPawn);
                            lineEndWidth += 24f;
                        }
                    }
                }
            }
        }

        private static void EditPortraitButton(float x,float y,Pawn p) {
            Rect rect = new Rect(x, y, 24f, 24f);
            if (Widgets.ButtonImage(rect, EditPortrait, GUI.color)) {
                Find.WindowStack.Add(new Dialog_EditPortrait(p));
            }
            UIHighlighter.HighlightOpportunity(rect, "EditPortrait");
            TooltipHandler.TipRegion(rect, "DetailPortraits.EditPortraitButton".Translate());
        }

    }

    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("RenderPortrait")]
    class PawnRenderer_RenderPortrait_Patch {
        static bool Prefix(PawnRenderer __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            PortraitData portrait = p.GetPortraitData();
            if (portrait != null && portrait.renderMode == Data.RenderMode.DetailPortrait) {
                portrait.Render();
                return false;
            }
            return true;
        }

        static void Postfix(PawnRenderer __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            PortraitData portrait = p.GetPortraitData();
            if (portrait != null && portrait.renderMode == Data.RenderMode.Both) {
                portrait.Render();
            }
        }
    }

    [HarmonyPatch(typeof(ColonistBar))]
    [HarmonyPatch("CheckRecacheEntries")]
    class ColonistBar_CheckRecacheEntries_Patch {
        static bool lastEntriesDirty = false;

        static void Prefix(ColonistBar __instance) {
            lastEntriesDirty = Traverse.Create<ColonistBar>().Field("entriesDirty").GetValue<bool>();
        }

        static void Postfix() {
            if (lastEntriesDirty) {
                GameComponent_DetailPortraits comp = Current.Game.GetComponent<GameComponent_DetailPortraits>();
                if (comp != null) {
                    comp.Refresh();
                }
            }
        }
    }

    [HarmonyPatch(typeof(ColonistBarColonistDrawer))]
    [HarmonyPatch("DrawIcons")]
    class ColonistBarColonistDrawer_DrawIcons_Patch {
        static bool Prefix(Pawn colonist) {
            GameComponent_DetailPortraits comp = Current.Game.GetComponent<GameComponent_DetailPortraits>();
            return comp == null || !comp.portraits.ContainsKey(colonist) || comp.portraits[colonist].renderMode == Data.RenderMode.Default || !comp.portraits[colonist].hideIcon;
        }
    }
}
