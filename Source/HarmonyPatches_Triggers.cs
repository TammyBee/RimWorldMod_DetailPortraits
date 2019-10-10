using System;
using Verse;
using Harmony;
using System.Reflection;
using DetailPortraits.Data;
using RimWorld;
using UnityEngine;
using DetailPortraits.Dialog;
using Verse.AI;
using System.Linq;

namespace DetailPortraits {
    class TriggerDCTP{
        public static Type[] TriggeredByJob = new Type[] {
            typeof(Data.DrawingCondition.DCTP_Job),
            typeof(Data.DrawingCondition.DCTP_ToilIndex),
            typeof(Data.DrawingCondition.DCTP_ToilIndexBack),
            typeof(Data.DrawingCondition.DCTP_IsSleeping)
        };
        public static Type[] TriggeredByToil = new Type[] {
            typeof(Data.DrawingCondition.DCTP_ToilIndex),
            typeof(Data.DrawingCondition.DCTP_ToilIndexBack),
        };
        public static Type[] TriggeredByHediff = new Type[] {
            typeof(Data.DrawingCondition.DCTP_AllHediffs),
            typeof(Data.DrawingCondition.DCTP_VisibleHediffs),
            typeof(Data.DrawingCondition.DCTP_BleedRate),
            typeof(Data.DrawingCondition.DCTP_AllHediffStages)
        };
        public static Type[] TriggeredByThought = new Type[] {
            typeof(Data.DrawingCondition.DCTP_AllThoughts),
            typeof(Data.DrawingCondition.DCTP_AllThoughtStages)
        };

        public static bool TryRefreshRenderableLayers(Pawn p,Type typeDCTP) {
            PortraitData portrait = p.GetPortraitData();
            if (portrait == null) {
                return false;
            }
            if (portrait.HasDCTP(typeDCTP)) {
                portrait.RefreshRenderableLayers();
                return true;
            }
            return false;
        }

