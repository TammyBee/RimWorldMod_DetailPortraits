using DetailPortraits.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DetailPortraits {
    public class GameComponent_DetailPortraits : GameComponent {
        public Dictionary<Pawn, PortraitData> portraits;

        private List<Pawn> tmpPawns;

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
                this.portraits = new Dictionary<Pawn, PortraitData>();
            }
            foreach(Pawn p in Utils.GetAllColonists()) {
                if (p != null && !this.portraits.ContainsKey(p)) {
                    this.portraits[p] = new PortraitData(p);
                }else if (p != null && this.portraits.ContainsKey(p)) {
                    this.portraits[p].RefreshRenderableLayers();
                }
            }
            //Log.Message("Refresh");
        }

        public override void ExposeData() {
            Scribe_Collections.Look(ref portraits, "portraits", LookMode.Reference,LookMode.Deep,ref tmpPawns,ref tmpPortraits);
            if (Scribe.mode == LoadSaveMode.PostLoadInit) {
                Refresh();
            }
        }
    }
}
