using DetailPortraits.Data;
using DetailPortraits.Data.DrawingCondition;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits {
    public static class Utils {
        public static List<Pawn> GetAllColonists() {
            if ((Find.UIRoot as UIRoot_Play) == null || Find.MapUI == null || Find.World == null || Find.WorldObjects == null || Find.ColonistBar?.Entries == null) {
                List<Pawn> pawns = new List<Pawn>();
                if (Current.Game?.Maps != null) {
                    foreach (Map map in Current.Game.Maps) {
                        pawns.AddRange(map.mapPawns.FreeColonists);
                        List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
                        for (int j = 0; j < list.Count; j++) {
                            if (!list[j].IsDessicated()) {
                                Pawn innerPawn = ((Corpse)list[j]).InnerPawn;
                                if (innerPawn != null) {
                                    if (innerPawn.IsColonist) {
                                        pawns.Add(innerPawn);
                                    }
                                }
                            }
                        }
                        List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
                        for (int k = 0; k < allPawnsSpawned.Count; k++) {
                            Corpse corpse = allPawnsSpawned[k].carryTracker.CarriedThing as Corpse;
                            if (corpse != null && !corpse.IsDessicated() && corpse.InnerPawn.IsColonist) {
                                pawns.Add(corpse.InnerPawn);
                            }
                        }
                    }
                }
                if (Find.World != null && Find.WorldObjects != null) {
                    foreach (Caravan caravan in Find.WorldObjects.Caravans) {
                        if (caravan.IsPlayerControlled) {
                            foreach (Pawn p in caravan.pawns) {
                                if (p.IsColonist) {
                                    pawns.Add(p);
                                }
                            }
                        }
                    }
                }
                return pawns;
            }
            return Find.ColonistBar.Entries.ConvertAll(e => e.pawn);
        }


        public static List<DrawingConditionOperator> AvailableOperators(Type lhs_type, Type rhs_type, bool isListLHS, IEnumerable<object> rhs, bool allowEmpty) {
            List<DrawingConditionOperator> operators = new List<DrawingConditionOperator>();
            Type lhs_t = lhs_type;
            Type rhs_t = rhs_type;
            if (rhs != null && rhs.Any() && rhs.First() != null) {
                rhs_t = rhs.First().GetType();
            }

            //Log.Message(lhs_type+","+rhs_type);
            bool IsNumeric(Type t) => (t == typeof(float) || t == typeof(int));

            if (!isListLHS && rhs.Count() <= 1) {
                operators.Add(DrawingConditionOperator.Equal);
            }
            if (IsNumeric(lhs_t) && IsNumeric(rhs_t) && rhs.Count() <= 1) {
                operators.Add(DrawingConditionOperator.GT);
                operators.Add(DrawingConditionOperator.GTE);
                operators.Add(DrawingConditionOperator.LT);
                operators.Add(DrawingConditionOperator.LTE);
            }
            if (!isListLHS && rhs.Count() >= 2) {
                operators.Add(DrawingConditionOperator.In);
            }
            if (isListLHS) {
                operators.Add(DrawingConditionOperator.Contains);
            }
            if (isListLHS && rhs.Count() >= 2) {
                operators.Add(DrawingConditionOperator.Shared);
            }
            if (allowEmpty) {
                operators.Add(DrawingConditionOperator.IsEmpty);
            }

            return operators;
        }

        public static string GetLabel(this DrawingConditionOperator op) {
            if (op == DrawingConditionOperator.Equal) {
                return "=";
            }else if (op == DrawingConditionOperator.GT) {
                return ">";
            } else if (op == DrawingConditionOperator.GTE) {
                return ">=";
            } else if (op == DrawingConditionOperator.LT) {
                return "<";
            } else if (op == DrawingConditionOperator.LTE) {
                return "<=";
            } else if (op == DrawingConditionOperator.In) {
                return "in";
            } else if (op == DrawingConditionOperator.Contains) {
                return "contains";
            } else if (op == DrawingConditionOperator.Shared) {
                return "shared";
            } else if (op == DrawingConditionOperator.IsEmpty) {
                return "is empty";
            }
            return "";
        }

        public static PortraitData GetPortraitData(this Pawn pawn) {
            if (Current.Game == null) {
                return null;
            }
            GameComponent_DetailPortraits comp = Current.Game.GetComponent<GameComponent_DetailPortraits>();
            if (comp?.portraits != null && comp.portraits.ContainsKey(pawn)) {
                Data.RenderMode renderMode = comp.portraits[pawn].renderMode;
                if (renderMode != Data.RenderMode.Default) {
                    return comp.portraits[pawn];
                }
            }
            return null;
        }

        public static bool SetPortraitData(this Pawn pawn, PortraitData portrait) {
            if (Current.Game == null) {
                return false;
            }
            GameComponent_DetailPortraits comp = Current.Game.GetComponent<GameComponent_DetailPortraits>();
            if (comp?.portraits != null && comp.portraits.ContainsKey(pawn)) {
                comp.portraits[pawn] = new PortraitData(portrait,pawn);
                comp.portraits[pawn].RefreshRenderableLayers(true);
                return true;
            }
            return false;
        }

        public static bool CanRenderPortraitIcon(this Pawn colonist) {
            GameComponent_DetailPortraits comp = Current.Game.GetComponent<GameComponent_DetailPortraits>();
            return comp == null || !comp.portraits.ContainsKey(colonist) || comp.portraits[colonist].renderMode == Data.RenderMode.Default || !comp.portraits[colonist].hideIcon;
        }
    }
}