        public static bool TryRefreshRenderableLayers(Pawn p, Type[] typesDCTP) {
            PortraitData portrait = p.GetPortraitData();
            if (portrait == null) {
                return false;
            }
            if (typesDCTP.Any(t => portrait.HasDCTP(t))) {
                portrait.RefreshRenderableLayers();
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("StartJob")]
    class Pawn_JobTracker_StartJob_Patch {
        static void Postfix(Pawn_JobTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, TriggerDCTP.TriggeredByJob);
        }
    }

    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("EndCurrentJob")]
    class Pawn_JobTracker_EndCurrentJob_Patch {
        static void Postfix(Pawn_JobTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, TriggerDCTP.TriggeredByJob);
        }
    }

    [HarmonyPatch(typeof(JobDriver))]
    [HarmonyPatch("TryActuallyStartNextToil")]
    class JobDriver_TryActuallyStartNextToil_Patch {
        static void Postfix(JobDriver __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, TriggerDCTP.TriggeredByToil);
        }
    }

    /*
    [HarmonyPatch(typeof(JobDriver))]
    [HarmonyPatch("SetNextToil")]
    class JobDriver_SetNextToil_Patch {
        static void Postfix(JobDriver __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, typeof(Data.DrawingCondition.DCTP_ToilIndex));
        }
    }

    [HarmonyPatch(typeof(Toil))]
    [HarmonyPatch("Cleanup")]
    class Toil_Cleanup_Patch {
        static void Postfix(Toil __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.actor, typeof(Data.DrawingCondition.DCTP_ToilIndex));
        }
    }
    */

    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("JobTrackerTick")]
    class Pawn_JobTracker_JobTrackerTick_Patch {
        private static bool lastAsleep = false;
        static void Prefix(Pawn_JobTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            lastAsleep = (p.CurJob != null && p.jobs.curDriver.asleep);
        }

        static void Postfix(Pawn_JobTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (lastAsleep != (p.CurJob != null && p.jobs.curDriver.asleep)) {
                TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsSleeping));
            }
        }
    }

    [HarmonyPatch(typeof(MemoryThoughtHandler))]
    [HarmonyPatch("TryGainMemory")]
    [HarmonyPatch(new Type[] { typeof(Thought_Memory),typeof(Pawn) })]
    class MemoryThoughtHandler_TryGainMemory_Patch {
        static void Postfix(MemoryThoughtHandler __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, TriggerDCTP.TriggeredByThought);
        }
    }

    [HarmonyPatch(typeof(MemoryThoughtHandler))]
    [HarmonyPatch("RemoveMemory")]
    class MemoryThoughtHandler_RemoveMemory_Patch {
        static void Postfix(MemoryThoughtHandler __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, TriggerDCTP.TriggeredByThought);
        }
    }

    [HarmonyPatch(typeof(SituationalThoughtHandler))]
    [HarmonyPatch("TryCreateThought")]
    class SituationalThoughtHandler_TryCreateThought_Patch {
        static void Postfix(SituationalThoughtHandler __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, TriggerDCTP.TriggeredByThought);
        }
    }

    [HarmonyPatch(typeof(Need_Mood))]
    [HarmonyPatch("NeedInterval")]
    class ThoughtHandler_GetAllMoodThoughts_Patch {
        static void Postfix(Need_Mood __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.thoughts.pawn, TriggerDCTP.TriggeredByThought);
        }
    }

    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("DirtyCache")]
    class HediffSet_DirtyCache_Patch {
        static void Postfix(HediffSet __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, TriggerDCTP.TriggeredByHediff);
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeDowned")]
    class Pawn_HealthTracker_MakeDowned_Patch {
        static void Postfix(Pawn_HealthTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsDowned));
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeUndowned")]
    class Pawn_HealthTracker_MakeUndowned_Patch {
        static void Postfix(Pawn_HealthTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsDowned));
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("Notify_Resurrected")]
    class Pawn_HealthTracker_Notify_Resurrected_Patch {
        static void Postfix(Pawn_HealthTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsDead));
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("Reset")]
    class Pawn_HealthTracker_Reset_Patch {
        static void Postfix(Pawn_HealthTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsDead));
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("SetDead")]
    class Pawn_HealthTracker_SetDead_Patch {
        static void Postfix(Pawn_HealthTracker __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_IsDead));
        }
    }

    [HarmonyPatch(typeof(Pawn_DraftController))]
    [HarmonyPatch("Drafted", MethodType.Setter)]
    class Pawn_DraftController_Drafted_Setter_Patch {
        static void Postfix(Pawn_DraftController __instance) {
            TriggerDCTP.TryRefreshRenderableLayers(__instance.pawn, typeof(Data.DrawingCondition.DCTP_IsDrafted));
        }
    }

    [HarmonyPatch(typeof(MentalStateHandler))]
    [HarmonyPatch("ClearMentalStateDirect")]
    class MentalStateHandler_ClearMentalStateDirect_Patch {
        static void Postfix(MentalStateHandler __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_MentalState));
        }
    }

    [HarmonyPatch(typeof(MentalStateHandler))]
    [HarmonyPatch("TryStartMentalState")]
    class MentalStateHandler_TryStartMentalState_Patch {
        static void Postfix(MentalStateHandler __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_MentalState));
        }
    }

    [HarmonyPatch(typeof(MentalState))]
    [HarmonyPatch("RecoverFromState")]
    class MentalState_RecoverFromState_Patch {
        static void Postfix(MentalStateHandler __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_MentalState));
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet))]
    [HarmonyPatch("ResolveApparelGraphics")]
    class PawnGraphicSet_ResolveApparelGraphics_Patch {
        static void Postfix(MentalStateHandler __instance) {
            Pawn p = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            TriggerDCTP.TryRefreshRenderableLayers(p, typeof(Data.DrawingCondition.DCTP_AllApparels));
        }
    }
}
