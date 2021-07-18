using DetailPortraits.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits {
    public class GameComponent_DetailPortraits : GameComponent {
        public Dictionary<Thing, PortraitData> portraits;

        private List<Thing> tmpPawns;

        private List<PortraitData> tmpPortraits;

        public GameComponent_DetailPortraits(Game game) {
            
        }

        public override void StartedNewGame() {
            Refresh();
        }

        public override void LoadedGame() {
            Refresh();
        }

        public override void GameComponentTick() {
            foreach (PortraitData p in portraits.Values) {
                p.Tick();
            }
        }

        public bool CanRefresh() {
            if (portraits == null || portraits.Keys.Count == 0) {
                return true;
            }
            List<int> ids = Utils.GetAllColonists().ConvertAll(p => p.thingIDNumber);
            List<int> ids2 = portraits.Keys.ToList().ConvertAll(p => p.thingIDNumber);
            if (ids.Count == ids2.Count && ids.Count + ids2.Count > 0 && ids.All(id => ids2.Contains(id)) && ids2.All(id => ids.Contains(id))) {
                return false;
            }
            return true;
        }

        public void Refresh() {
            if (this.portraits == null) {
                this.portraits = new Dictionary<Thing, PortraitData>();
            }
            foreach(Pawn p in Utils.GetAllColonists()) {
                if (p != null && !this.portraits.ContainsKey(p)) {
                    this.portraits[p] = new PortraitData(p);
                }else if (p != null && this.portraits.ContainsKey(p)) {
                    this.portraits[p].RefreshRenderableLayers(false);
                }
            }
            //Log.Message("Refresh");
        }

        public override void ExposeData() {
            if (Scribe.mode == LoadSaveMode.Saving) {
                List<Pawn> deadColonists = new List<Pawn>();
                foreach (Thing t in portraits.Keys) {
                    if (t != null) {
                        Pawn p = t as Pawn;
                        if (p != null && p.Dead && p.Corpse != null) {
                            deadColonists.Add(p);
                        }
                    }
                }
                foreach (Pawn p in deadColonists) {
                    portraits[p.Corpse] = new PortraitData(portraits[p],null);
                    //Log.Message("Set Corpse Key:" + p.ToStringSafe());
                    portraits.Remove(p);
                }
                Scribe_Collections.Look(ref portraits, "portraits", LookMode.Reference, LookMode.Deep, ref tmpPawns, ref tmpPortraits);
                foreach (Pawn p in deadColonists) {
                    Corpse c = p.Corpse;
                    if (c != null) {
                        portraits[c.InnerPawn] = new PortraitData(portraits[c], c.InnerPawn);
                        //Log.Message("Set Colonist Key:" + c.InnerPawn.ToStringSafe());
                        portraits.Remove(c);
                    }
                }
            } else {
                Scribe_Collections.Look(ref portraits, "portraits", LookMode.Reference, LookMode.Deep, ref tmpPawns, ref tmpPortraits);
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                List<Corpse> corpse = new List<Corpse>();
                foreach (Thing t in portraits.Keys) {
                    if (t != null) {
                        Corpse c = t as Corpse;
                        if (c != null) {
                            //Log.Message("Corpse:" + c.Label);
                            if (c.InnerPawn != null) {
                                corpse.Add(c);
                            }
                        }
                    }
                }
                foreach (Corpse c in corpse) {
                    portraits[c.InnerPawn] = new PortraitData(portraits[c], c.InnerPawn);
                    //Log.Message("Set Colonist Key:" + c.InnerPawn.ToStringSafe());
                    portraits.Remove(c);
                }
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                Refresh();
            }
        }
    }
}
