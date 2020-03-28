using System;
using Verse;
using HarmonyLib;
using System.Reflection;
using DetailPortraits.Data;
using RimWorld;
using UnityEngine;
using DetailPortraits.Dialog;
using Verse.AI;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace DetailPortraits {
    [StaticConstructorOnStartup]
    class HarmonyPatches {
        static HarmonyPatches() {
            var harmony = new Harmony("com.tammybee.detailportraits");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MethodInfo origDrawColonist = typeof(ColonistBarColonistDrawer).GetMethod("DrawColonist", AccessTools.all);
            MethodInfo transDrawColonist = AccessTools.Method(typeof(ColonistBarColonistDrawer_DrawColonist_Patch), "Transpiler");
            harmony.Patch(origDrawColonist, null, null, new HarmonyMethod(transDrawColonist));

            DetailPortraitsPref.LoadPref();
        }
    }

    class For_Debug {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> cis = new List<CodeInstruction>(instructions);
            PrintCodeInstraction("[Debug]\n", cis);

            foreach (CodeInstruction ci in cis) {
                yield return ci;
            }
        }

        public static void PrintCodeInstraction(string header, List<CodeInstruction> cis) {
            StringBuilder sb = new StringBuilder(header);
            foreach (CodeInstruction ci in cis) {
                sb.Append(ci.ToString() + "\n");
            }
            Log.Message(sb.ToString());
        }
    }

    class ColonistBarColonistDrawer_DrawColonist_Patch {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> cis = new List<CodeInstruction>(instructions);
            //For_Debug.PrintCodeInstraction("[Before]", cis);

            int insertPos = cis.FindIndex(c => (c.opcode == OpCodes.Brfalse_S && c.operand != null && c.operand.GetType() == typeof(Label) && $"Label{((Label)c.operand).GetHashCode()}" == "Label14"));
            List<CodeInstruction> injections = new List<CodeInstruction> {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ColonistBarColonistDrawer_DrawColonist_Patch), "CanRenderDeadMark")),
                new CodeInstruction(cis[insertPos])
            };
            cis.InsertRange(insertPos + 1, injections);
             
            foreach (CodeInstruction ci in cis) {
                yield return ci;
            }
            //For_Debug.PrintCodeInstraction("[After]", cis);
        }

        private static bool CanRenderDeadMark(Pawn p) {
            return p.CanRenderPortraitIcon();
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
                    float x = 72f;
                    if (pawn != null && pawn.playerSettings != null && pawn.playerSettings.UsesConfigurableHostilityResponse) {
                        x = 96f;
                    }
                    if (pawn != null && pawn.IsColonist) {
                        EditPortraitButton(rect.width - x, 0f, pawn);
                        lineEndWidth += 24f;
                    } else {
                        Corpse corpse = singleSelectedThing as Corpse;
                        if (corpse?.InnerPawn != null && corpse.InnerPawn.IsColonist) {
                            EditPortraitButton(rect.width - x, 0f, corpse.InnerPawn);
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
            return colonist.CanRenderPortraitIcon();
        }
    }
}
