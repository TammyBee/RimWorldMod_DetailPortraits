using DetailPortraits.Data;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DetailPortraits.Dialog {
    [StaticConstructorOnStartup]
    public class Dialog_EditPortrait : Window {
        public static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus", true);
        public static readonly Texture2D Copy = ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true);

        private Pawn pawn;

        private FloatField floatFieldPositionX;
        private FloatField floatFieldPositionY;
        private FloatField floatFieldScale;
        private FloatField floatFieldScaleH;

        private Vector2 scrollPosition;
        private LayerData selectedLayer = null;

        private string bufferRefreshTick;

        public PortraitData PortraitData {
            get {
                Dictionary<Thing, PortraitData> dict = Current.Game.GetComponent<GameComponent_DetailPortraits>().portraits;
                if (!dict.ContainsKey(pawn)) {
                    dict[pawn] = new PortraitData(pawn);
                }
                return dict[pawn];
            }
        }

        public Dialog_EditPortrait(Pawn pawn) {
            this.pawn = pawn;
            Setup();
        }

        public override Vector2 InitialSize {
            get {
                return new Vector2(950f, 462f + LayerEditorHeight);
            }
        }

        private float LayerEditorHeight {
            get {
                return PortraitWidget.LayerPanelHeight * 7 + 28f;
            }
        }

        public void Setup() {
            this.forcePause = true;
            this.doCloseButton = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.soundAppear = SoundDefOf.InfoCard_Open;
            this.soundClose = SoundDefOf.InfoCard_Close;

            this.floatFieldPositionX = new FloatField(PortraitData.globalPosition.x);
            this.floatFieldPositionY = new FloatField(PortraitData.globalPosition.y);
            this.floatFieldScale = new FloatField(PortraitData.globalScale);
            this.floatFieldScaleH = new FloatField(PortraitData.globalScaleH);

            this.bufferRefreshTick = PortraitData.refreshTick.ToString();

            this.scrollPosition = Vector2.zero;
        }

        private string GetTitle() {
            return "DetailPortraits.Dialog_EditPortraitTitle".Translate(pawn.Name.ToStringShort);
        }

        public override void DoWindowContents(Rect inRect) {
            Rect rect = new Rect(inRect);
            rect = rect.ContractedBy(18f);
            rect.height = 34f;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, this.GetTitle());
            Text.Font = GameFont.Small;

            Rect rect2 = new Rect(inRect);
            rect2.yMin = rect.yMax;
            rect2.yMax -= 38f;

            if (Widgets.ButtonText(new Rect(rect2.x + 500f, rect2.y, 140f, 28f), "DetailPortraits.OpenPresetListDialog".Translate())) {
                Find.WindowStack.Add(new Dialog_PresetList(PortraitData,this));
            }

            TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, 200f, 24), "DetailPortraits.Tooltip_Desc_RenderMode".Translate());
            Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24), "DetailPortraits.Label_RenderMode".Translate());
            if (Widgets.RadioButtonLabeled(new Rect(rect2.x + 200f, rect2.y, 240f, 24), "DetailPortraits.Radio_RenderModeDefault".Translate(), PortraitData.renderMode == Data.RenderMode.Default)) {
                PortraitData.renderMode = Data.RenderMode.Default;
            }
            rect2.y += 28f;
            if (Widgets.RadioButtonLabeled(new Rect(rect2.x + 200f, rect2.y, 240f, 24), "DetailPortraits.Radio_RenderModeDetailPortrait".Translate(), PortraitData.renderMode == Data.RenderMode.DetailPortrait)) {
                PortraitData.renderMode = Data.RenderMode.DetailPortrait;
            }
            rect2.y += 28f;
            if (Widgets.RadioButtonLabeled(new Rect(rect2.x + 200f, rect2.y, 240f, 24), "DetailPortraits.Radio_RenderModeBoth".Translate(), PortraitData.renderMode == Data.RenderMode.Both)) {
                PortraitData.renderMode = Data.RenderMode.Both;
            }
            rect2.y += 32f;

            if (PortraitData.renderMode != Data.RenderMode.Default) {
                Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Label_Position".Translate());
                if (floatFieldPositionX.Update(new Rect(rect2.x + 200f, rect2.y, 100f, 24f))) {
                    if (PortraitData.globalPosition.x != floatFieldPositionX.Value) {
                        PortraitData.globalPosition.x = floatFieldPositionX.Value;
                        this.PortraitData.RefreshRenderableLayers();
                    }
                }
                if (floatFieldPositionY.Update(new Rect(rect2.x + 310f, rect2.y, 100f, 24f))) {
                    if (PortraitData.globalPosition.y != floatFieldPositionY.Value) {
                        PortraitData.globalPosition.y = floatFieldPositionY.Value;
                        this.PortraitData.RefreshRenderableLayers();
                    }
                }
                rect2.y += 32f;

                Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Label_Scale".Translate());
                if (floatFieldScale.Update(new Rect(rect2.x + 200f, rect2.y, 100f, 24f))) {
                    if (PortraitData.globalScale != floatFieldScale.Value) {
                        PortraitData.globalScale = floatFieldScale.Value;
                        this.PortraitData.RefreshRenderableLayers();
                    }
                }
                if (floatFieldScaleH.Update(new Rect(rect2.x + 310f, rect2.y, 100f, 24f))) {
                    if (PortraitData.globalScaleH != floatFieldScaleH.Value) {
                        PortraitData.globalScaleH = floatFieldScaleH.Value;
                        this.PortraitData.RefreshRenderableLayers();
                    }
                }
                rect2.y += 32f;

                TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Tooltip_Desc_HideIcon".Translate());
                Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Label_HideIcon".Translate());
                Widgets.Checkbox(new Vector2(rect2.x + 200f, rect2.y), ref PortraitData.hideIcon);
                rect2.y += 32f;

                TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Tooltip_Desc_RefreshTick".Translate());
                Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Label_RefreshTick".Translate());
                Widgets.TextFieldNumeric(new Rect(rect2.x + 200f, rect2.y, 100f, 24f), ref PortraitData.refreshTick, ref bufferRefreshTick);
                rect2.y += 32f;

                TooltipHandler.TipRegion(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Tooltip_Desc_RootPath".Translate());
                Widgets.Label(new Rect(rect2.x, rect2.y, 200f, 24f), "DetailPortraits.Label_RootPath".Translate());
                this.PortraitData.rootPath = Widgets.TextField(new Rect(rect2.x + 200f, rect2.y, 300f, 24f), this.PortraitData.rootPath);
                rect2.y += 32f;

                Widgets.DrawLineHorizontal(0f, rect2.y, rect2.width);
                rect2.y += 4f;
                Widgets.Label(new Rect(rect2.x, rect2.y, 320f, 24f), "DetailPortraits.Label_Layers".Translate());
                rect2.y += 30f;

                if (Widgets.ButtonImage(new Rect(rect2.x, rect2.y, 24f, 24f), Dialog_EditPortrait.Plus)) {
                    if (this.selectedLayer != null) {
                        int selectedIndex = PortraitData.layers.IndexOf(this.selectedLayer);
                        if (selectedIndex != -1 && selectedIndex + 1 <= PortraitData.layers.Count) {
                            PortraitData.layers.Insert(selectedIndex + 1, new LayerData(this.PortraitData));
                        } else {
                            PortraitData.layers.Add(new LayerData(this.PortraitData));
                        }
                    } else {
                        PortraitData.layers.Add(new LayerData(this.PortraitData));
                    }
                }

                if (this.selectedLayer != null && Widgets.ButtonImage(new Rect(rect2.x + 24f, rect2.y, 24f, 24f), Dialog_EditPortrait.Copy)) {
                    PortraitData.layers.Add(new LayerData(this.selectedLayer));
                }
                rect2.y += 32f;

                Rect viewRect = new Rect(2f, rect2.y, rect2.width / 3, PortraitData.layers.Count * PortraitWidget.LayerPanelHeight);
                Rect layerListRect = new Rect(rect2.x + 3f, rect2.y, viewRect.width + 16f, LayerEditorHeight);
                DrawLayerList(layerListRect, viewRect);

                Rect layerEditorRect = new Rect(12f + layerListRect.width, rect2.y, rect2.width - layerListRect.width - 30f, layerListRect.height);
                if (selectedLayer != null) {
                    PortraitWidget.DrawLayerEditor(layerEditorRect, ref selectedLayer);
                } else {
                    Widgets.DrawWindowBackground(layerEditorRect.ExpandedBy(2f));
                }
            }

            //RenderDebug();
        }

        private void RenderDebug() {
            foreach (LayerData layer in PortraitData.RenderableLayers) {
                Graphic graphic = layer.textureData.GetGraphic(PortraitData.globalScale * layer.localScale, PortraitData.globalScaleH * layer.localScaleH, this.PortraitData.rootPath);
                Texture tex = graphic?.MatSingle?.mainTexture;
                Widgets.DrawTextureFitted(new Rect(layer.localPosition + PortraitData.globalPosition, new Vector2(tex.width,tex.height)),tex,1f);
            }
            //PortraitData.Render();
        }

        private void DrawLayerList(Rect rect,Rect viewRect) {
            Widgets.DrawWindowBackground(rect.ExpandedBy(2f));
            Widgets.BeginScrollView(rect, ref scrollPosition, viewRect, true);
            float num = 0f;
            int reorderUp = -1;
            int reorderDown = -1;
            int delete = -1;
            for (int i = 0;i < PortraitData.layers.Count;i++) {
                LayerData layerData = PortraitData.layers[i];
                Rect rect2 = new Rect(rect.x, rect.y + num, viewRect.width, PortraitWidget.LayerPanelHeight);
                PortraitWidget.LayerPanelEvent layerPanelEvent = PortraitWidget.DrawLayerPanel(rect2, layerData, ref num,i == 0,i == PortraitData.layers.Count - 1, layerData == selectedLayer);
                if(layerPanelEvent == PortraitWidget.LayerPanelEvent.Select) {
                    selectedLayer = layerData;
                }else if (layerPanelEvent == PortraitWidget.LayerPanelEvent.ReorderUp) {
                    reorderUp = i;
                } else if (layerPanelEvent == PortraitWidget.LayerPanelEvent.ReorderDown) {
                    reorderDown = i;
                } else if (layerPanelEvent == PortraitWidget.LayerPanelEvent.Delete) {
                    delete = i;
                    if (layerData == selectedLayer) {
                        selectedLayer = null;
                    }
                }
            }
            if (reorderUp != -1) {
                LayerData tmp = PortraitData.layers[reorderUp - 1];
                PortraitData.layers[reorderUp - 1] = PortraitData.layers[reorderUp];
                PortraitData.layers[reorderUp] = tmp;
                this.PortraitData.RefreshRenderableLayers();
            } else if (reorderDown != -1) {
                LayerData tmp = PortraitData.layers[reorderDown + 1];
                PortraitData.layers[reorderDown + 1] = PortraitData.layers[reorderDown];
                PortraitData.layers[reorderDown] = tmp;
                this.PortraitData.RefreshRenderableLayers();
            } else if (delete != -1) {
                PortraitData.layers.RemoveAt(delete);
                this.PortraitData.RefreshRenderableLayers();
            }
            Widgets.EndScrollView();
        }

        public override void PostClose() {
            base.PostClose();
            this.PortraitData.RefreshRenderableLayers();
        }
    }
}
